
using UnityEngine;

public class CanvasUI : MonoBehaviour
{
    // General Canvas UI controller

    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private GameObject interactableImage;
    [SerializeField] private GameObject throwIndicator;

    [Header("Throw Indicator")]
    [SerializeField] private float minScale = 0f;
    [SerializeField] private float maxScale = 2f;

    private Vector3 originalThrowScale;

    private void Awake()
    {
        originalThrowScale = throwIndicator.transform.localScale;
    }

    private void OnEnable()
    {
        player.OnCanInteractChanged += SetInteractableVisible;
        player.OnThrowChargeChanged += UpdateThrowIndicator;

        SetInteractableVisible(false);
        SetThrowIndicatorVisible(false);
    }

    private void OnDisable()
    {
        player.OnCanInteractChanged -= SetInteractableVisible;
        player.OnThrowChargeChanged -= UpdateThrowIndicator;
    }

    private void SetInteractableVisible(bool visible)
    {
        interactableImage.SetActive(visible);
    }

    private void SetThrowIndicatorVisible(bool visible)
    {
        throwIndicator.SetActive(visible);
    }

    private void UpdateThrowIndicator(float chargePercent)
    {
        if (!throwIndicator.activeSelf)
            SetThrowIndicatorVisible(true);

        float scale = Mathf.Lerp(minScale, maxScale, chargePercent);

        throwIndicator.transform.localScale = originalThrowScale * scale;

        if (chargePercent >= 1f)
            SetThrowIndicatorVisible(true);
    }
}
