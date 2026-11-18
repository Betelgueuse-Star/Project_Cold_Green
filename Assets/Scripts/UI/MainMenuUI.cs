using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    //[SerializeField] private GameObject creditMenu;
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    //[SerializeField] private Button creditsButton;
    //[SerializeField] private Button closeCreditsButton;

    private void Awake()
    {
        //creditMenu.SetActive(false);

        playButton.onClick.AddListener(() => //funcao anonima 
        {
            Loader.Load(Loader.SceneName.Prototype);
        });

        quitButton.onClick.AddListener(() => 
        {
            Application.Quit();
        });

        //creditsButton.onClick.AddListener(() =>
        //{
        //    creditMenu.SetActive(true);
        //});

        //closeCreditsButton.onClick.AddListener(() =>
        //{
        //    creditMenu.SetActive(false);
        //});

        Time.timeScale = 1f; // normaliza o tempo caso o jogador tenha pausado o jogo antes de voltar ao menu principal

        // reseta o cursor para o estado padrão (visível e destravado) caso o jogador tenha escondido o cursor antes de voltar ao menu principal
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //remove todos os listeners dos botões para evitar que eles sejam chamados mais de uma vez caso a cena seja recarregada
    private void OnDestroy()
    {
        playButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
        //creditsButton.onClick.RemoveAllListeners();
        //closeCreditsButton.onClick.RemoveAllListeners();
    }
}

