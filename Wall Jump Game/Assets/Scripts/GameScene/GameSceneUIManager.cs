using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameSceneUIManager : MonoBehaviour
{
    public static GameSceneUIManager instance;

    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject EnemyKilledTextParentObject;
    [SerializeField] private GameObject BarePassTextParentObject;
    [SerializeField] private Animator coinTextAnimator;
    [SerializeField] private RectTransform coinTextAnimatorPosition;
    [SerializeField] private GameObject deathScreen;

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


    private Coroutine EnemyKilledTextCoroutine;
    private Coroutine BarePassTextCoroutine;
    public void Init()
    {
        instance = this;
        UpdateJumpCountText();
        ToggleBomberText(false);
        ToggleDoublePointText(false);
        
        EnemyKilledTextParentObject.SetActive(false);
        BarePassTextParentObject.SetActive(false);


        PlayerEventHandler.instance.OnPlayerJump += UpdateJumpCountText;
        PlayerEventHandler.instance.OnPlayerDied += DeathScreen;
    }

    private void Update()
    {
        UpdatePointText();
    }

    private void OnDestroy()
    {
        PlayerEventHandler.instance.OnPlayerJump -= UpdateJumpCountText;
        PlayerEventHandler.instance.OnPlayerDied -= DeathScreen;
    }

    private void DeathScreen()
    {
        int gainedCoin = PlayerManager.instance.Point / 5;
        deathScreen.SetActive(true);
        StartCoroutine(AnimateGoldGainOnEnd(gainedCoin));
    }

    private IEnumerator AnimateGoldGainOnEnd(int amount)
    {

        int startGold = PlayerManager.instance.playerData.Coin;
        int targetGold = startGold + amount;
        float duration = 3f;
        float elapsed = 0f;

        GainedCoinText.gameObject.SetActive(true);
        GainedCoinText.text = $"+0"; // Baþlangýçta 0'dan baþlayacak

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            int currentGold = (int)Mathf.Lerp(startGold, targetGold, t);
            int gainedGold = currentGold - startGold;

            DeathCoinText.text = currentGold.ToString(); // Ana altýn göstergesini güncelle
            GainedCoinText.text = $"+{gainedGold}";

            yield return null;
        }

        PlayerManager.instance.playerData.Coin = targetGold; // Son deðeri sabitle
        DeathCoinText.text = PlayerManager.instance.playerData.Coin.ToString();
        GainedCoinText.text = $"+{amount}";

        yield return new WaitForSeconds(1f); // Biraz bekle, sonra kaybolsun
        GainedCoinText.gameObject.SetActive(false);
    }


    public void CoinGainText()
    {
        Vector2 pos = coinTextAnimatorPosition.position + new Vector3(Random.Range(-100, 100), Random.Range(-100, 100));
        var obj = Instantiate(coinTextAnimator.gameObject, pos,Quaternion.identity,canvas.transform);
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
