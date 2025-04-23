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

    public SceneEnum currentSceneEnum;
    public Scene currentScene;

    private Dictionary<SceneEnum,string> sceneNamePairs = new Dictionary<SceneEnum, string>()
    {
        {SceneEnum.MainMenu,"MainMenu"},
        {SceneEnum.GameScene,"GameScene"}
    };
    public void Init()
    {
        instance = this;
        currentScene = SceneManager.GetActiveScene();

        currentSceneEnum = sceneNamePairs.FirstOrDefault(x => x.Value == SceneManager.GetActiveScene().name).Key;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    public void LoadScene(SceneEnum sceneName)
    {
        currentSceneEnum = sceneName;
        string sceneNameString = sceneNamePairs.GetValueOrDefault(sceneName,"MainMenu");
        SceneManager.LoadScene(sceneNameString);
    }

    public void OnSceneLoaded(Scene scene,LoadSceneMode loadSceneMode)
    {
        currentScene = scene;
        AdManager.instance.LoadRewardedAd();
        if (scene.name == "GameScene" || scene.name == "Taha map")
        {
            // Do something
            AdManager.instance.HideBannerAd();
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
