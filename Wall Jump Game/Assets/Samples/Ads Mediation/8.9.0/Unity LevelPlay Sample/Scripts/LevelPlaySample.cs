using Unity.Services.LevelPlay;
using UnityEngine;

// This sample demonstrates how to use the LevelPlay SDK to load and show ads in a Unity game.
public class LevelPlaySample : MonoBehaviour
{
    [SerializeField]
    private Texture2D lpLogo;

    private LevelPlayBannerAd bannerAd;
    private LevelPlayInterstitialAd interstitialAd;
    private LevelPlayRewardedAd rewardedVideoAd;

    bool isAdsEnabled = false;

    public void Start()
    {
        Debug.Log("[LevelPlaySample] LevelPlay.ValidateIntegration");
        LevelPlay.ValidateIntegration();

        Debug.Log($"[LevelPlaySample] Unity version {LevelPlay.UnityVersion}");

        Debug.Log("[LevelPlaySample] Register initialization callbacks");
        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;

        // SDK init
        Debug.Log("[LevelPlaySample] LevelPlay SDK initialization");
        LevelPlay.Init(AdConfig.AppKey);
    }

    void EnableAds()
    {
        // Register to ImpressionDataReadyEvent
        LevelPlay.OnImpressionDataReady += ImpressionDataReadyEvent;

        // Create Rewarded Video object
        rewardedVideoAd = new LevelPlayRewardedAd(AdConfig.RewardedVideoAdUnitId);

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
        bannerAd = new LevelPlayBannerAd(AdConfig.BannerAdUnitId);

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
        interstitialAd = new LevelPlayInterstitialAd(AdConfig.InterstitalAdUnitId);

        // Register to Interstitial events
        interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
        interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
        interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
        interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
        interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
        interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
        interstitialAd.OnAdInfoChanged += InterstitialOnAdInfoChangedEvent;
    }

    #region GUI
    public void OnGUI()
    {
        GUI.enabled = isAdsEnabled;

        var safeArea = new Rect(
            Screen.safeArea.x,
            Screen.height - Screen.safeArea.yMax,
            Screen.safeArea.width,
            Screen.safeArea.height
        );

        var buttonXLeft = 0.05f * safeArea.width;
        var buttonXRight = 0.55f * safeArea.width;
        var buttonWidth = 0.4f * safeArea.width;
        var buttonHeight = 0.08f * safeArea.height;

        GUI.backgroundColor = Color.blue;
        GUI.skin.button.fontSize = 40;

        GUI.BeginGroup(safeArea);

        if (lpLogo != null)
        {
            const float lpLogoWidth = 500;
            const float lpLogoHeight = 100;
            var displayRect = new Rect((safeArea.width - lpLogoWidth) / 2.0f, 20, lpLogoWidth, lpLogoHeight);
            GUI.DrawTexture(displayRect, lpLogo, ScaleMode.ScaleToFit);
        }

        var loadRewardedVideoButton = new Rect(buttonXLeft, 0.15f * safeArea.height, buttonWidth, buttonHeight);
        if (GUI.Button(loadRewardedVideoButton, "Load Rewarded Video"))
        {
            Debug.Log("[LevelPlaySample] LoadRewardedVideoButtonClicked");
            rewardedVideoAd.LoadAd();
        }

        var showRewardedVideoButton = new Rect(buttonXRight, 0.15f * safeArea.height, buttonWidth, buttonHeight);
        if (GUI.Button(showRewardedVideoButton, "Show Rewarded Video"))
        {
            Debug.Log("[LevelPlaySample] ShowRewardedVideoButtonClicked");
            if (rewardedVideoAd.IsAdReady())
            {
                Debug.Log("[LevelPlaySample] Showing Rewarded Video Ad");
                rewardedVideoAd.ShowAd();
            }
            else
            {
                Debug.Log("[LevelPlaySample] LevelPlay Rewarded Video Ad is not ready");
            }
        }

        var loadInterstitialButton = new Rect(buttonXLeft, 0.25f * safeArea.height, buttonWidth, buttonHeight);
        if (GUI.Button(loadInterstitialButton, "Load Interstitial"))
        {
            Debug.Log("[LevelPlaySample] LoadInterstitialButtonClicked");
            interstitialAd.LoadAd();
        }

        var showInterstitialButton = new Rect(buttonXRight, 0.25f * safeArea.height, buttonWidth, buttonHeight);
        if (GUI.Button(showInterstitialButton, "Show Interstitial"))
        {
            Debug.Log("[LevelPlaySample] ShowInterstitialButtonClicked");
            if (interstitialAd.IsAdReady())
            {
                Debug.Log("[LevelPlaySample] Showing Interstitial Ad");
                interstitialAd.ShowAd();
            }
            else
            {
                Debug.Log("[LevelPlaySample] LevelPlay Interstital Ad is not ready");
            }
        }

        var loadBannerButton = new Rect(buttonXLeft, 0.35f * safeArea.height, buttonWidth, buttonHeight);
        if (GUI.Button(loadBannerButton, "Load Banner"))
        {
            Debug.Log("[LevelPlaySample] LoadBannerButtonClicked");
            bannerAd.LoadAd();
        }

        var hideBannerButton = new Rect(buttonXRight, 0.35f * safeArea.height, buttonWidth, buttonHeight);
        if (GUI.Button(hideBannerButton, "Hide Banner"))
        {
            Debug.Log("[LevelPlaySample] HideBannerButtonClicked");
            bannerAd.HideAd();
        }

        GUI.EndGroup();
    }
    #endregion

    #region Init callback handlers

    void SdkInitializationCompletedEvent(LevelPlayConfiguration config)
    {
        Debug.Log($"[LevelPlaySample] Received SdkInitializationCompletedEvent with Config: {config}");
        EnableAds();
        isAdsEnabled = true;
    }

    void SdkInitializationFailedEvent(LevelPlayInitError error)
    {
        Debug.Log($"[LevelPlaySample] Received SdkInitializationFailedEvent with Error: {error}");
    }

    #endregion

    #region AdInfo Rewarded Video
    void RewardedVideoOnLoadedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnLoadedEvent With AdInfo: {adInfo}");
    }

    void RewardedVideoOnAdLoadFailedEvent(LevelPlayAdError error)
    {
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdLoadFailedEvent With Error: {error}");
    }

    void RewardedVideoOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdDisplayedEvent With AdInfo: {adInfo}");
    }
#pragma warning disable 0618
    void RewardedVideoOnAdDisplayedFailedEvent(LevelPlayAdDisplayInfoError error)
    {
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdDisplayedFailedEvent With Error: {error}");
    }
#pragma warning restore 0618
    void RewardedVideoOnAdRewardedEvent(LevelPlayAdInfo adInfo, LevelPlayReward reward)
    {
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdRewardedEvent With AdInfo: {adInfo} and Reward: {reward}");
    }

    void RewardedVideoOnAdClickedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdClickedEvent With AdInfo: {adInfo}");
    }

    void RewardedVideoOnAdClosedEvent(LevelPlayAdInfo adInfo)
    {
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

    #region ImpressionSuccess callback handler

    void ImpressionDataReadyEvent(LevelPlayImpressionData impressionData)
    {
        Debug.Log($"[LevelPlaySample] Received ImpressionDataReadyEvent ToString(): {impressionData}");
        Debug.Log($"[LevelPlaySample] Received ImpressionDataReadyEvent allData: {impressionData.AllData}");
    }

    #endregion

    private void OnDisable()
    {
        bannerAd?.DestroyAd();
        interstitialAd?.DestroyAd();
    }
}
