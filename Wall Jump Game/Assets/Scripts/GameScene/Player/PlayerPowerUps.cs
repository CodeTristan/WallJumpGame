using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPowerUps : MonoBehaviour
{
    public enum powerUpType
    {
        DoublePoint,
        Bomber,
        SlingShot
    }

    public int PointExponent = 1;

    private float currentBomberTimer;
    private float currentDoublePointTimer;

    public List<powerUpType> currentPowerUps = new List<powerUpType>();

    private PlayerMovement PlayerMovement;
    private PlayerData PlayerData;
    public void Init()
    {
        PlayerMovement = PlayerManager.instance.playerMovement;
        PlayerData = PlayerManager.instance.playerData;
    }

    private void Update()
    {
        if (currentPowerUps.Contains(powerUpType.DoublePoint))
        {
            currentDoublePointTimer -= Time.deltaTime;
            GameSceneUIManager.instance.UpdateDoublePointText(currentDoublePointTimer);
            if (currentDoublePointTimer <= 0)
            {
                GameSceneUIManager.instance.ToggleDoublePointText(false);
                currentPowerUps.Remove(powerUpType.DoublePoint);
                PointExponent = 1;
            }
        }

        if (currentPowerUps.Contains(powerUpType.Bomber))
        {
            currentBomberTimer -= Time.deltaTime;
            GameSceneUIManager.instance.UpdateBomberText(currentBomberTimer);
            if (currentBomberTimer <= 0)
            {
                GameSceneUIManager.instance.ToggleBomberText(false);
                currentPowerUps.Remove(powerUpType.Bomber);
            }
        }
    }

    public bool HasPowerUp(powerUpType powerUp)
    {
        return currentPowerUps.Contains(powerUp);
    }

    public void AddPowerUp(powerUpType powerUp)
    {
        if (currentPowerUps.Contains(powerUp))
        {
            switch (powerUp)
            {
                case powerUpType.DoublePoint:
                    currentDoublePointTimer = PlayerData.PowerUpData.DoublePointTimer;
                    PointExponent++;
                    break;
                case powerUpType.Bomber:
                    currentBomberTimer = PlayerData.PowerUpData.BomberTimer;
                    break;
            }
        }
        else
        {
            currentPowerUps.Add(powerUp);
            switch (powerUp)
            {
                case powerUpType.DoublePoint:
                    GameSceneUIManager.instance.ToggleDoublePointText(true);
                    currentDoublePointTimer = PlayerData.PowerUpData.DoublePointTimer;
                    PointExponent = 2;
                    break;
                case powerUpType.Bomber:
                    GameSceneUIManager.instance.ToggleBomberText(true);
                    currentBomberTimer = PlayerData.PowerUpData.BomberTimer;
                    break;
                case powerUpType.SlingShot:
                    PlayerMovement.ResetVelocity();
                    PlayerMovement.rb.AddForce(Vector2.up * PlayerData.PowerUpData.SlingShotPower, ForceMode2D.Impulse);
                    break;
            }
        }
    }
}
