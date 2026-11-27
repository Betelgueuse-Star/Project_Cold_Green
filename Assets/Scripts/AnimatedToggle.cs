using UnityEngine;

// Alterna entre aberto/fechado tocando UMA animação para frente (abrir)
// ou ao contrário (fechar). Se o jogador interagir no meio da animação,
// ela só inverte de onde está, sem cortar.
public class AnimatedToggle : MonoBehaviour
{
    [Header("Animação")]
    [SerializeField] private Animator animator;
    [SerializeField] private string openStateName = "Open"; // nome do state com o clip de abrir
    [SerializeField] private string speedParameter = "Speed"; // float ligado ao Multiplier do state
    [SerializeField] private bool startsOpen = false;

    private bool isOpen;
    private int speedHash;

    public bool IsOpen => isOpen;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError($"{name}: Animator não encontrado.", this);
            return;
        }

        speedHash = Animator.StringToHash(speedParameter);
        isOpen = startsOpen;

        // Começa parado no primeiro frame (fechado) ou no último (aberto)
        animator.Play(openStateName, 0, startsOpen ? 1f : 0f);
        animator.SetFloat(speedHash, 0f);
    }

    // Chamado pelo UnityEvent do QuickInteract
    public void Toggle()
    {
        if (animator == null) return;

        isOpen = !isOpen;

        // Progresso atual limitado entre 0 e 1 (evita o "atraso" ao inverter)
        float progress = Mathf.Clamp01(
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime);

        animator.Play(openStateName, 0, progress);
        animator.SetFloat(speedHash, isOpen ? 1f : -1f);
    }
}