using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CircularImageTimer : MonoBehaviour
{
    public delegate void OnTimerEndDelegate();
    public event OnTimerEndDelegate OnTimerEnd;

    public Image TimerImage;
    public TextMeshProUGUI TimerText;

    private List<OnTimerEndDelegate> tempOnTimerEndFunctions;

    public void StartTimer(float duration,List<OnTimerEndDelegate> OnTimerEndFunctions)
    {
        gameObject.SetActive(true);
        tempOnTimerEndFunctions = OnTimerEndFunctions;
        foreach (var func in tempOnTimerEndFunctions)
        {
            OnTimerEnd += func;
        }
        StartCoroutine(TimerStart(duration));

    }

    private void OnDisable()
    {
        if (tempOnTimerEndFunctions == null)
            return;

        foreach (var func in tempOnTimerEndFunctions)
        {
            OnTimerEnd -= func;
        }
    }


    public void StopTimer()
    {
        StopAllCoroutines();
        TimerImage.gameObject.SetActive(false);
        TimerText.gameObject.SetActive(false);
        foreach (var func in tempOnTimerEndFunctions)
        {
            OnTimerEnd -= func;
        }
    }
    private IEnumerator TimerStart(float duration)
    {
        TimerImage.gameObject.SetActive(true);
        TimerText.gameObject.SetActive(true);

        TimerImage.fillAmount = 0;
        TimerText.text = duration.ToString();

        float amount = 0;
        while (amount <= duration-0.01f)
        {
            amount += Time.deltaTime;
            float fillamount = Mathf.Clamp01(amount / duration);
            TimerImage.fillAmount = fillamount;
            TimerText.text = Mathf.FloorToInt(duration+1 - amount).ToString();

            yield return null;
        }
        OnTimerEnd?.Invoke();

        TimerText.gameObject.SetActive(false);
        gameObject.SetActive(false);


    }
}
