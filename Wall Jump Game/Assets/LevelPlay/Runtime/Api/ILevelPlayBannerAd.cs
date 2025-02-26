using System;

namespace com.unity3d.mediation
{
    /// <summary>
    /// Defines the interface for banner ads in the LevelPlay mediation system.
    /// </summary>
    [Obsolete("The namespace com.unity3d.mediation is deprecated. Use ILevelPlayBannerAd under the new namespace Unity.Services.LevelPlay.")]
    public interface ILevelPlayBannerAd : Unity.Services.LevelPlay.ILevelPlayBannerAd {}
}

namespace Unity.Services.LevelPlay
{
#pragma warning disable 0618
    /// <summary>
    /// Defines the interface for banner ads in the LevelPlay mediation system.
    /// </summary>
    public interface ILevelPlayBannerAd : IDisposable
    {
        /// <summary>
        /// Loads the banner ad.
        /// </summary>
        void LoadAd();

        /// <summary>
        /// Destroys the banner ad and releases resources.
        /// </summary>
        void DestroyAd();

        /// <summary>
        /// Displays the banner ad to the user.
        /// </summary>
        void ShowAd();

        /// <summary>
        /// Hides the banner ad from the user.
        /// </summary>
        void HideAd();

        /// <summary>
        /// Gets the ad ID associated with this ad.
        /// </summary>
        /// <returns>The ID of the ad.</returns>
        public string GetAdId();

        /// <summary>
        /// Gets the ad unit ID associated with this ad.
        /// </summary>
        /// <returns>The ID of the ad unit.</returns>
        string GetAdUnitId();

        /// <summary>
        /// Retrieves the size of the ad.
        /// </summary>
        /// <returns>The size of the ad.</returns>
        com.unity3d.mediation.LevelPlayAdSize GetAdSize();

        /// <summary>
        /// Retrieves the position of the banner ad.
        /// </summary>
        /// <returns>The position of the ad.</returns>
        com.unity3d.mediation.LevelPlayBannerPosition GetPosition();

        /// <summary>
        /// Retrieves the placement name associated with this ad.
        /// </summary>
        /// <returns>The placement name of the ad.</returns>
        string GetPlacementName();

        /// <summary>
        /// Pauses the auto-refreshing of the banner ad.
        /// </summary>
        void PauseAutoRefresh();

        /// <summary>
        /// Resumes the auto-refreshing of the banner ad that was previously paused.
        /// </summary>
        void ResumeAutoRefresh();


        event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdLoaded;
        event Action<com.unity3d.mediation.LevelPlayAdError> OnAdLoadFailed;
        event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdClicked;
        event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdDisplayed;
        event Action<com.unity3d.mediation.LevelPlayAdDisplayInfoError> OnAdDisplayFailed;
        event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdExpanded;
        event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdCollapsed;
        event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdLeftApplication;
    }
}
