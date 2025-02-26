using System;

namespace com.unity3d.mediation
{
    /// <summary>
    /// APIs for LevelPlay Rewarded Ad in the Unity package.
    /// </summary>
    [Obsolete("The namespace com.unity3d.mediation is deprecated. Use ILevelPlayRewardedAd under the new namespace Unity.Services.LevelPlay.")]
    public interface ILevelPlayRewardedAd : Unity.Services.LevelPlay.ILevelPlayRewardedAd {}
}

namespace Unity.Services.LevelPlay
{
#pragma warning disable 0618
    /// <summary>
    /// APIs for LevelPlay Rewarded Ad in the Unity package.
    /// </summary>
    public interface ILevelPlayRewardedAd : IDisposable
    {
        /// <summary>
        /// Invoked when the Rewarded ad is loaded.
        /// </summary>
        event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdLoaded;

        /// <summary>
        /// Invoked when the Rewarded ad fails to load.
        /// </summary>
        event Action<com.unity3d.mediation.LevelPlayAdError> OnAdLoadFailed;

        /// <summary>
        /// Invoked when the Rewarded ad is displayed.
        /// </summary>
        event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdDisplayed;

        /// <summary>
        /// Invoked when the Rewarded ad fails to display.
        /// </summary>
        event Action<com.unity3d.mediation.LevelPlayAdDisplayInfoError> OnAdDisplayFailed;

        /// <summary>
        /// Invoked when the Rewarded ad receives a reward.
        /// </summary>
        event Action<com.unity3d.mediation.LevelPlayAdInfo, com.unity3d.mediation.LevelPlayReward> OnAdRewarded;

        /// <summary>
        /// Invoked when the Rewarded ad when the user clicks on the ad.
        /// </summary>
        event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdClicked;

        /// <summary>
        /// Invoked when the Rewarded ad is closed.
        /// </summary>
        event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdClosed;

        /// <summary>
        /// Invoked when the Rewarded ad info is changed.
        /// </summary>
        event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdInfoChanged;

        /// <summary>
        /// Gets the ad ID associated with this ad.
        /// </summary>
        /// <returns>The ID of the ad.</returns>
        string GetAdId();

        /// <summary>
        /// Gets the ad unit id of the ad.
        /// </summary>
        /// <returns>The ID of the ad unit.</returns>
        string AdUnitId { get; }

        /// <summary>
        /// Loads the Rewarded Ad.
        /// </summary>
        void LoadAd();

        /// <summary>
        /// Shows the Rewarded Ad.
        /// </summary>
        /// <param name="placementName"><i><b>(Optional)</b></i>Placement Name for the Rewarded Ad.</param>
        void ShowAd(string placementName = null);

        /// <summary>
        /// Destroys the Rewarded Ad.
        /// </summary>
        void DestroyAd();

        /// <summary>
        /// Checks if the Rewarded ad is ready
        /// </summary>
        /// <returns>Returns true if the Rewarded ad is ready, returns false if not.</returns>
        bool IsAdReady();
    }
}
