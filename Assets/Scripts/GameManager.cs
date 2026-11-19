using System;
using StarterAssets;
using UnityEngine;

// Gerencia o estado global do jogo (por enquanto: pausa).
// Fica na GameScene. Não é static nem DontDestroyOnLoad: ele nasce e morre com a cena,
// então não deixa estado "fantasma" ao voltar para o menu.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // A UI de pausa (e futuramente áudio, etc.) escuta este evento.
    public event Action<bool> OnPauseChanged;

    [Header("Player")]
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private StarterAssetsInputs playerInputs;

    [Header("Terminal")]
    [Tooltip("Opcional: se preenchido, o ESC dentro do terminal sai do terminal em vez de pausar.")]
    [SerializeField] private TerminalModeController terminalMode;

    public bool IsPaused { get; private set; }

    private bool isConfigured;

    private void Awake()
    {
        // Singleton simples, limitado à cena.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        isConfigured = playerController != null && playerInputs != null;

        if (!isConfigured)
            Debug.LogError($"{name}: referência faltando (playerController ou playerInputs), a pausa não vai funcionar.", this);

        if (terminalMode == null)
            Debug.LogWarning($"{name}: terminalMode não atribuído, o ESC pausa mesmo dentro do terminal.", this);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void TogglePause()
    {
        // Dentro do terminal, o ESC primeiro sai do terminal.
        if (!IsPaused && terminalMode != null && terminalMode.IsInTerminalMode)
        {
            terminalMode.ExitTerminalMode();
            return;
        }

        SetPaused(!IsPaused);
    }

    public void SetPaused(bool paused)
    {
        if (!isConfigured || IsPaused == paused)
            return;

        IsPaused = paused;

        Time.timeScale = paused ? 0f : 1f;

        // Pausado: controles desligados e cursor livre para clicar no menu.
        playerController.SetControlsEnabled(!paused);
        playerInputs.SetCursorLocked(!paused);

        OnPauseChanged?.Invoke(paused);
    }

    // Para o botão "Continuar" do menu de pausa.
    public void Resume() => SetPaused(false);

    // Para o botão "Menu principal" do menu de pausa.
    public void QuitToMainMenu()
    {
        // O Loader já restaura o timeScale, mas deixamos o estado coerente.
        IsPaused = false;
        Loader.Load(Loader.SceneName.MainMenuScene);
    }
}