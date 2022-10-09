using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    public int coin;
    public TextMeshProUGUI coinText;

    [Header("MaxJump")]
    public GameObject JumpUpgrade;
    public int[] JumpCoin;
    public TextMeshProUGUI jumpMoneyText;
    public Image[] jumpCountImage;
    private int jumpCountIndex = 2;
    private int maxJump;

    [Header("MaxCombo")]
    public GameObject ComboUpgrade;
    public int[] ComboCoin;
    public TextMeshProUGUI comboMoneyText;
    public Image[] comboImage;
    private int comboIndex;
    private int maxCombo;

    [Header("Speed")]
    public GameObject SpeedUpgrade;
    public int[] SpeedCoin;
    public TextMeshProUGUI speedMoneyText;
    public Image[] SpeedImage;
    private int speedIndex = 2;
    private int speed;

    [Header("SlowTime")]
    public GameObject SlowTimeUpgrade;
    public int[] SlowTimeCoin;
    public TextMeshProUGUI slowTimeMoneyText;
    public Image[] SlowTimeImage;
    private int slowTimeIndex = 2;
    private float slowTime;

    private void Start()
    {
        if (PlayerPrefs.GetInt("MaxCombo") == 0)
            PlayerPrefs.SetInt("MaxCombo", 2);
        if (PlayerPrefs.GetInt("MaxJump") == 0)
            PlayerPrefs.SetInt("MaxJump", 2);
        if (PlayerPrefs.GetInt("MaxSpeed") == 0)
            PlayerPrefs.SetInt("MaxSpeed", 11);


        jumpCountIndex = PlayerPrefs.GetInt("MaxJump");
        coin = PlayerPrefs.GetInt("Coin");
        comboIndex = PlayerPrefs.GetInt("MaxCombo");
        speedIndex = PlayerPrefs.GetInt("MaxSpeed");
        slowTimeIndex = PlayerPrefs.GetInt("slowTimeIndex");

        maxJump = jumpCountIndex;
        maxCombo = comboIndex;
        speed = speedIndex;
        slowTime = PlayerPrefs.GetFloat("MaxSlowTime"); ;

        jumpCountIndex -= 2;
        comboIndex -= 2;
        speedIndex -= 11;

        AdjustTexts();

        AdjustButtons();

        AdjustCompletedUpgrades();

    }
    public void JumpCount()
    {
        if(coin >= JumpCoin[jumpCountIndex])
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

        coinText.text = coin.ToString() + "$";
    }
    private void AdjustButtons()
    {
        if (jumpCountIndex >= JumpCoin.Length)
        {
            jumpMoneyText.text = "Maxed Out";
            JumpUpgrade.GetComponentInParent<Button>().enabled = false;
        }
        if (comboIndex >= ComboCoin.Length)
        {
            comboMoneyText.text = "Maxed Out";
            ComboUpgrade.GetComponentInParent<Button>().enabled = false;
        }
        if (speedIndex >= SpeedCoin.Length)
        {
            speedMoneyText.text = "Maxed Out";
            SpeedUpgrade.GetComponentInParent<Button>().enabled = false;
        }
        if (slowTimeIndex >= SlowTimeCoin.Length)
        {
            slowTimeMoneyText.text = "Maxed Out";
            SlowTimeUpgrade.GetComponentInParent<Button>().enabled = false;
        }
    }
    private void AdjustCompletedUpgrades()
    {
        for(int i = 0; i < jumpCountIndex; i++)
        {
            jumpCountImage[i].color = new Color32(255, 255, 255, 255);
        }
        for (int i = 0; i < comboIndex; i++)
        {
            comboImage[i].color = new Color32(255, 255, 255, 255);
        }
        for (int i = 0; i < speedIndex; i++)
        {
            SpeedImage[i].color = new Color32(255, 255, 255, 255);
        }
        for (int i = 0; i < slowTimeIndex; i++)
        {
            SlowTimeImage[i].color = new Color32(255, 255, 255, 255);
        }
    }

}
