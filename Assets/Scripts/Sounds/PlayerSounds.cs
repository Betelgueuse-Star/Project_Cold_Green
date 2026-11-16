using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Player player;
    [SerializeField] private CharacterController characterController;

    [Header("Sons")]
    [SerializeField] private SoundSO grabSound;
    [SerializeField] private SoundSO dropSound;
    [SerializeField] private SoundSO throwSound;
    [SerializeField] private SoundSO footstepSound;
    [SerializeField] private SoundSO landSound;

    [Header("Passos")]
    [SerializeField] private float stepDistance = 2f;        // metros andados entre um passo e outro
    [SerializeField] private float minSpeedForSteps = 0.5f;  // abaixo disso não toca passo
    [SerializeField] private float minAirTimeForLanding = 0.25f; // evita som de pouso ao descer rampas

    private float distanceWalked;
    private float airTime;
    private bool wasGrounded = true;

    private void Awake()
    {
        // referências essenciais: valida uma vez
        if (player == null) player = GetComponent<Player>();
        if (characterController == null) characterController = GetComponent<CharacterController>();

        if (player == null)
            Debug.LogError($"{name}: PlayerSounds sem referência ao Player.");
        if (characterController == null)
            Debug.LogError($"{name}: PlayerSounds sem referência ao CharacterController.");
    }

    private void OnEnable()
    {
        player.OnItemGrabbed += HandleGrabbed;
        player.OnItemDropped += HandleDropped;
        player.OnItemThrown += HandleThrown;
    }

    private void OnDisable() // sempre desinscreve
    {
        player.OnItemGrabbed -= HandleGrabbed;
        player.OnItemDropped -= HandleDropped;
        player.OnItemThrown -= HandleThrown;
    }

    private void Update()
    {
        HandleFootsteps();
    }

    // Sons "na mão" do jogador: 2D, porque sempre acontecem junto do ouvinte.
    private void HandleGrabbed() => AudioManager.Instance.Play2D(grabSound);
    private void HandleDropped() => AudioManager.Instance.Play2D(dropSound);
    private void HandleThrown() => AudioManager.Instance.Play2D(throwSound);

    private void HandleFootsteps()
    {
        bool grounded = characterController.isGrounded;

        if (!grounded)
        {
            airTime += Time.deltaTime;
        }
        else
        {
            // acabou de pousar, mas só toca se ficou tempo suficiente no ar
            if (!wasGrounded && airTime >= minAirTimeForLanding)
                AudioManager.Instance.Play3D(landSound, transform.position);

            airTime = 0f;
        }
        wasGrounded = grounded;

        if (!grounded)
            return;

        // velocidade horizontal (ignora subir/descer)
        Vector3 velocity = characterController.velocity;
        velocity.y = 0f;
        float speed = velocity.magnitude;

        if (speed < minSpeedForSteps)
            return;

        // passos por distância: correndo, os passos vêm mais rápido sozinhos
        distanceWalked += speed * Time.deltaTime;
        if (distanceWalked >= stepDistance)
        {
            distanceWalked = 0f;
            AudioManager.Instance.Play3D(footstepSound, transform.position);
        }
    }
}