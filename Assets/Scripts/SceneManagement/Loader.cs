using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class Loader //static class is a class that cannot be instantiated and can only contain static members
{
    // Variaveis estaticas sao compartilhadas entre todas as instancias da classe, ou seja,
    // se uma instancia alterar o valor de uma variavel estatica, todas as outras instancias terao acesso a esse novo valor
    // Variaveis estaticas sao inicializadas apenas uma vez, quando a classe é carregada pela primeira vez, e nao sao destruídas quando a cena é trocada

    public enum SceneName
    {
        MainMenuScene,
        Prototype,
        LoadingScene,
    }

    public static SceneName TargetScene { get; private set; }

    public static void Load(SceneName target)
    {
        Time.timeScale = 1f;// garante que o tempo esteja normalizado ao carregar uma nova cena

        TargetScene = target;// define a cena de destino que será carregada após a cena de loading

        SceneManager.LoadScene(SceneName.LoadingScene.ToString());
    }

    // mostra o progress do carregamento da cena de destino, que é carregada de forma assíncrona
    public static AsyncOperation LoadTargetAsync()
    {
        return SceneManager.LoadSceneAsync(TargetScene.ToString());
    }
}

