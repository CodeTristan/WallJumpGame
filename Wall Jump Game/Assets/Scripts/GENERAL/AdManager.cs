using System.Collections.Generic;
using TMPro;
using Unity.Services.LevelPlay;
using UnityEngine;


public class AdManager : MonoBehaviour
{
    public static AdManager instance;

#if UNITY_ANDROID
    string appKey = "212556f5d";
#elif UNITY_IOS
    string appKey = "";
#else
    string appKey = "unexpected_platform";
#endif

    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private Canvas debugCanvas;

    public const string BANNER_AD_ID = "dwrrnzaju0z9gy4f";
    public const string INTERSTITIAL_AD_ID = "ur2jrunhpqnqt7tn";
    public const string REWARDED_AD_ID = "rnkq0zu8ryjzylzb";
    public const string NATIVE_AD_ID = "kt4l55oylbdqwqw6";

    public delegate void ShowRewardedAdFailDelegate();



    private bool inited = false;
    public bool DEBUG_MODE = false;
    public bool InAdMenu;


    private bool RewardAdReady;

    private LevelPlayBannerAd bannerAd;
    private LevelPlayInterstitialAd interstitialAd;
    private LevelPlayRewardedAd rewardedVideoAd;

    public void Init()
    {
        instance = this;
        debugText.gameObject.SetActive(DEBUG_MODE);
        debugCanvas.enabled = DEBUG_MODE;


#if UNITY_EDITOR || DEVELOPMENT_BUILD
        LevelPlay.ValidateIntegration();

#endif


        LevelPlay.OnInitSuccess += SDKInitialized;
        LevelPlay.OnInitFailed += LevelPlay_OnInitFailed;

        LevelPlay.Init(appKey, GetUserId());

    }

    public void OnPLayerDiedFR()
    {
        if(GameManager.instance.current_Game_Before_Ad == 0)
        {
            GameManager.instance.current_Game_Before_Ad = GameManager.instance.Number_Of_Game_Before_Ad;
            interstitialAd.ShowAd("Game_Over");
        }
        else
        {
            GameManager.instance.current_Game_Before_Ad--;
        }
    }

    void EnableAds()
    {
        // Register to ImpressionDataReadyEvent
        LevelPlay.OnImpressionDataReady += ImpressionDataReadyEvent;

        // Create Rewarded Video object
        rewardedVideoAd = new LevelPlayRewardedAd(REWARDED_AD_ID);

        // Register to Rewarded Video events
        rewardedVideoAd.OnAdLoaded += RewardedVideoOnLoadedEvent;
        rewardedVideoAd.OnAdLoadFailed += RewardedVideoOnAdLoadFailedEvent;
        rewardedVideoAd.OnAdDisplayed += RewardedVideoOnAdDisplayedEvent;
        rewardedVideoAd.OnAdDisplayFailed += RewardedVideoOnAdDisplayedFailedEvent;
        rewardedVideoAd.OnAdRewarded += RewardedVideoOnAdRewardedEvent;
        rewardedVideoAd.OnAdClicked += RewardedVideoOnAdClickedEvent;
        rewardedVideoAd.OnAdClosed += RewardedVideoOnAdClosedEvent;
        rewardedVideoAd.OnAdInfoChanged += RewardedVideoOnAdInfoChangedEvent;

        // Create Banner object
        bannerAd = new LevelPlayBannerAd(BANNER_AD_ID);

        // Register to Banner events
        bannerAd.OnAdLoaded += BannerOnAdLoadedEvent;
        bannerAd.OnAdLoadFailed += BannerOnAdLoadFailedEvent;
        bannerAd.OnAdDisplayed += BannerOnAdDisplayedEvent;
        bannerAd.OnAdDisplayFailed += BannerOnAdDisplayFailedEvent;
        bannerAd.OnAdClicked += BannerOnAdClickedEvent;
        bannerAd.OnAdCollapsed += BannerOnAdCollapsedEvent;
        bannerAd.OnAdLeftApplication += BannerOnAdLeftApplicationEvent;
        bannerAd.OnAdExpanded += BannerOnAdExpandedEvent;

        // Create Interstitial object
        interstitialAd = new LevelPlayInterstitialAd(INTERSTITIAL_AD_ID);

        // Register to Interstitial events
        interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
        interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
        interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
        interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
        interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
        interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
        interstitialAd.OnAdInfoChanged += InterstitialOnAdInfoChangedEvent;

        // Load the first ads
        rewardedVideoAd.LoadAd();
        bannerAd.LoadAd();
        interstitialAd.LoadAd();

    }

