using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private SaveSystem saveSystem;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private SahneManager sahneManager;
    void Awake()
    {
        instance = this;

        musicManager.Init();
        saveSystem.Init();
        sahneManager.Init();

        DontDestroyOnLoad(gameObject);

        sahneManager.LoadScene(SceneEnum.MainMenu);
    }
}
