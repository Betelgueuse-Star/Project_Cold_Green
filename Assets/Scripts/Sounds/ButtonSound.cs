using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    [SerializeField] private UISoundsSO uiSounds; // um asset so, configurado no prefab

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(PlayClick);    // cadastra
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(PlayClick); // descadastra
    }

    private void PlayClick()
    {
        AudioManager.Instance.Play2D(uiSounds.click);
    }
}