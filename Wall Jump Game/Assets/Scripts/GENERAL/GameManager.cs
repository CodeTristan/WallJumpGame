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


    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        musicManager.Init();
        saveSystem.Init();
        sahneManager.Init();

        adManager.Init();

        DontDestroyOnLoad(gameObject);

        //if(FirstSceneToLoad != sahneManager.currentScene)
        //    sahneManager.LoadScene(FirstSceneToLoad);

    }
}
