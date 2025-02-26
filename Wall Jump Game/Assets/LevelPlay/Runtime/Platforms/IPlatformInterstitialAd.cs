using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Unity.Services.LevelPlay.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Unity.Services.LevelPlay
{
    internal interface IPlatformInterstitialAd : IDisposable
    {
#pragma warning disable 0618
        event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdLoaded;
        event Action<com.unity3d.mediation.LevelPlayAdError> OnAdLoadFailed;
        event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdDisplayed;
        event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdClosed;
        event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdClicked;
        event Action<com.unity3d.mediation.LevelPlayAdDisplayInfoError> OnAdDisplayFailed;
        event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdInfoChanged;
#pragma warning restore 0618

        string AdId { get; }
        string AdUnitId { get; }

        void LoadAd();

        void ShowAd(string placementName);

        bool IsAdReady();
    }
}
