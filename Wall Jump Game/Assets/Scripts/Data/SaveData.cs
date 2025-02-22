using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public PlayerData playerData;

    public ShopUpgradableData[] shopUpgradableData;
    public int MusicVolume;
    public int SoundVolume;
    public bool TutorialOn;
    public SaveData()
    {
        playerData = new PlayerData();
        MusicVolume = 100;
        SoundVolume = 100;
        TutorialOn = true;

        shopUpgradableData = new ShopUpgradableData[7];
        for (int i = 0; i < shopUpgradableData.Length; i++)
        {
            shopUpgradableData[i] = new ShopUpgradableData();
            shopUpgradableData[i].UpgradeType = (UpgradeType)i;
            shopUpgradableData[i].UpgradeIndex = 0;
        }
    }

    public SaveData(SaveData data)
    {
        playerData = new PlayerData(data.playerData);
        MusicVolume = data.MusicVolume;
        SoundVolume = data.SoundVolume;
        TutorialOn = data.TutorialOn;

        shopUpgradableData = new ShopUpgradableData[7];
        for (int i = 0; i < shopUpgradableData.Length; i++)
        {
            shopUpgradableData[i] = new ShopUpgradableData();
            shopUpgradableData[i].UpgradeType = (UpgradeType)i;
            shopUpgradableData[i].UpgradeIndex = data.shopUpgradableData[i].UpgradeIndex;
        }
    }
}
