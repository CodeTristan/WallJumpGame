using System;
using com.unity3d.mediation;

namespace com.unity3d.mediation
{
    /// <summary>
    /// APIs for LevelPlay Interstitial Ad in the Unity package.
    /// </summary>
    [Obsolete("The namespace com.unity3d.mediation is deprecated. Use ILevelPlayInterstitialAd under the new namespace Unity.Services.LevelPlay.")]
    public interface ILevelPlayInterstitialAd : Unity.Services.LevelPlay.ILevelPlayInterstitialAd {}
}

namespace Unity.Services.LevelPlay
{
#pragma warning disable 0618
    /// <summary>
    /// APIs for LevelPlay Interstitial Ad in the Unity package.
    /// </summary>
    public interface ILevelPlayInterstitialAd : IDisposable
    {
        /// <summary>
        /// Invoked when the interstitial ad is loaded.
        /// </summary>
        event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdLoaded;

        /// <summary>
        /// Invoked when the interstitial ad fails to load.
        /// </summary>
        event Action<com.unity3d.mediation.LevelPlayAdError> OnAdLoadFailed;

        /// <summary>
        /// Invoked when the interstitial ad is displayed.
        /// </summary>
        event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdDisplayed;

        /// <summary>
        /// Invoked when the interstitial ad is closed.
        /// </summary>
        event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdClosed;

        /// <summary>
        /// Invoked when the user clicks on the interstitial ad.
        /// </summary>
        event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdClicked;

        /// <summary>
        /// Invoked when the interstitial ad fails to display.
        /// </summary>
#pragma warning disable 0618
        event Action<com.unity3d.mediation.LevelPlayAdDisplayInfoError> OnAdDisplayFailed;

        /// <summary>
        /// Invoked when the interstitial ad info is changed.
        /// </summary>
        event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdInfoChanged;
#pragma warning restore 0618

        /// <summary>
        /// Gets the ad ID associated with this ad.
        /// </summary>
        /// <returns>The ID of the ad unit.</returns>
        string GetAdId();

        /// <summary>
        /// Gets the ad unit id of the ad.
        /// </summary>
        /// <returns>The ID of the ad unit.</returns>
        string AdUnitId { get; }

        /// <summary>
        /// Loads the Interstitial Ad.
        /// </summary>
        void LoadAd();

        /// <summary>
        /// Shows the Interstitial Ad.
        /// </summary>
        /// <param name="placementName"><i><b>(Optional)</b></i>Placement Name for the Interstitial Ad.</param>
        void ShowAd(string placementName = null);

        /// <summary>
        /// Destroys the Interstitial Ad.
        /// </summary>
        void DestroyAd();

        /// <summary>
        /// Checks if the interstitial ad is ready
        /// </summary>
        /// <returns>Returns true if the interstitial ad is ready, returns false if not.</returns>
        bool IsAdReady();
    }
}
