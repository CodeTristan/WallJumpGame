#if UNITY_IOS && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using com.unity3d.mediation;

namespace Unity.Services.LevelPlay
{
    class IosRewardedAd : IosNativeObject, IPlatformRewardedAd
    {
        public event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdLoaded;
        public event Action<com.unity3d.mediation.LevelPlayAdError> OnAdLoadFailed;
        public event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdDisplayed;
        public event Action<com.unity3d.mediation.LevelPlayAdDisplayInfoError> OnAdDisplayFailed;
        public event Action<com.unity3d.mediation.LevelPlayAdInfo, com.unity3d.mediation.LevelPlayReward> OnAdRewarded;
        public event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdClicked;
        public event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdClosed;
        public event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdInfoChanged;

        public string AdUnitId { get; }
        public string AdId { get {return GetAdId();} }

        IosRewardedAdListener m_RewardedAdListener;

        public IosRewardedAd(string adUnitId) : base(true)
        {
            AdUnitId = adUnitId;
            NativePtr = RewardedAdCreate(adUnitId);
            m_RewardedAdListener = new IosRewardedAdListener(this);
            RewardedAdSetDelegate(NativePtr, m_RewardedAdListener.NativePtr);
        }

        public void LoadAd()
        {
            if (CheckDisposedAndLogError("Cannot Load Rewarded Ad")) return;
            RewardedAdLoadAd(NativePtr);
        }

        public void ShowAd(string placementName)
        {
            if (CheckDisposedAndLogError("Cannot Show Rewarded Ad")) return;
            RewardedAdShowAd(NativePtr, placementName);
        }

        public bool IsAdReady()
        {
            if (CheckDisposedAndLogError("Cannot Check if Rewarded Ad is Ready")) return false;
            return RewardedAdIsAdReady(NativePtr);
        }

        public static bool IsPlacementCapped(string placementName)
        {
            return RewardedAdIsPlacementCapped(placementName);
        }

        public override void Dispose()
        {
            m_RewardedAdListener?.Dispose();
            m_RewardedAdListener = null;
            base.Dispose();
        }

        internal void InvokeLoadedEvent(string adInfo)
        {
            ThreadUtil.Post(_ => OnAdLoaded?.Invoke(new com.unity3d.mediation.LevelPlayAdInfo(adInfo)));
        }

        internal void InvokeFailedLoadEvent(string error)
        {
            ThreadUtil.Post(_ => OnAdLoadFailed?.Invoke(new com.unity3d.mediation.LevelPlayAdError(error)));
        }

        internal void InvokeDisplayedEvent(string adInfo)
        {
            ThreadUtil.Post(_ => OnAdDisplayed?.Invoke(new com.unity3d.mediation.LevelPlayAdInfo(adInfo)));
        }

        internal void InvokeFailedDisplayEvent(string adInfo, string error)
        {
            var errorInfo = new com.unity3d.mediation.LevelPlayAdDisplayInfoError(new LevelPlayAdInfo(adInfo), new LevelPlayAdError(error));
            ThreadUtil.Post(_ => OnAdDisplayFailed?.Invoke(errorInfo));
        }

        internal void InvokeRewardedEvent(string adInfo, string rewardName, int rewardAmount)
        {
            ThreadUtil.Post(_ => OnAdRewarded?.Invoke(new com.unity3d.mediation.LevelPlayAdInfo(adInfo), new com.unity3d.mediation.LevelPlayReward(rewardName, rewardAmount)));
        }

        internal void InvokeClickedEvent(string adInfo)
        {
            ThreadUtil.Post(_ => OnAdClicked?.Invoke(new com.unity3d.mediation.LevelPlayAdInfo(adInfo)));
        }

        internal void InvokeClosedEvent(string adInfo)
        {
            ThreadUtil.Post(_ => OnAdClosed?.Invoke(new com.unity3d.mediation.LevelPlayAdInfo(adInfo)));
        }

        internal void InvokeOnAdInfoChangedEvent(string adInfo)
        {
            ThreadUtil.Post(_ => OnAdInfoChanged?.Invoke(new com.unity3d.mediation.LevelPlayAdInfo(adInfo)));
        }

        private string GetAdId()
        {
            if (CheckDisposedAndLogError("Cannot get Rewarded ad Id")) return "";
            return RewardedAdId(NativePtr);
        }

        ~IosRewardedAd()
        {
            Dispose();
        }

        [DllImport("__Internal", EntryPoint = "LPMRewardedAdCreate")]
        static extern IntPtr RewardedAdCreate(string adUnitId);

        [DllImport("__Internal", EntryPoint = "LPMRewardedAdSetDelegate")]
        static extern void RewardedAdSetDelegate(IntPtr rewardedAd, IntPtr rewardedAdListener);

        [DllImport("__Internal", EntryPoint = "LPMRewardedAdLoadAd")]
        static extern void RewardedAdLoadAd(IntPtr rewardedAd);

        [DllImport("__Internal", EntryPoint = "LPMRewardedAdShowAd")]
        static extern void RewardedAdShowAd(IntPtr rewardedAd, string placementName);

        [DllImport("__Internal", EntryPoint = "LPMRewardedAdIsAdReady")]
        static extern bool RewardedAdIsAdReady(IntPtr rewardedAd);

        [DllImport("__Internal", EntryPoint = "LPMRewardedAdIsPlacementCapped")]
        static extern bool RewardedAdIsPlacementCapped(string placementName);

        [DllImport("__Internal", EntryPoint = "LPMRewardedAdAdId")]
        static extern string RewardedAdId(IntPtr rewardedAd);
    }
}
#endif
