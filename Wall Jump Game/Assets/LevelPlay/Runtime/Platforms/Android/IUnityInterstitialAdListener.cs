namespace Unity.Services.LevelPlay
{
    interface IUnityInterstitialAdListener
    {
        void onAdLoaded(string adInfo);
        void onAdLoadFailed(string error);
        void onAdDisplayed(string adInfo);
        void onAdDisplayFailed(string error, string adInfo);
        void onAdClosed(string adInfo);
        void onAdClicked(string adInfo);
        void onAdInfoChanged(string adInfo);
    }
}
