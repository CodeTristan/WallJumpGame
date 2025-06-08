using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum UpgradeType { Jump, Combo, Speed, SlowTime, SlowRate, SlowCount, SlingShot, DoublePoint, Bomber }

public class Shop : MonoBehaviour
{
    public static Shop instance;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI coinText;

    [Header("Upgrades")]
    [SerializeField] private ShopUpgradeable[] upgrades;


    private PlayerData playerData;

    public void Init()
    {
        instance = this;

        playerData = SaveSystem.instance.GameData.playerData;
        coinText.text = playerData.Coins.ToString();

        for (int i = 0; i < upgrades.Length; i++)
        {
            upgrades[i].Init();
        }
    }

    public void Upgrade(UpgradeType upgradeType)
    {
        ShopUpgradeable upgradeable = GetUpgradeable(upgradeType);

        if (upgradeable.upgradeIndex >= upgradeable.Coins.Length)
        {
            //DO SOMETHING IN UI
            Debug.Log("UPGRADE IS FULL");
            return;
        }
        if (upgradeable.Coins[upgradeable.upgradeIndex] > playerData.Coins)
        {
            //DO SOMETHING IN UI
            Debug.Log("NOT ENOUGH COIN");
            return;
        }

        playerData.Coins -= upgradeable.Coins[upgradeable.upgradeIndex];
        coinText.text = playerData.Coins.ToString();

        for (int i = 0; i < upgradeable.upgradeImage.Length; i++)
        {
            if (upgradeable.upgradeIndex >= i)
            {
                upgradeable.upgradeImage[i].color = Color.white;
            }
            else
            {
                upgradeable.upgradeImage[i].color = Color.grey;
            }
        }

        upgradeable.upgradeIndex++;
        upgradeable.moneyText.text = upgradeable.Coins[upgradeable.upgradeIndex].ToString();

        SetData();
        SaveSystem.instance.GameData.shopUpgradableData[(int)upgradeType].UpgradeIndex = upgradeable.upgradeIndex;
        SaveSystem.instance.SaveData();
    }

    private void SetData()
    {
        playerData.MaxJumpCount = (int)GetUpgradeable(UpgradeType.Jump).values[GetUpgradeable(UpgradeType.Jump).upgradeIndex];
        playerData.MaxCombo = (int)GetUpgradeable(UpgradeType.Combo).values[GetUpgradeable(UpgradeType.Combo).upgradeIndex];
        playerData.MaxSpeed = (int)GetUpgradeable(UpgradeType.Speed).values[GetUpgradeable(UpgradeType.Speed).upgradeIndex];
        playerData.MaxSlowTime = GetUpgradeable(UpgradeType.SlowTime).values[GetUpgradeable(UpgradeType.SlowTime).upgradeIndex];
        playerData.PowerUpData.SlingShotPower = (int)GetUpgradeable(UpgradeType.SlingShot).values[GetUpgradeable(UpgradeType.SlingShot).upgradeIndex];
        playerData.PowerUpData.DoublePointTimer = GetUpgradeable(UpgradeType.DoublePoint).values[GetUpgradeable(UpgradeType.DoublePoint).upgradeIndex];
        playerData.PowerUpData.BomberTimer = GetUpgradeable(UpgradeType.Bomber).values[GetUpgradeable(UpgradeType.Bomber).upgradeIndex];

    }

    private ShopUpgradeable GetUpgradeable(UpgradeType upgradeType)
    {
        foreach (ShopUpgradeable upgradeable in upgrades)
        {
            if (upgradeable.UpgradeType == upgradeType)
            {
                return upgradeable;
            }
        }

        Debug.LogError("Upgradeable not found!");
        return null;
    }
}
