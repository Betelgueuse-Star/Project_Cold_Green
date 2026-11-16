using UnityEngine;
using UnityEngine.InputSystem;

public class AudioTest : MonoBehaviour
{
    [SerializeField] private SoundSO testSound;

    private void Update()
    {
        // 1 = som 2D (ouve igual de qualquer lugar)
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            AudioManager.Instance.Play2D(testSound);

        // 2 = som 3D tocando na posicao deste objeto
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            AudioManager.Instance.Play3D(testSound, transform.position);
    }
}