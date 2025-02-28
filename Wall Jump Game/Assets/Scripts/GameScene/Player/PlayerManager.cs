using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public PlayerHealth playerHealth;
    public PlayerPowerUps playerPowerUps;
    public PlayerCollisionHandler playerCollisionHandler;
    public PlayerMovement playerMovement;
    public PlayerSprite playerSprite;

    public int Point;
    public float DieFromHeightTreshHold;
    public float EnemyKillCounterTimer;
    public float BarePassCounterTimer;
    public PlayerData playerData;
    public bool OnWall;
    public bool OnInvisWall;
    public bool isDead;

    private PlayerEventHandler PlayerEventHandler;

    private float yPos;

    public void Init()
    {
        instance = this;
        PlayerEventHandler = new PlayerEventHandler();
        playerData = SaveSystem.instance.GameData.playerData;
        yPos = transform.position.y;

        playerHealth.Init();
        playerPowerUps.Init();
        playerCollisionHandler.Init();
        playerMovement.Init();
        playerSprite.Init();
    }

    private void Update()
    {
        //Point Adjustment
        if (transform.position.y > yPos + 1)
        {
            Point = Point + (1 * playerPowerUps.PointExponent);
            yPos = transform.position.y;
            GameSceneUIManager.instance.UpdatePointText();
        }

        if(LevelGenerator.instance.currentLevel != null && yPos > LevelGenerator.instance.currentLevel.EndLine.transform.position.y)
        {
            LevelGenerator.instance.OnLevelCompletedEvent();
        }

        if (yPos - transform.position.y > DieFromHeightTreshHold && isDead == false)
        {
            PlayerEventHandler.instance.PlayerDied();
            return;
        }
    }


    public void Restart()
    {
        
    }


}
