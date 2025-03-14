#if UNITY_ANDROID
namespace Unity.Services.LevelPlay
{
    interface IUnityBannerAdListener
    {
        void onAdLoaded(string adInfo);
        void onAdLoadFailed(string error);
        void onAdDisplayed(string adInfo);
        void onAdDisplayFailed(string adInfo, string error);
        void onAdClicked(string adInfo);
        void onAdExpanded(string adInfo);
        void onAdCollapsed(string adInfo);
        void onAdLeftApplication(string adInfo);
    }
}
#endif
