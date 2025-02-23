using System.Collections;
using System.Collections.Generic;
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

    private Dictionary<SceneEnum,string> sceneNamePairs = new Dictionary<SceneEnum, string>()
    {
        {SceneEnum.MainMenu,"MainMenu"},
        {SceneEnum.GameScene,"GameScene"}
    };
    public void Init()
    {
        instance = this;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    public void LoadScene(SceneEnum sceneName)
    {
        string sceneNameString = sceneNamePairs.GetValueOrDefault(sceneName,"MainMenu");
        SceneManager.LoadScene(sceneNameString);
    }

    public void OnSceneLoaded(Scene scene,LoadSceneMode loadSceneMode)
    {
        if(scene.name == "GameScene")
        {
            // Do something
            FindObjectOfType<GameSceneManager>().Init();
        }
        else if (scene.name == "MainMenu")
        {
            // Do something
            FindObjectOfType<MainMenuManager>().Init();

        }
    }
}
