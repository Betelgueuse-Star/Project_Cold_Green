using UnityEngine;

public class TerminalAudio : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Terminal terminal;
    [SerializeField] private TerminalModeController modeController;

    [Header("Sons do aparelho (3D, tocam no terminal)")]
    [SerializeField] private SoundSO bootSound;      // sfx_Terminal_Boot
    [SerializeField] private SoundSO shutdownSound;  // sfx_Terminal_Shutdown
    [SerializeField] private AudioSource runningLoop; // amb_Terminal_Running (opcional)

    [Header("Sons de interface (2D)")]
    [SerializeField] private SoundSO purchaseSound;  // sfx_Terminal_Purchase
    [SerializeField] private SoundSO upgradeSound;   // sfx_Terminal_Upgrade
    [SerializeField] private UISoundsSO uiSounds;    // actionBlocked (genérico)

    private bool isConfigured;

    private void Awake()
    {
        if (terminal == null)
            terminal = GetComponent<Terminal>();

        isConfigured = terminal != null && modeController != null;

        if (!isConfigured)
            Debug.LogError($"{name}: TerminalAudio sem referência ao Terminal ou ao TerminalModeController.", this);
    }

    private void OnEnable()
    {
        if (!isConfigured)
            return;

        modeController.OnTerminalEntered += HandleEntered;
        modeController.OnTerminalExited += HandleExited;
        terminal.OnItemPurchased += HandleItemPurchased;
        terminal.OnWaterUpgradePurchased += HandleUpgradePurchased;
        terminal.OnPurchaseBlocked += HandlePurchaseBlocked;
    }

    private void OnDisable()
    {
        if (!isConfigured)
            return;

        modeController.OnTerminalEntered -= HandleEntered;
        modeController.OnTerminalExited -= HandleExited;
        terminal.OnItemPurchased -= HandleItemPurchased;
        terminal.OnWaterUpgradePurchased -= HandleUpgradePurchased;
        terminal.OnPurchaseBlocked -= HandlePurchaseBlocked;
    }

    private void HandleEntered()
    {
        AudioManager.Instance.Play3D(bootSound, transform.position);
        SetLoop(true);
    }

    private void HandleExited()
    {
        AudioManager.Instance.Play3D(shutdownSound, transform.position);
        SetLoop(false);
    }

    private void HandleItemPurchased(ItemTypeSO item) => AudioManager.Instance.Play2D(purchaseSound);
    private void HandleUpgradePurchased() => AudioManager.Instance.Play2D(upgradeSound);

    private void HandlePurchaseBlocked()
    {
        if (uiSounds != null)
            AudioManager.Instance.Play2D(uiSounds.actionBlocked);
    }

    private void SetLoop(bool on)
    {
        if (runningLoop == null)
            return;

        if (on && !runningLoop.isPlaying) runningLoop.Play();
        else if (!on) runningLoop.Stop();
    }
}