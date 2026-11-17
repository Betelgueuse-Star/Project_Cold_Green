using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public event Action<bool>OnCanInteractChanged;
    public event Action<float> OnThrowChargeChanged;

    // novos: usados pelo PlayerSounds (e depois pelas animações)
    public event Action OnItemGrabbed;
    public event Action OnItemDropped;
    public event Action OnItemThrown;

    [Header("Referências")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private Transform toolHoldPointTransform;

    [Header("Interação")]
    [SerializeField] private LayerMask interactionLayerMask;
    [SerializeField] private float interactionRange = 3f;

    [Header("Arremesso")]
    [SerializeField] private float maxChargeTime = 2f;
    [SerializeField] private float minThrowForce = 3f;
    [SerializeField] private float maxThrowForce = 15f;

    private float throwChargeTime;
    private bool isChargingThrow;

    private Tool currentTool;

    private ObjectGrabbable objectGrabbable; // Objeto físico que está sendo carregado.
    private InteractableOutline currentOutline; // Outline que está aceso no momento.

    private RaycastHit currentHit;
    private bool hasHit;

    private Collider lastCollider;

    private bool canInteract;
    private bool targetIsPickup;        // ObjectGrabbable ou Tool
    private bool targetIsInteractable;  // IInteractable

    private float ThrowChargePercent => Mathf.Clamp01(throwChargeTime / maxChargeTime);

    public ToolType CurrentToolType => currentTool != null ? currentTool.Type : ToolType.None;
    public Tool CurrentTool => currentTool;
    public bool IsHoldingSomething => objectGrabbable != null || currentTool != null;



    private void Update()
    {
        hasHit = Physics.Raycast(
            cameraTransform.position,
            cameraTransform.forward,
            out currentHit,
            interactionRange,
            interactionLayerMask);

        UpdateTarget();

        if (isChargingThrow)
        {
            throwChargeTime += Time.deltaTime;
            throwChargeTime = Mathf.Min(throwChargeTime, maxChargeTime);

            OnThrowChargeChanged?.Invoke(ThrowChargePercent); //evento para a UI atualizar a barra de arremesso
        }
    }

    private void UpdateTarget()
    {
        if (!hasHit)
        {
            lastCollider = null;
            targetIsPickup = false;
            targetIsInteractable = false;
            HideCurrentOutline();
            SetCanInteract(false);
            return;
        }

        // Só busca os componentes quando muda de alvo
        if (currentHit.collider != lastCollider)
        {
            lastCollider = currentHit.collider;
            Transform t = currentHit.transform;

            targetIsPickup = t.TryGetComponent(out ObjectGrabbable _) || t.TryGetComponent(out Tool _);

            // Procura também nos pais, para que colliders filhos (vidro, corpo da incubadora)
            // repassem a interação para o IInteractable do objeto raiz
            targetIsInteractable = t.GetComponentInParent<IInteractable>() != null;

            HideCurrentOutline();

            if (!IsHoldingSomething && t.TryGetComponent(out InteractableOutline outline))
            {
                currentOutline = outline;
                currentOutline.Show();
            }
        }
    }

    private void SetCanInteract(bool value)
    {
        if (canInteract == value) // só dispara o evento quando o estado muda
            return; 

        canInteract = value;
        OnCanInteractChanged?.Invoke(canInteract);
    }

    private float CalculateThrowForce()
    {
        float chargePercent = Mathf.Clamp01(throwChargeTime / maxChargeTime);

        return Mathf.Lerp(minThrowForce, maxThrowForce, chargePercent);
    }

    private void ThrowObject()
    {
        if (objectGrabbable == null || !objectGrabbable.CanBeThrown)
            return;

        float force = CalculateThrowForce();
        Vector3 direction = cameraTransform.forward;

        objectGrabbable.Throw(direction, force);
        OnItemThrown?.Invoke(); // novo

        objectGrabbable = null;
        throwChargeTime = 0f;
        isChargingThrow = false;
    }

    public void OnThrow(InputValue value)
    {
        if (objectGrabbable == null || !objectGrabbable.CanBeThrown)
            return;

        if (value.isPressed)
        {
            isChargingThrow = true;
            throwChargeTime = 0f;

            OnThrowChargeChanged?.Invoke(0f);
        }
        else if (isChargingThrow)
        {
            ThrowObject();
            OnThrowChargeChanged?.Invoke(0f);
        }
    }

    private void HideCurrentOutline()
    {
        if (currentOutline == null)
            return;

        currentOutline.Hide();
        currentOutline = null;
    }

    private void DropCurrentItem()
    {
        lastCollider = null;

        // Caso 1: objeto físico
        if (objectGrabbable != null)
        {
            objectGrabbable.OnDrop();
            objectGrabbable = null;
            OnItemDropped?.Invoke(); // novo
            return;
        }

        // Caso 2: ferramenta
        if (currentTool != null)
        {
            currentTool.Drop(toolHoldPointTransform.position, transform.rotation);
            currentTool = null;
            OnItemDropped?.Invoke(); // novo
        }
    }

    private void GrabTool(Tool newTool)
    {
        if (IsHoldingSomething)
            return;

        lastCollider = null;
        HideCurrentOutline();

        newTool.Equip(toolHoldPointTransform);
        currentTool = newTool;
        OnItemGrabbed?.Invoke(); // novo
    }

    public void GrabObject(ObjectGrabbable newObjectGrabbable)
    {
        if (IsHoldingSomething)
            return;

        lastCollider = null;
        HideCurrentOutline();

        objectGrabbable = newObjectGrabbable;
        objectGrabbable.OnGrab(objectGrabPointTransform);
        OnItemGrabbed?.Invoke(); // novo
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (!hasHit)
            return;

        Transform target = currentHit.transform;

        // A ordem importa: objeto pegável > ferramenta > interagível genérico.
        if (target.TryGetComponent(out ObjectGrabbable grabbable))
        {
            GrabObject(grabbable);
        }
        else if (target.TryGetComponent(out Tool tool))
        {
            GrabTool(tool);
        }
        else
        {
            IInteractable interactable = target.GetComponentInParent<IInteractable>();

            if (interactable != null)
                interactable.Interact(this);
        }
    }

    public void OnDrop(InputValue value)
    {
        if (!value.isPressed)
            return;

        DropCurrentItem();
    }

    // Pega a ferramenta como um tipo específico com generic (ex.: WateringCan)
    public bool TryGetCurrentTool<T>(out T tool) where T : Tool
    {
        tool = currentTool as T;
        return tool != null;
    }
}