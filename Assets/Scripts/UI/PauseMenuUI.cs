using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        pausePanel.SetActive(false);

        resumeButton.onClick.AddListener(() => GameManager.Instance.Resume());
        mainMenuButton.onClick.AddListener(() => GameManager.Instance.QuitToMainMenu());

        GameManager.Instance.OnPauseChanged += HandlePauseChanged;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPauseChanged -= HandlePauseChanged;
    }

    private void HandlePauseChanged(bool paused)
    {
        pausePanel.SetActive(paused);
    }
}