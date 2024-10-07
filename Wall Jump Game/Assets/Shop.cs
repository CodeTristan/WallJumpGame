using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    public int coin;
    public TextMeshProUGUI coinText;
    public Button[] buttons;

    [Header("MaxJump")]
    public GameObject JumpUpgrade;
    public int[] JumpCoin;
    public TextMeshProUGUI jumpMoneyText;
    public Image[] jumpCountImage;
    public int jumpCountIndex = 2;
    private int maxJump;

    [Header("MaxCombo")]
    public GameObject ComboUpgrade;
    public int[] ComboCoin;
    public TextMeshProUGUI comboMoneyText;
    public Image[] comboImage;
    public int comboIndex;
    private int maxCombo;

    [Header("Speed")]
    public GameObject SpeedUpgrade;
    public int[] SpeedCoin;
    public TextMeshProUGUI speedMoneyText;
    public Image[] SpeedImage;
    public int speedIndex = 2;
    private int speed;

    [Header("SlowTime")]
    public GameObject SlowTimeUpgrade;
    public int[] SlowTimeCoin;
    public TextMeshProUGUI slowTimeMoneyText;
    public Image[] SlowTimeImage;
    public int slowTimeIndex = 2;
    private float slowTime;

    [Header("SlingShot")]
    public GameObject SlingShotUpgrade;
    public int[] SlingShotCoin;
    public TextMeshProUGUI slingShotMoneyText;
    public Image[] SlingShotImage;
    public int slingShotIndex = 0;
    private int slingShotPower;

    [Header("DoublePoint")]
    public GameObject DoublePointUpgrade;
    public int[] DoublePointCoin;
    public TextMeshProUGUI doublePointMoneyText;
    public Image[] DoublePointImage;
    public int DoublePointIndex = 0;
    private float DoublePointTimer;

    [Header("Bomber")]
    public GameObject BomberUpgrade;
    public int[] BomberCoin;
    public TextMeshProUGUI BomberText;
    public Image[] BomberImage;
    public int BomberIndex = 0;
    private float BomberTimer;

    private void OnEnable()
    {
        if (PlayerPrefs.GetInt("MaxCombo") == 0)
            PlayerPrefs.SetInt("MaxCombo", 2);
        if (PlayerPrefs.GetInt("MaxJump") == 0)
            PlayerPrefs.SetInt("MaxJump", 2);
        if (PlayerPrefs.GetInt("MaxSpeed") == 0)
            PlayerPrefs.SetInt("MaxSpeed", 11);

        if (PlayerPrefs.GetInt("slingShotPower") == 0)
            PlayerPrefs.SetInt("slingShotPower", 30);
        if (PlayerPrefs.GetFloat("doublePointTimer") == 0)
            PlayerPrefs.SetFloat("doublePointTimer", 10);
        if (PlayerPrefs.GetFloat("bomberTimer") == 0)
            PlayerPrefs.SetFloat("bomberTimer", 6);


        jumpCountIndex = PlayerPrefs.GetInt("MaxJump");
        coin = PlayerPrefs.GetInt("Coin");
        comboIndex = PlayerPrefs.GetInt("MaxCombo");
        speedIndex = PlayerPrefs.GetInt("MaxSpeed");
        slowTimeIndex = PlayerPrefs.GetInt("slowTimeIndex");
        
        BomberIndex = PlayerPrefs.GetInt("bomberIndex");
        DoublePointIndex = PlayerPrefs.GetInt("doublePointIndex");
        slingShotIndex = PlayerPrefs.GetInt("slingShotIndex");


        maxJump = jumpCountIndex;
        maxCombo = comboIndex;
        speed = speedIndex;
        slowTime = PlayerPrefs.GetFloat("MaxSlowTime"); ;
        BomberTimer = PlayerPrefs.GetFloat("bomberTimer");
        DoublePointTimer = PlayerPrefs.GetFloat("doublePointTimer");
        slingShotPower = PlayerPrefs.GetInt("slingShotPower");

        jumpCountIndex -= 2;
        comboIndex -= 2;
        speedIndex -= 11;

        AdjustTexts();

        AdjustButtons();

        AdjustCompletedUpgrades();

        FindObjectOfType<MenuManager>().goShopPage1();
    }
    public void JumpCount()
    {
        PlayerPrefs.SetInt("bomberIndex",BomberIndex);
        PlayerPrefs.SetInt("doublePointIndex",DoublePointIndex);
        PlayerPrefs.SetInt("slingShotIndex",slingShotIndex);
        if (coin >= JumpCoin[jumpCountIndex])
        {
            maxJump++;
            PlayerPrefs.SetInt("MaxJump", maxJump);
            jumpCountImage[jumpCountIndex].color = new Color32(255, 255, 255, 255);
            coin -= JumpCoin[jumpCountIndex];
            PlayerPrefs.SetInt("Coin", coin);
            jumpCountIndex++;
            if (jumpCountIndex >= JumpCoin.Length)
            {
                jumpMoneyText.text = "Maxed Out";
                JumpUpgrade.GetComponentInParent<Button>().enabled = false;
            }
            AdjustTexts();
            AdjustButtons();
            AdjustCompletedUpgrades();
        }
        else
        {
            //AddAnimation
            Debug.Log("Not Enough Money");
        }
    }
    public void MaxCombo()
    {
        if (coin >= ComboCoin[comboIndex])
        {
            maxCombo++;
            PlayerPrefs.SetInt("MaxCombo", maxCombo);
            comboImage[comboIndex].color = new Color32(255, 255, 255, 255);
            coin -= ComboCoin[comboIndex];
            PlayerPrefs.SetInt("Coin", coin);
            comboIndex++;
            if (comboIndex >= ComboCoin.Length)
            {
                comboMoneyText.text = "Maxed Out";
                ComboUpgrade.GetComponentInParent<Button>().enabled = false;
            }
            AdjustTexts();
            AdjustButtons();
            AdjustCompletedUpgrades();
        }
        else
        {
            //AddAnimation
            Debug.Log("Not Enough Money");
        }
    }
    public void MaxSpeed()
    {
        if (coin >= SpeedCoin[speedIndex])
        {
            speed++;
            PlayerPrefs.SetInt("MaxSpeed", speed);
            SpeedImage[speedIndex].color = new Color32(255, 255, 255, 255);
            coin -= SpeedCoin[speedIndex];
            PlayerPrefs.SetInt("Coin", coin);
            speedIndex++;
            if (speedIndex >= SpeedCoin.Length)
            {
                speedMoneyText.text = "Maxed Out";
                SpeedUpgrade.GetComponentInParent<Button>().enabled = false;
            }
            AdjustTexts();
            AdjustButtons();
            AdjustCompletedUpgrades();
        }
        else
        {
            //AddAnimation
            Debug.Log("Not Enough Money");
        }
    }
    public void MaxSlowTime()
    {
        if (coin >= SlowTimeCoin[slowTimeIndex])
        {
            slowTime += 0.1f;
            PlayerPrefs.SetFloat("MaxSlowTime", slowTime);
            SlowTimeImage[slowTimeIndex].color = new Color32(255, 255, 255, 255);
            coin -= SlowTimeCoin[slowTimeIndex];
            PlayerPrefs.SetInt("Coin", coin);
            slowTimeIndex++;
            if (slowTimeIndex >= SlowTimeCoin.Length)
            {
                slowTimeMoneyText.text = "Maxed Out";
                SlowTimeUpgrade.GetComponentInParent<Button>().enabled = false;
            }
            PlayerPrefs.SetInt("slowTimeIndex",slowTimeIndex);
            AdjustTexts();
            AdjustButtons();
            AdjustCompletedUpgrades();
        }
        else
        {
            //AddAnimation
            Debug.Log("Not Enough Money");
        }
    }
    public void SlingShot()
    {
        if (coin >= SlingShotCoin[slingShotIndex])
        {
            slingShotPower += 10;
            PlayerPrefs.SetInt("slingShotPower", slingShotPower);
            SlingShotImage[slingShotIndex].color = new Color32(255, 255, 255, 255);
            coin -= SlingShotCoin[slingShotIndex];
            PlayerPrefs.SetInt("Coin", coin);
            slingShotIndex++;
            if (slingShotIndex >= SlingShotCoin.Length)
            {
                slingShotMoneyText.text = "Maxed Out";
                SlingShotUpgrade.GetComponentInParent<Button>().enabled = false;
            }
            PlayerPrefs.SetInt("slingShotIndex", slingShotIndex);
            AdjustTexts();
            AdjustButtons();
            AdjustCompletedUpgrades();
        }
        else
        {
            //AddAnimation
            Debug.Log("Not Enough Money");
        }
    }
    public void DoublePoint()
    {
        if (coin >= DoublePointCoin[DoublePointIndex])
        {
            DoublePointTimer += 2;
            PlayerPrefs.SetFloat("doublePointTimer", DoublePointTimer);
            DoublePointImage[DoublePointIndex].color = new Color32(255, 255, 255, 255);
            coin -= DoublePointCoin[DoublePointIndex];
            PlayerPrefs.SetInt("Coin", coin);
            DoublePointIndex++;
            if (DoublePointIndex >= DoublePointCoin.Length)
            {
                doublePointMoneyText.text = "Maxed Out";
                DoublePointUpgrade.GetComponentInParent<Button>().enabled = false;
            }
            PlayerPrefs.SetInt("doublePointIndex", DoublePointIndex);
            AdjustTexts();
            AdjustButtons();
            AdjustCompletedUpgrades();
        }
        else
        {
            //AddAnimation
            Debug.Log("Not Enough Money");
        }
    }
    public void Bomber()
    {
        if (coin >= BomberCoin[BomberIndex])
        {
            BomberTimer += 2;
            PlayerPrefs.SetFloat("bomberTimer", BomberTimer);
            BomberImage[BomberIndex].color = new Color32(255, 255, 255, 255);
            coin -= BomberCoin[BomberIndex];
            PlayerPrefs.SetInt("Coin", coin);
            BomberIndex++;
            if (BomberIndex >= BomberCoin.Length)
            {
                BomberText.text = "Maxed Out";
                BomberUpgrade.GetComponentInParent<Button>().enabled = false;
            }
            PlayerPrefs.SetInt("bomberIndex", BomberIndex);
            AdjustTexts();
            AdjustButtons();
            AdjustCompletedUpgrades();
        }
        else
        {
            //AddAnimation
            Debug.Log("Not Enough Money");
        }
    }
    private void AdjustTexts()
    {
        if (jumpCountIndex < JumpCoin.Length)
            jumpMoneyText.text = JumpCoin[jumpCountIndex].ToString() + "$";

        if (comboIndex < ComboCoin.Length)
            comboMoneyText.text = ComboCoin[comboIndex].ToString() + "$";

        if (speedIndex < SpeedCoin.Length)
            speedMoneyText.text = SpeedCoin[speedIndex].ToString() + "$";

        if (slowTimeIndex < SlowTimeCoin.Length)
            slowTimeMoneyText.text = SlowTimeCoin[slowTimeIndex].ToString() + "$";

        if (slingShotIndex < SlingShotCoin.Length)
            slingShotMoneyText.text = SlingShotCoin[slingShotIndex].ToString() + "$";

        if (DoublePointIndex < DoublePointCoin.Length)
            doublePointMoneyText.text = DoublePointCoin[DoublePointIndex].ToString() + "$";

        if (BomberIndex < BomberCoin.Length)
            BomberText.text = BomberCoin[BomberIndex].ToString() + "$";

        coinText.text = coin.ToString() + "$";
    }
    private void AdjustButtons()
    {
        if (jumpCountIndex >= JumpCoin.Length)
        {
            jumpMoneyText.text = "Maxed Out";
            buttons[0].enabled = false;
        }
        if (comboIndex >= ComboCoin.Length)
        {
            comboMoneyText.text = "Maxed Out";
            buttons[1].enabled = false;
        }
        if (speedIndex >= SpeedCoin.Length)
        {
            speedMoneyText.text = "Maxed Out";
            buttons[2].enabled = false;
        }
        if (slowTimeIndex >= SlowTimeCoin.Length)
        {
            slowTimeMoneyText.text = "Maxed Out";
            buttons[3].enabled = false;
        }
        if (slingShotIndex >= SlingShotCoin.Length)
        {
            slingShotMoneyText.text = "Maxed Out";
            buttons[4].enabled = false;
        }
        if (DoublePointIndex >= DoublePointCoin.Length)
        {
            doublePointMoneyText.text = "Maxed Out";
            buttons[5].enabled = false;
        }
        if (BomberIndex >= BomberCoin.Length)
        {
            BomberText.text = "Maxed Out";
            buttons[6].enabled = false;
        }
    }
    private void AdjustCompletedUpgrades()
    {
        int i;
        for(i = 0; i < jumpCountIndex; i++)
        {
            jumpCountImage[i].color = new Color32(255, 255, 255, 255);
        }
        for (i = 0; i < comboIndex; i++)
        {
            comboImage[i].color = new Color32(255, 255, 255, 255);
        }
        for (i = 0; i < speedIndex; i++)
        {
            SpeedImage[i].color = new Color32(255, 255, 255, 255);
        }
        for (i = 0; i < slowTimeIndex; i++)
        {
            SlowTimeImage[i].color = new Color32(255, 255, 255, 255);
        }
        for (i = 0; i < slingShotIndex; i++)
        {
            SlingShotImage[i].color = new Color32(255, 255, 255, 255);
        }
        for (i = 0; i < DoublePointIndex; i++)
        {
            DoublePointImage[i].color = new Color32(255, 255, 255, 255);
        }
        for (i = 0; i < BomberIndex; i++)
        {
            BomberImage[i].color = new Color32(255, 255, 255, 255);
        }

    }

}
