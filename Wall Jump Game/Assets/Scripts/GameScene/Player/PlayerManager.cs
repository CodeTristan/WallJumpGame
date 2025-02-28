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
        ////Level Generator
        //completedLevels = 0;
        //levelGenerator.resetGame();
        //levelGenerator.MakeGame();

        //restartMenu.SetActive(false);
        //currentHealth = Health;
        //transform.position = new Vector3(0, -12, 0);
        //cam.transform.position = camStartPos;
        //yPos = transform.position.y;

        //Point = 0;
        //rb.gravityScale = gravityScale;
        //slowUsed = false;

        //jumpCount = maxJumpCount + 1;
        //slowUsage = maxSlowUsage + 1;

        //resetVelocity();
        //resetted = true;
        //died = false;

        //currentBarePassTimer = BarePassTimer;
        //BarePassExponent = 0;
        //BarePassXCount.gameObject.SetActive(false);
        //BarePassXCountTimer.gameObject.SetActive(false);
        //barePassImage.SetActive(false);
        //pointText.gameObject.SetActive(true);

        //currentKillCountTimer = KillCountTimer;
        //KillCountExponent = 0;
        //KillCountX.gameObject.SetActive(false);
        //KillCountXTimer.gameObject.SetActive(false);
        //killCountImage.SetActive(false);

        //GainedCoinText.gameObject.GetComponent<Animator>().SetBool("Dead", false);
        //Time.timeScale = 1;
        //FindObjectOfType<PlayerSprite>().AdjustSphereStart();
        //currentCoin = Coin;
        //rb.constraints = RigidbodyConstraints2D.None;

    }


}
