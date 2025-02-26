using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneEnum
{
    MainMenu,
    GameScene
}
public class SahneManager : MonoBehaviour
{
    public static SahneManager instance;

    public SceneEnum currentScene;

    private Dictionary<SceneEnum,string> sceneNamePairs = new Dictionary<SceneEnum, string>()
    {
        {SceneEnum.MainMenu,"MainMenu"},
        {SceneEnum.GameScene,"GameScene"}
    };
    public void Init()
    {
        instance = this;

        currentScene = sceneNamePairs.FirstOrDefault(x => x.Value == SceneManager.GetActiveScene().name).Key;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    public void LoadScene(SceneEnum sceneName)
    {
        string sceneNameString = sceneNamePairs.GetValueOrDefault(sceneName,"MainMenu");
        SceneManager.LoadScene(sceneNameString);
    }

    public void OnSceneLoaded(Scene scene,LoadSceneMode loadSceneMode)
    {
        AdManager.instance.DestroyBannerAd();
        if (scene.name == "GameScene")
        {
            // Do something
            FindObjectOfType<GameSceneManager>().Init();
        }
        else if (scene.name == "MainMenu")
        {
            // Do something
            FindObjectOfType<MainMenuManager>().Init();
            AdManager.instance.LoadBannerAd();
            AdManager.instance.ShowBannerAd();
        }
    }
}
