using UnityEngine;

public class IncubatorDoor : MonoBehaviour
{
    [SerializeField] private PlantingArea plantingArea;
    [SerializeField] private Transform doorPivot;
    [SerializeField] private Vector3 openRotationOffset = new Vector3(0f, 180f, 0f);
    [SerializeField] private float rotationSpeed = 180f; // graus por segundo

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Quaternion targetRotation;

    // Tudo é feito no Awake porque o Start não roda em um script desabilitado
    private void Awake()
    {
        // A rotação do pivô no prefab é a posição FECHADA
        closedRotation = doorPivot.localRotation;
        openRotation = closedRotation * Quaternion.Euler(openRotationOffset);

        // A incubadora começa vazia, então a porta começa aberta (sem animação)
        doorPivot.localRotation = openRotation;
        targetRotation = openRotation;

        plantingArea.StateChanged += HandleStateChanged;

        // O Update só roda enquanto a porta está se mexendo
        enabled = false;
    }

    private void OnDestroy()
    {
        if (plantingArea != null)
            plantingArea.StateChanged -= HandleStateChanged;
    }

    // Aberta quando vazia (colocar itens) ou pronta (retirar a planta);
    // fechada enquanto a semente cresce
    private void HandleStateChanged(PlantGrowthState state)
    {
        bool shouldBeOpen = state == PlantGrowthState.Empty
                         || state == PlantGrowthState.Ready;

        targetRotation = shouldBeOpen ? openRotation : closedRotation;
        enabled = true;
    }

    private void Update()
    {
        doorPivot.localRotation = Quaternion.RotateTowards(
            doorPivot.localRotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Quaternion.Angle(doorPivot.localRotation, targetRotation) < 0.01f)
        {
            doorPivot.localRotation = targetRotation;
            enabled = false;
            // FUTURO (áudio): disparar evento Opened/Closed aqui
        }
    }
}