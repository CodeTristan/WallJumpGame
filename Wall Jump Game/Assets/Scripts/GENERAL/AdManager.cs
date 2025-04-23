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
    public const string INSERSTITIAL_AD_ID = "ur2jrunhpqnqt7tn";
    public const string REWARDED_AD_ID = "rnkq0zu8ryjzylzb";
    public const string NATIVE_AD_ID = "kt4l55oylbdqwqw6";

    public delegate void ShowRewardedAdFailDelegate();

    private LevelPlayBannerAd bannerAd;
    private LevelPlayInterstitialAd interstitialAd;

    private bool inited = false;
    public bool DEBUG_MODE = false;
    public bool InAdMenu;


    private bool RewardAdReady;
    public void Init()
    {
        instance = this;
        debugText.gameObject.SetActive(DEBUG_MODE);
        debugCanvas.enabled = DEBUG_MODE;

        //IronSourceEvents.onSdkInitializationCompletedEvent += SDKInitialized;

        //IronSource.Agent.init(appKey);
        //LevelPlay.Init(appKey);

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
                IronSource.Agent.validateIntegration();
        #endif

        LevelPlay.OnInitSuccess += SDKInitialized;
        LevelPlay.OnInitFailed += LevelPlay_OnInitFailed;

        LevelPlay.Init(appKey, GetUserId());
        IronSource.Agent.init(appKey);

        //Add AdInfo Rewarded Video Events
        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;
        IronSourceRewardedVideoEvents.onAdReadyEvent += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdLoadFailedEvent += RewardedVideoFailedToLoad;

        //IronSource.Agent.shouldTrackNetworkState(true);

    }

    private void Start()
    {


    }

    public void OnPLayerDiedFR()
    {
        if(GameManager.instance.current_Game_Before_Ad == 0)
        {
            GameManager.instance.current_Game_Before_Ad = GameManager.instance.Number_Of_Game_Before_Ad;
            ShowInterstitialAd("Game_Over");
        }
        else
        {
            GameManager.instance.current_Game_Before_Ad--;
        }
    }
    private void LevelPlay_OnInitFailed(LevelPlayInitError error)
    {
        Debug.LogError("LevelPlay Initialization Failed: " + error.ToString());
        debugText.text += "Init Failed: " + error.ToString();
    }

    private void OnEnable()
    {
    }

    void RewardedVideoFailedToLoad(IronSourceError error)
    {
        Debug.LogError("Rewarded Video Failed to Load: " + error.getDescription());
        debugText.text += " Rewarded Video Failed to Load: " + error.getDescription() + " ";
    }

    private void OnDisable()
    {
        LevelPlay.OnInitSuccess -= SDKInitialized;

        if(bannerAd != null)
        {
            bannerAd.OnAdLoaded -= BannerOnAdLoadedEvent;
            bannerAd.OnAdLoadFailed -= BannerOnAdLoadFailedEvent;
            bannerAd.OnAdDisplayed -= BannerOnAdDisplayedEvent;
            bannerAd.OnAdDisplayFailed -= BannerOnAdDisplayFailedEvent;
            bannerAd.OnAdClicked -= BannerOnAdClickedEvent;
            bannerAd.OnAdCollapsed -= BannerOnAdCollapsedEvent;
            bannerAd.OnAdLeftApplication -= BannerOnAdLeftApplicationEvent;
            bannerAd.OnAdExpanded -= BannerOnAdExpandedEvent;
        }

        if(interstitialAd != null)
        {
            interstitialAd.OnAdLoaded -= InterstitialOnAdLoadedEvent;
            interstitialAd.OnAdLoadFailed -= InterstitialOnAdLoadFailedEvent;
            interstitialAd.OnAdDisplayed -= InterstitialOnAdDisplayedEvent;
            interstitialAd.OnAdDisplayFailed -= InterstitialOnAdDisplayFailedEvent;
            interstitialAd.OnAdClicked -= InterstitialOnAdClickedEvent;
            interstitialAd.OnAdClosed -= InterstitialOnAdClosedEvent;
            interstitialAd.OnAdInfoChanged -= InterstitialOnAdInfoChangedEvent;
        }

        //Add AdInfo Rewarded Video Events
        IronSourceRewardedVideoEvents.onAdOpenedEvent -= RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent -= RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent -= RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent -= RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent -= RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent -= RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent -= RewardedVideoOnAdClickedEvent;
    }
    private void OnApplicationPause(bool pause)
    {
        IronSource.Agent.onApplicationPause(pause);
    }

    private void SDKInitialized(LevelPlayConfiguration levelPlayConfiguration)
    {
        Debug.Log("SDKInitialized");
        debugText.text += " SDK Initialized... ";
        inited = true;
        CreateBannerAd();
        CreateInterstitialAd();
        LoadInterstitialAd();
        LoadBannerAd();
        if (SahneManager.instance.currentSceneEnum == SceneEnum.GameScene)
            HideBannerAd();
    }

    private string GetUserId()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }

    #region RewardedAd

    public void LoadRewardedAd()
    {
        IronSource.Agent.loadRewardedVideo();
    }
    public void ShowRewardedAd(string placementName,List<ShowRewardedAdFailDelegate> functionsWhenFail)
    {
        IronSourcePlacement placement = IronSource.Agent.getPlacementInfo(placementName);
        //Placement can return null if the placementName is not valid.
        if (placement != null)
        {
            string rewardName = placement.getRewardName();
            int rewardAmount = placement.getRewardAmount();
        }

        debugText.text += " " + IronSource.Agent.isRewardedVideoAvailable() + " " + IronSource.Agent.isRewardedVideoPlacementCapped(placementName) + " ";
            
        if(IronSource.Agent.isRewardedVideoAvailable() || RewardAdReady)
        {
            IronSource.Agent.showRewardedVideo(placementName);
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

    //REWARD THE PLAYER
    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        //TODO - here you can reward the user according to the reward name and amount
        string PlacementName = placement.getPlacementName();
        string rewardName = placement.getRewardName();
        int RewardAmount = placement.getRewardAmount();

        debugText.text = "Reward: " + rewardName + "\nAmount: " + RewardAmount;

        if(PlacementName == "Respawn")
        {
            InAdMenu = false;
            GameSceneUIManager.instance.ToggleDeathAdScreen(false);
            PlayerManager.instance.Respawn();
        }
        if(PlacementName == "DoubleMoney")
        {
            InAdMenu = false;
            GameSceneUIManager.instance.DeathScreen();
        }

        IronSource.Agent.loadRewardedVideo();
    }

    /************* RewardedVideo AdInfo Delegates *************/
    // Indicates that there’s an available ad.
    // The adInfo object includes information about the ad that was loaded successfully
    // This replaces the RewardedVideoAvailabilityChangedEvent(true) event
    void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
    {
        debugText.text += " Ad ready: " + adInfo.ToString();
        RewardAdReady = true;
    }
    // Indicates that no ads are available to be displayed
    // This replaces the RewardedVideoAvailabilityChangedEvent(false) event
    void RewardedVideoOnAdUnavailable()
    {
        debugText.text += " Ad unavailable... ";
        RewardAdReady = false;

        IronSource.Agent.loadRewardedVideo();
    }
    // The Rewarded Video ad view has opened. Your activity will loose focus.
    void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {

    }
    // The Rewarded Video ad view is about to be closed. Your activity will regain its focus.
    void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        debugText.text = " Ad Closed: " + adInfo.ToString();
        InAdMenu = false;
        IronSource.Agent.loadRewardedVideo();
    }
    // The user completed to watch the video, and should be rewarded.
    // The placement parameter will include the reward data.
    // When using server-to-server callbacks, you may ignore this event and wait for the ironSource server callback.
    // The rewarded video ad was failed to show.
    void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
    {
        debugText.text += error.ToString();

    }
    // Invoked when the video ad was clicked.
    // This callback is not supported by all networks, and we recommend using it only if
    // it’s supported by all networks you included in your build.
    void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        debugText.text += " Ad Clicked: " + adInfo.ToString();
    }



    #endregion

    #region Interstitial

    public void CreateInterstitialAd()
    {
        interstitialAd = new LevelPlayInterstitialAd(INSERSTITIAL_AD_ID);

        // Register to interstitial events
        interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
        interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
        interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
        interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
        interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
        interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
        interstitialAd.OnAdInfoChanged += InterstitialOnAdInfoChangedEvent;
    }

    public void LoadInterstitialAd()
    {
        if(interstitialAd != null)
            interstitialAd.LoadAd();
    }

    public void ShowInterstitialAd(string placementName)
    {
        if (interstitialAd.IsAdReady() && !LevelPlayInterstitialAd.IsPlacementCapped(placementName))
            interstitialAd.ShowAd(placementName);
    }

    // Implement the events
    void InterstitialOnAdLoadedEvent(LevelPlayAdInfo adInfo) { debugText.text += adInfo.ToString(); }
    void InterstitialOnAdLoadFailedEvent(LevelPlayAdError error) { debugText.text += error.ToString(); }
    void InterstitialOnAdDisplayedEvent(LevelPlayAdInfo adInfo) { debugText.text += adInfo.ToString(); }
    void InterstitialOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError infoError) { debugText.text += infoError.ToString(); }
    void InterstitialOnAdClickedEvent(LevelPlayAdInfo adInfo) { debugText.text += adInfo.ToString(); }
    void InterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo) { debugText.text += adInfo.ToString(); LoadInterstitialAd(); }
    void InterstitialOnAdInfoChangedEvent(LevelPlayAdInfo adInfo) { debugText.text += adInfo.ToString(); }


    #endregion


    #region Banner   

    public void CreateBannerAd()
    {
        //Create banner instance
        bannerAd = new LevelPlayBannerAd(BANNER_AD_ID);
        //Subscribe BannerAd events
        bannerAd.OnAdLoaded += BannerOnAdLoadedEvent;
        bannerAd.OnAdLoadFailed += BannerOnAdLoadFailedEvent;
        bannerAd.OnAdDisplayed += BannerOnAdDisplayedEvent;
        bannerAd.OnAdDisplayFailed += BannerOnAdDisplayFailedEvent;
        bannerAd.OnAdClicked += BannerOnAdClickedEvent;
        bannerAd.OnAdCollapsed += BannerOnAdCollapsedEvent;
        bannerAd.OnAdLeftApplication += BannerOnAdLeftApplicationEvent;
        bannerAd.OnAdExpanded += BannerOnAdExpandedEvent;
    }
    public void LoadBannerAd()
    {
        //Load the banner ad 
        if(bannerAd != null)
            bannerAd.LoadAd();
    }
    public void ShowBannerAd()
    {
        //Show the banner ad, call this method only if you turned off the auto show when you created this banner instance.
        if(bannerAd != null)
            bannerAd.ShowAd();
    }
    public void HideBannerAd()
    {
        //Hide banner
        if (bannerAd != null)
            bannerAd.HideAd();
    }
    public void DestroyBannerAd()
    {
        //Destroy banner
        if (bannerAd != null)
            bannerAd.DestroyAd();

        bannerAd = null;
    }
    //Implement BannAd Events
    void BannerOnAdLoadedEvent(LevelPlayAdInfo adInfo) { debugText.text += "Banner Loaded: " + adInfo.ToString(); ShowBannerAd();}
    void BannerOnAdLoadFailedEvent(LevelPlayAdError ironSourceError) { debugText.text += "Banner Load Failed: " + ironSourceError.ToString(); }
    void BannerOnAdClickedEvent(LevelPlayAdInfo adInfo) { debugText.text+= adInfo.ToString(); }
    void BannerOnAdDisplayedEvent(LevelPlayAdInfo adInfo) { debugText.text += adInfo.ToString(); }
    void BannerOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError adInfoError) { debugText.text += adInfoError.ToString(); }
    void BannerOnAdCollapsedEvent(LevelPlayAdInfo adInfo) { debugText.text += adInfo.ToString();}
    void BannerOnAdLeftApplicationEvent(LevelPlayAdInfo adInfo) { debugText.text += adInfo.ToString(); }
    void BannerOnAdExpandedEvent(LevelPlayAdInfo adInfo) { debugText.text += adInfo.ToString(); }
    #endregion
}
