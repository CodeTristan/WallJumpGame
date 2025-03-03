using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private SceneEnum FirstSceneToLoad;
    [SerializeField] private SaveSystem saveSystem;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private SahneManager sahneManager;
    [SerializeField] private AdManager adManager;

    public int Number_Of_Game_Before_Ad = 3;
    public int current_Game_Before_Ad = 0;
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        current_Game_Before_Ad = Number_Of_Game_Before_Ad;

        musicManager.Init();
        saveSystem.Init();
        sahneManager.Init();

        adManager.Init();

        DontDestroyOnLoad(gameObject);

        if (FirstSceneToLoad != sahneManager.currentSceneEnum || sahneManager.currentScene.name == "SplashScene")
            sahneManager.LoadScene(FirstSceneToLoad);

    }
}
