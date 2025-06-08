using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ShopUpgradeable : MonoBehaviour
{
    public UpgradeType UpgradeType;
    public TextMeshProUGUI moneyText;
    public Button button;
    public Image[] upgradeImage;
    public int[] Coins;
    public float[] values;
    public int upgradeIndex = 0;


    public void Init()
    {
        moneyText.text = Coins[upgradeIndex].ToString();
        for (int i = 0; i < upgradeImage.Length; i++)
        {
            if (upgradeIndex >= i)
            {
                upgradeImage[i].color = Color.white;
            }
            else
            {
                upgradeImage[i].color = Color.grey;
            }
        }

        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(() =>
        {
            Shop.instance.Upgrade(UpgradeType);
        });
    }
}

[System.Serializable]

public class ShopUpgradableData
{
    public UpgradeType UpgradeType;
    public int UpgradeIndex;
}
