using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;

    [SerializeField] private PlayerManager player;
    [SerializeField] private GameSceneUIManager UI_Manager;
    [SerializeField] private LevelGenerator levelGenerator;
    [SerializeField] private Shop shop;
    [SerializeField] private ParticleManager particleManager;

    private GameSceneEventHandler eventHandler;
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
        shop.Init();
        particleManager.Init();

        eventHandler = new GameSceneEventHandler();
    }

    private void OnDestroy()
    {
        eventHandler.UnSubscribeAll();
    }

    public void Restart()
    {
        levelGenerator.Restart();
        player.Restart();
    }

    public void StartGame()
    {
        UI_Manager.Restart();
        AdManager.instance.HideBannerAd();
        player.isDead = false;

    }
}
