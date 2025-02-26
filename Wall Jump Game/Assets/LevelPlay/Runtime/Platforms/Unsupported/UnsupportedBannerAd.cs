using System;
using Unity.Services.LevelPlay;

namespace com.unity3d.mediation
{
    [Obsolete("UnsupportedBannerAd will be deprecated.")]
    public class UnsupportedBannerAd : Unity.Services.LevelPlay.UnsupportedBannerAd
    {
        public UnsupportedBannerAd(string adUnitId, com.unity3d.mediation.LevelPlayAdSize size, com.unity3d.mediation.LevelPlayBannerPosition position, string placementId) : base(adUnitId, size, position, placementId) {}
    }
}

namespace Unity.Services.LevelPlay
{
#pragma warning disable 67, 0618
    public class UnsupportedBannerAd : IPlatformBannerAd
    {
        public UnsupportedBannerAd(string adUnitId, com.unity3d.mediation.LevelPlayAdSize size, com.unity3d.mediation.LevelPlayBannerPosition position, string placementId)
        {
            LevelPlayLogger.Log("UnsupportedBannerAd is not supported on this platform");
        }

        public event EventHandler<com.unity3d.mediation.LevelPlayAdInfo> OnAdLoaded;
        public event EventHandler<com.unity3d.mediation.LevelPlayAdError> OnAdLoadFailed;
        public event EventHandler<com.unity3d.mediation.LevelPlayAdInfo> OnAdClicked;
        public event EventHandler<com.unity3d.mediation.LevelPlayAdInfo> OnAdDisplayed;
        public event EventHandler<com.unity3d.mediation.LevelPlayAdDisplayInfoError> OnAdDisplayFailed;
        public event EventHandler<com.unity3d.mediation.LevelPlayAdInfo> OnAdExpanded;
        public event EventHandler<com.unity3d.mediation.LevelPlayAdInfo> OnAdCollapsed;
        public event EventHandler<com.unity3d.mediation.LevelPlayAdInfo> OnAdLeftApplication;

        public com.unity3d.mediation.LevelPlayBannerPosition Position { get; }

        public void Load()
        {
        }

        public void DestroyAd()
        {
        }

        public void ShowAd()
        {
        }

        public void HideAd()
        {
        }

        public void PauseAutoRefresh()
        {
        }

        public void ResumeAutoRefresh()
        {
        }

        public void SetAutoRefresh(bool flag)
        {
        }

        public void Dispose()
        {
        }

        public string AdId { get; }
        public string AdUnitId { get; }
        public com.unity3d.mediation.LevelPlayAdSize AdSize { get; }
        public LevelPlayAdSize Size { get; }
        public string PlacementName { get; }
    }
}