    private void LevelPlay_OnInitFailed(LevelPlayInitError error)
    {
        Debug.LogError("LevelPlay Initialization Failed: " + error.ToString());
        debugText.text += "Init Failed: " + error.ToString();
    }

    private void OnApplicationPause(bool pause)
    {
        LevelPlay.SetPauseGame(pause);
    }

    private void SdkInitializationCompletedEvent()
    {
        if(DEBUG_MODE) LevelPlay.LaunchTestSuite();

        inited = true;
        EnableAds();

        if (SahneManager.instance.currentSceneEnum == SceneEnum.GameScene)
            bannerAd.HideAd();
    }

    private void SDKInitialized(LevelPlayConfiguration levelPlayConfiguration)
    {
        Debug.Log("SDKInitialized");
        debugText.text += " SDK Initialized... ";
        SdkInitializationCompletedEvent();
    }

    private string GetUserId()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }

    #region RewardedAd

    public void ShowRewardedAd(string placementName,List<ShowRewardedAdFailDelegate> functionsWhenFail)
    {
        debugText.text += " " + rewardedVideoAd.IsAdReady() + " " + LevelPlayRewardedAd.IsPlacementCapped(placementName) + " ";
            
        if(rewardedVideoAd.IsAdReady() || RewardAdReady)
        {
            rewardedVideoAd.ShowAd(placementName);
        }
        else
        {
            debugText.text += " AD CANT SHOW ";
            foreach (var item in functionsWhenFail)
            {
                item.Invoke();
            }
        }
    }

    public bool IsRewardedAdReady()
    {
        return rewardedVideoAd.IsAdReady() || RewardAdReady;
    }

    void RewardedVideoOnLoadedEvent(LevelPlayAdInfo adInfo)
    {
        debugText.text += " " + $"[LevelPlaySample] Received RewardedVideoOnLoadedEvent With AdInfo: {adInfo}";
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnLoadedEvent With AdInfo: {adInfo}");
        RewardAdReady = true;
    }

    void RewardedVideoOnAdLoadFailedEvent(LevelPlayAdError error)
    {
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdLoadFailedEvent With Error: {error}");
        debugText.text += " " + $"[LevelPlaySample] Received RewardedVideoOnAdLoadFailedEvent With Error: {error}";
        RewardAdReady = false;
    }

    void RewardedVideoOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    {
        debugText.text += " " + $"[LevelPlaySample] Received RewardedVideoOnAdDisplayedEvent With AdInfo: {adInfo}";
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdDisplayedEvent With AdInfo: {adInfo}");
    }
#pragma warning disable 0618
    void RewardedVideoOnAdDisplayedFailedEvent(LevelPlayAdDisplayInfoError error)
    {
        debugText.text += " " + $"[LevelPlaySample] Received RewardedVideoOnAdDisplayedFailedEvent With Error: {error}";
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdDisplayedFailedEvent With Error: {error}");
    }
