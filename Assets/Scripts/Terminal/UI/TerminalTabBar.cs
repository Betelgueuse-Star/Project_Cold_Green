using UnityEngine;

// Destaca o botão da aba atual na barra inferior do terminal.
// Escuta o evento do TerminalNavigationController, então não precisa saber o que há dentro das telas.
public class TerminalTabBar : MonoBehaviour
{
    [SerializeField] private TerminalNavigationController navigation;

    [Tooltip("Um indicador por aba, na MESMA ordem do array de telas do TerminalNavigationController")]
    [SerializeField] private GameObject[] activeIndicators;

    private bool isConfigured;

    private void Awake()
    {
        isConfigured = navigation != null;

        if (!isConfigured)
            Debug.LogError($"{name}: TerminalNavigationController faltando, a barra de abas não vai destacar a aba atual.", this);
    }

    private void OnEnable()
    {
        if (!isConfigured)
            return;

        navigation.OnScreenChanged += Highlight;

        // Cobre mudanças que aconteceram enquanto a barra estava desativada.
        Highlight(navigation.CurrentScreenIndex);
    }

    private void OnDisable()
    {
        if (!isConfigured)
            return;

        navigation.OnScreenChanged -= Highlight;
    }

    private void Highlight(int index)
    {
        for (int i = 0; i < activeIndicators.Length; i++)
        {
            if (activeIndicators[i] != null)
                activeIndicators[i].SetActive(i == index);
        }
    }
}