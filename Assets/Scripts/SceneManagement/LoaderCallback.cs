using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// This class is responsible for calling the Loader's callback method after the loading scene has been loaded
// It ensures that the target scene is loaded after the loading scene has been displayed for at least one frame
// Tem que estar em um gameobject na cena de loading para que o callback seja chamado corretamente
public class LoaderCallback : MonoBehaviour
{
    [SerializeField] private Slider progressBar; // opcional

    private IEnumerator Start()
    {
        yield return null; // espera 1 frame para a tela de loading aparecer

        AsyncOperation op = Loader.LoadTargetAsync();
        while (!op.isDone)
        {
            if (progressBar != null)
                progressBar.value = Mathf.Clamp01(op.progress / 0.9f); // o Unity para em 0.9 até ativar a cena
            yield return null;
        }
    }
}