#pragma warning restore 0618

    //REWARD
    void RewardedVideoOnAdRewardedEvent(LevelPlayAdInfo adInfo, LevelPlayReward reward)
    {
        //TODO - here you can reward the user according to the reward name and amount

        debugText.text = "Reward: " + reward.Name + "\nAmount: " + reward.Amount;

        if (adInfo.PlacementName == "Respawn")
        {
            InAdMenu = false;
            GameSceneUIManager.instance.ToggleDeathAdScreen(false);
            PlayerManager.instance.Respawn();
        }
        if (adInfo.PlacementName == "DoubleMoney")
        {
            InAdMenu = false;
            GameSceneUIManager.instance.DeathScreen();
        }

    }

    void RewardedVideoOnAdClickedEvent(LevelPlayAdInfo adInfo)
    {
        InAdMenu = true;
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdClickedEvent With AdInfo: {adInfo}");
    }

    void RewardedVideoOnAdClosedEvent(LevelPlayAdInfo adInfo)
    {
        InAdMenu = false;
        rewardedVideoAd.LoadAd(); // Load the next ad after rewarding
        debugText.text += " " + $"[LevelPlaySample] Received RewardedVideoOnAdClosedEvent With AdInfo: {adInfo}";
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdClosedEvent With AdInfo: {adInfo}");
    }

    void RewardedVideoOnAdInfoChangedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdInfoChangedEvent With AdInfo {adInfo}");
    }


    #endregion

    #region AdInfo Interstitial


    void InterstitialOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received InterstitialOnAdLoadedEvent With AdInfo: {adInfo}");
    }

    void InterstitialOnAdLoadFailedEvent(LevelPlayAdError error)
    {
        Debug.Log($"[LevelPlaySample] Received InterstitialOnAdLoadFailedEvent With Error: {error}");
    }

    void InterstitialOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received InterstitialOnAdDisplayedEvent With AdInfo: {adInfo}");
    }
#pragma warning disable 0618
    void InterstitialOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError infoError)
    {
        Debug.Log($"[LevelPlaySample] Received InterstitialOnAdDisplayFailedEvent With InfoError: {infoError}");
    }
#pragma warning restore 0618
    void InterstitialOnAdClickedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received InterstitialOnAdClickedEvent With AdInfo: {adInfo}");
    }

    void InterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received InterstitialOnAdClosedEvent With AdInfo: {adInfo}");
    }

    void InterstitialOnAdInfoChangedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received InterstitialOnAdInfoChangedEvent With AdInfo: {adInfo}");
    }

    #endregion

    #region Banner AdInfo

    public void ShowBannerAd()
    {
        if (bannerAd != null && inited)
        {
            bannerAd.ShowAd();
            Debug.Log("[LevelPlaySample] Banner Ad Shown");
        }
        else
        {
            Debug.LogWarning("[LevelPlaySample] Banner Ad not ready or SDK not initialized.");
        }
    }

    public void HideBannerAd()
    {
        if (bannerAd != null && inited)
        {
            bannerAd.HideAd();
            Debug.Log("[LevelPlaySample] Banner Ad Hidden");
        }
        else
        {
            Debug.LogWarning("[LevelPlaySample] Banner Ad not ready or SDK not initialized.");
        }
    }

    void BannerOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received BannerOnAdLoadedEvent With AdInfo: {adInfo}");
    }

    void BannerOnAdLoadFailedEvent(LevelPlayAdError error)
    {
        Debug.Log($"[LevelPlaySample] Received BannerOnAdLoadFailedEvent With Error: {error}");
    }

    void BannerOnAdClickedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received BannerOnAdClickedEvent With AdInfo: {adInfo}");
    }

    void BannerOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received BannerOnAdDisplayedEvent With AdInfo: {adInfo}");
    }
#pragma warning disable 0618
    void BannerOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError adInfoError)
    {
        Debug.Log($"[LevelPlaySample] Received BannerOnAdDisplayFailedEvent With AdInfoError: {adInfoError}");
    }
#pragma warning restore 0618
    void BannerOnAdCollapsedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received BannerOnAdCollapsedEvent With AdInfo: {adInfo}");
    }

    void BannerOnAdLeftApplicationEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received BannerOnAdLeftApplicationEvent With AdInfo: {adInfo}");
    }

    void BannerOnAdExpandedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received BannerOnAdExpandedEvent With AdInfo: {adInfo}");
    }

    #endregion


    void ImpressionDataReadyEvent(LevelPlayImpressionData impressionData)
    {
        Debug.Log($"[LevelPlaySample] Received ImpressionDataReadyEvent ToString(): {impressionData}");
        Debug.Log($"[LevelPlaySample] Received ImpressionDataReadyEvent allData: {impressionData.AllData}");
    }

    private void OnDisable()
    {
        bannerAd?.DestroyAd();
        interstitialAd?.DestroyAd();
    }
}
