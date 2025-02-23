using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;

    [SerializeField] private PlayerManager player;
    [SerializeField] private GameSceneUIManager UI_Manager;
    [SerializeField] private LevelGenerator levelGenerator;
    public void Init()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        player.Init();
        UI_Manager.Init();
        levelGenerator.Init();
    }
}
