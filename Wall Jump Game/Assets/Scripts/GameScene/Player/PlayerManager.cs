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

    public const int MAX_PLAYER_RESPAWN_COUNT = 3;
    public int Point;
    public float DieFromHeightTreshHold;
    public float EnemyKillCounterTimer;
    public float BarePassCounterTimer;
    public PlayerData playerData;
    public bool OnWall;
    public bool OnInvisWall;
    public bool isDead = true;

    private float yPos;
    private int currentPlayerRespawnCount;


    private Vector2 startPos;

    public void Init()
    {
        instance = this;
        playerData = SaveSystem.instance.GameData.playerData;
        yPos = transform.position.y;
        startPos = transform.position;

        playerHealth.Init();
        playerPowerUps.Init();
        playerCollisionHandler.Init();
        playerMovement.Init();
        playerSprite.Init();

    }


    private void Update()
    {
        if (AdManager.instance.InAdMenu) //WAIT IN AD MENU
            return;



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
            GameSceneEventHandler.instance.PlayerDied();
            return;
        }
    }

    
    public void OnPlayerDied()
    {
        currentPlayerRespawnCount++;
        GameSceneUIManager.instance.ToggleDeathAdScreen(true);
        //if(currentPlayerRespawnCount < MAX_PLAYER_RESPAWN_COUNT)
        //{
        //    currentPlayerRespawnCount++;
        //    GameSceneUIManager.instance.ToggleDeathAdScreen(true);
        //}
        //else
        //{
        //    GameSceneEventHandler.instance.PlayerDiedFR();
        //}
    }

    public void OnPlayerDieFR()
    {
        if (Point > playerData.MaxPoint)
            playerData.MaxPoint = Point;

        playerData.Coins += Point / 5;

        SaveSystem.instance.SaveData();
    }


    public void Restart()
    {

        Point = 0;
        currentPlayerRespawnCount = 0;
        yPos = 0;
        Camera.main.transform.position = new Vector3(0,0,-10);

        playerHealth.Init();
        playerMovement.Init();
        playerSprite.Init();  //Also enables the collider
        playerSprite.Reset();

        transform.position = startPos;
        playerMovement.ResetValues();
        playerMovement.currentJumpCount++;
    }

    public void Respawn()
    {
        transform.position = new Vector2(0, yPos);


        playerHealth.Init();
        playerMovement.Init();
        playerSprite.Init();  //Also enables the collider

        playerPowerUps.AddPowerUp(PowerUpType.DoublePoint);
        playerPowerUps.AddPowerUp(PowerUpType.SlingShot);
    }
}
