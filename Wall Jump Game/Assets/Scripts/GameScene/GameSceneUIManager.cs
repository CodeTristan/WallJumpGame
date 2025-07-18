using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameSceneUIManager : MonoBehaviour
{
    public static GameSceneUIManager instance;

    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject EnemyKilledTextParentObject;
    [SerializeField] private GameObject BarePassTextParentObject;
    [SerializeField] private Animator gainedCoinTextAnimator;
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Volume volume;

    [Header("Gem and Coin Gain")]
    [SerializeField] private Animator coinTextAnimator;
    [SerializeField] private Animator gemTextAnimator;
    [SerializeField] private RectTransform coinTextAnimatorPosition;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI pointText;
    [SerializeField] private TextMeshProUGUI jumpCountText;
    [SerializeField] private TextMeshProUGUI BarePassXCount;
    [SerializeField] private TextMeshProUGUI BarePassXCountTimer;
    [SerializeField] private TextMeshProUGUI BarePassPointText;
    [SerializeField] private TextMeshProUGUI KillCountX;
    [SerializeField] private TextMeshProUGUI KillCountXTimer;
    [SerializeField] private TextMeshProUGUI KillPointText;
    [SerializeField] private TextMeshProUGUI DeathMaxPointText;
    [SerializeField] private TextMeshProUGUI DeathPointText;
    [SerializeField] private TextMeshProUGUI DeathCoinText;
    [SerializeField] private TextMeshProUGUI GainedCoinText;
    [SerializeField] private TextMeshProUGUI doublePointTimerText;
    [SerializeField] private TextMeshProUGUI bomberTimerText;

    [Header("DeathAdScreen")]
    [SerializeField] private GameObject DeathAdScreen;
    [SerializeField] private GameObject DeathAd_NotEnoughDiamonds;
    [SerializeField] private GameObject DeathAd_AdNotReady;
    [SerializeField] private CircularImageTimer DeathAdScreenTimerImage;
    [SerializeField] private Button DeathAdScreenWatchAdButton;
    [SerializeField] private Button DeathAdScreenPayDiamondButton;
    [SerializeField] private Button DeathAdScreenCloseButton;
    [SerializeField] private Button DeathDoubleMoneyWatchAdButton;

    [Header("StartScreen")]
    [SerializeField] private Canvas StartCanvas;


    private Coroutine EnemyKilledTextCoroutine;
    private Coroutine BarePassTextCoroutine;

    private bool inDeathScreen = true;
    private Vector2 startTouchPosition;
    public void Init()
    {
        instance = this;
        UpdateJumpCountText();
        ToggleBomberText(false);
        ToggleDoublePointText(false);
        deathScreen.SetActive(false);
        DeathAdScreen.SetActive(false);
        
        EnemyKilledTextParentObject.SetActive(false);
        BarePassTextParentObject.SetActive(false);

        DeathAdScreenWatchAdButton.onClick.AddListener(() => { _WatchAdToRevive(); });
        DeathAdScreenPayDiamondButton.onClick.AddListener(() => { _PayDiamondsToRevive(); });
        DeathAdScreenCloseButton.onClick.AddListener(() => { ToggleDeathAdScreen(false); GameSceneEventHandler.instance.PlayerDiedFR(); });
        DeathDoubleMoneyWatchAdButton.onClick.AddListener(() => { _DoubleMoneyWatchAd(); });

        StartCanvas.enabled = true;
        canvas.enabled = false;
        volume.enabled = true;
    }

    private void Update()
    {
        if (AdManager.instance.InAdMenu)
            return;

        UpdatePointText();

        if(inDeathScreen && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            else if (Vector2.Distance(touch.position,startTouchPosition) > 60 && (touch.phase == TouchPhase.Moved ||touch.phase == TouchPhase.Stationary))
            {
                GameSceneManager.instance.StartGame();
            }
        }
    }

    public void ToggleCanvas(bool toggle)
    {
        canvas.enabled = toggle;
    }
    public void ToggleStartCanvas(bool toggle)
    {
        StartCanvas.enabled = toggle;
    }

    public void ToggleVolume(bool toggle)
    {
        if (volume != null)
        {
            volume.enabled = toggle;
        }
    }

    public void Restart()
    {
        inDeathScreen = false;
        volume.enabled = false;
        StartCanvas.enabled = false;
        canvas.enabled = true;
        deathScreen.SetActive(false);
    }
    public void DeathScreen()
    {
        AdManager.instance.ShowBannerAd();

        volume.enabled = true;
        inDeathScreen = true;
        PlayerManager.instance.isDead = true;

        int gainedCoin = PlayerManager.instance.Point / 5;
        deathScreen.SetActive(true);
        DeathMaxPointText.text = PlayerManager.instance.playerData.MaxPoint.ToString();
        DeathPointText.text = PlayerManager.instance.Point.ToString();
        DeathDoubleMoneyWatchAdButton.interactable = AdManager.instance.IsRewardedAdReady();

        SaveSystem.instance.SaveData();
        GameSceneManager.instance.Restart();

        StartCoroutine(AnimateGoldGainOnEnd(gainedCoin));
    }

    private IEnumerator AnimateGoldGainOnEnd(int amount)
    {
        int startGold = PlayerManager.instance.playerData.Coins - amount;
        if (startGold < 0)
            startGold = 0;

        int targetGold = startGold + amount;

        float duration = 3f;
        if (amount < 100)
            duration = 1;

        float elapsed = 0f;

        GainedCoinText.color = new Color(GainedCoinText.color.r, GainedCoinText.color.g, GainedCoinText.color.b, 255);
        GainedCoinText.gameObject.SetActive(true);
        gainedCoinTextAnimator.SetTrigger("Dead");
        GainedCoinText.text = $"+0"; // Ba�lang��ta 0'dan ba�layacak

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            int currentGold = (int)Mathf.Lerp(startGold, targetGold, t);
            int gainedGold = currentGold - startGold;
            GainedCoinText.transform.localScale += new Vector3(0.01f,0.01f,0.01f);

            DeathCoinText.text = currentGold.ToString(); // Ana alt�n g�stergesini g�ncelle
            GainedCoinText.text = $"+{gainedGold}";

            yield return null;
        }

        DeathCoinText.text = PlayerManager.instance.playerData.Coins.ToString();
        GainedCoinText.text = $"+{amount}";

        gainedCoinTextAnimator.SetTrigger("Dead");

        elapsed = 0;
        Color oldColor = GainedCoinText.color;
        Color targetColor = new Color(oldColor.r,oldColor.g,oldColor.b,0);
        while(elapsed < 1.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 1.5f;
            GainedCoinText.color = Color32.Lerp(oldColor, targetColor, t);
            yield return null;
        }
        GainedCoinText.gameObject.SetActive(false);
        GainedCoinText.transform.localScale = Vector3.one;

    }

    public void ToggleDeathAdScreen(bool toggle)
    {
        if(toggle)
        {
            StopCoroutines();
            AdManager.instance.InAdMenu = true;
            DeathAdScreen.SetActive(true);
            DeathAd_NotEnoughDiamonds.SetActive(false);
            DeathAd_AdNotReady.SetActive(false);
            DeathAdScreenWatchAdButton.interactable = AdManager.instance.IsRewardedAdReady();

            CircularImageTimer.OnTimerEndDelegate func = CloseDeathAdScreen;
            CircularImageTimer.OnTimerEndDelegate func2 = GameSceneEventHandler.instance.PlayerDiedFR;
            DeathAdScreenTimerImage.StartTimer(5,new List<CircularImageTimer.OnTimerEndDelegate>() { func, func2 });
        }
        else
        {
            CloseDeathAdScreen();
            DeathAdScreenTimerImage.StopTimer();
        }
    }


    private void CloseDeathAdScreen()
    {
        AdManager.instance.InAdMenu = false;
        DeathAdScreen.SetActive(false);
    }
    

    private void _WatchAdToRevive()
    {
        AdManager.ShowRewardedAdFailDelegate function = ReviveAdError;
        AdManager.instance.ShowRewardedAd("Respawn",new List<AdManager.ShowRewardedAdFailDelegate>() { function });
    }

    private void _DoubleMoneyWatchAd()
    {
        AdManager.instance.ShowRewardedAd("DoubleMoney", new List<AdManager.ShowRewardedAdFailDelegate>() {  });
    }

    private void ReviveAdError()
    {
        DeathAd_AdNotReady.SetActive(true);
    }

    private void _PayDiamondsToRevive()
    {
        if(PlayerManager.instance.playerData.Gems >= 10)
        {
            PlayerManager.instance.playerData.Gems -= 10;
            AdManager.instance.InAdMenu = false;
            DeathAdScreen.SetActive(false);
            PlayerManager.instance.Respawn();
        }
        else
        {
            DeathAd_NotEnoughDiamonds.SetActive(true);
        }
    }


    public void CoinGainText()
    {
        Vector2 pos = coinTextAnimatorPosition.position + new Vector3(Random.Range(-100, 100), Random.Range(-100, 100));
        var obj = Instantiate(coinTextAnimator.gameObject, pos,Quaternion.identity,canvas.transform);
        Destroy(obj.gameObject, 1f);
    }

    public void GemGainText()
    {
        Vector2 pos = coinTextAnimatorPosition.position + new Vector3(Random.Range(-100, 100), Random.Range(-100, 100));
        var obj = Instantiate(gemTextAnimator.gameObject, pos, Quaternion.identity, canvas.transform);
        Destroy(obj.gameObject, 1f);
    }

    public void ToggleDoublePointText(bool value)
    {
        doublePointTimerText.gameObject.SetActive(value);
    }

    public void ToggleBomberText(bool value)
    {
        bomberTimerText.gameObject.SetActive(value);
    }

    public void UpdateBomberText(float currentBomberTimer)
    {
        bomberTimerText.text = currentBomberTimer.ToString("F1");
    }
    public void UpdateDoublePointText(float currentDoublePointTimer)
    {
        doublePointTimerText.text = currentDoublePointTimer.ToString("F1");
    }
    public void UpdatePointText()
    {
        pointText.text = PlayerManager.instance.Point.ToString();
    }

    public void UpdateJumpCountText()
    {
        jumpCountText.text = PlayerManager.instance.playerMovement.currentJumpCount.ToString();
    }

    public void EnemyKilledToText(int KillCountExponent, int PointExponent)
    {
        EnemyKilledTextCoroutine = StartCoroutine(EnemyKilledToTextCoroutine(KillCountExponent, PointExponent));
    }

    public void BarePassToText(int BarePassExponent, int PointExponent)
    {
        BarePassTextCoroutine = StartCoroutine(BarePassToTextCoroutine(BarePassExponent,PointExponent));
    }

    private void StopCoroutines()
    {
        StopAllCoroutines();
        BarePassTextParentObject.gameObject.SetActive(false);
        EnemyKilledTextParentObject.gameObject.SetActive(false);
    }

    public IEnumerator BarePassToTextCoroutine(int BarePassExponent, int PointExponent)
    {
        if(BarePassTextCoroutine != null)
        {
            StopCoroutine(BarePassTextCoroutine);
        }
        BarePassTextParentObject.SetActive(true);

        BarePassPointText.text = "+" + (15 * BarePassExponent * PointExponent) + "X";
        BarePassXCount.text = BarePassExponent.ToString() + "X";

        float timer = PlayerManager.instance.BarePassCounterTimer;
        while (timer > 0)
        {
            BarePassXCountTimer.text = timer.ToString("F1");
            timer -= Time.deltaTime;
            yield return null;
        }

        BarePassTextParentObject.SetActive(false);
        PlayerManager.instance.playerCollisionHandler.BarePassExponent = 0;
    }
    public IEnumerator EnemyKilledToTextCoroutine(int KillCountExponent, int PointExponent)
    {
        if(EnemyKilledTextCoroutine != null)
        {
            StopCoroutine(EnemyKilledTextCoroutine);
        }
        EnemyKilledTextParentObject.SetActive(true);

        KillPointText.text = "+" + (15 *  KillCountExponent * PointExponent).ToString();
        KillCountX.text = KillCountExponent.ToString() + "X";

        float timer = PlayerManager.instance.EnemyKillCounterTimer;
        while (timer > 0)
        {
            KillCountXTimer.text = timer.ToString("F1");
            timer -= Time.deltaTime;
            yield return null;
        }

        EnemyKilledTextParentObject.SetActive(false);
        PlayerManager.instance.playerCollisionHandler.KillCountExponent = 0;
    }

}
