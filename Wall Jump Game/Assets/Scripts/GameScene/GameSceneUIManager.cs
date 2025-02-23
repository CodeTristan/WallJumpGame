using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameSceneUIManager : MonoBehaviour
{
    public static GameSceneUIManager instance;

    [SerializeField] private GameObject EnemyKilledTextParentObject;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI pointText;
    [SerializeField] private TextMeshProUGUI jumpCountText;
    [SerializeField] private TextMeshProUGUI BarePassXCount;
    [SerializeField] private TextMeshProUGUI BarePassXCountTimer;
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
    public void Init()
    {
        instance = this;
        UpdateJumpCountText();
        ToggleBomberText(false);
        ToggleDoublePointText(false);

        PlayerEventHandler.OnPlayerJump += UpdateJumpCountText;
    }

    private void Update()
    {
        UpdatePointText();
    }

    private void OnDestroy()
    {
        PlayerEventHandler.OnPlayerJump -= UpdateJumpCountText;

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
    }

}
