using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int MaxHealth;
    public int Coins;
    public int MaxJumpCount;
    public int MaxCombo;
    public int MaxSpeed;
    public float SlowRate;
    public float MaxSlowTime;
    public int MaxSlowUsage;



    public PlayerPowerUpData PowerUpData;


    public PlayerData()
    {
        MaxHealth = 1;
        Coins = 0;
        MaxJumpCount = 2;
        MaxCombo = 2;
        MaxSpeed = 11;
        SlowRate = 0.2f;
        MaxSlowTime = 0.6f;
        MaxSlowUsage = 1;

        PowerUpData = new PlayerPowerUpData();
    }

    public PlayerData(PlayerData data)
    {
        MaxHealth = data.MaxHealth;
        Coins = data.Coins;
        MaxJumpCount = data.MaxJumpCount;
        MaxCombo = data.MaxCombo;
        MaxSpeed = data.MaxSpeed;
        SlowRate = data.SlowRate;
        MaxSlowTime = data.MaxSlowTime;
        MaxSlowUsage = data.MaxSlowUsage;

        PowerUpData = new PlayerPowerUpData();
        PowerUpData.SlingShotPower = data.PowerUpData.SlingShotPower;
        PowerUpData.DoublePointTimer = data.PowerUpData.DoublePointTimer;
        PowerUpData.BomberTimer = data.PowerUpData.BomberTimer;
    }
}

[System.Serializable]
public class PlayerPowerUpData
{
    public int SlingShotPower;
    public float DoublePointTimer;
    public float BomberTimer;

    public PlayerPowerUpData()
    {
        SlingShotPower = 11;
        DoublePointTimer = 10;
        BomberTimer = 10;
    }

    public PlayerPowerUpData(PlayerPowerUpData data)
    {
        SlingShotPower = data.SlingShotPower;
        DoublePointTimer = data.DoublePointTimer;
        BomberTimer = data.BomberTimer;
    }
}