#if UNITY_IOS && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using com.unity3d.mediation;

namespace Unity.Services.LevelPlay
{
    class IosInterstitialAd : IosNativeObject, IPlatformInterstitialAd
    {
        public event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdLoaded;
        public event Action<com.unity3d.mediation.LevelPlayAdError> OnAdLoadFailed;
        public event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdDisplayed;
        public event Action<com.unity3d.mediation.LevelPlayAdDisplayInfoError> OnAdDisplayFailed;
        public event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdClicked;
        public event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdClosed;
        public event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdInfoChanged;

        public string AdUnitId { get; }

        public string AdId { get {return GetAdId();} }

        IosInterstitialAdListener m_InterstitialListener;

        public IosInterstitialAd(string adUnitId) : base(true)
        {
            AdUnitId = adUnitId;
            NativePtr = InterstitialAdCreate(adUnitId);
            m_InterstitialListener = new IosInterstitialAdListener(this);
            InterstitialAdSetDelegate(NativePtr, m_InterstitialListener.NativePtr);
        }

        public void LoadAd()
        {
            if (CheckDisposedAndLogError("Cannot Load Interstitial Ad")) return;
            InterstitialAdLoadAd(NativePtr);
        }

        public void ShowAd(string placementName)
        {
            if (CheckDisposedAndLogError("Cannot Show Interstitial Ad")) return;
            InterstitialAdShowAd(NativePtr, placementName);
        }

        public bool IsAdReady()
        {
            if (CheckDisposedAndLogError("Cannot Check if Interstitial Ad is Ready")) return false;
            return InterstitialAdIsAdReady(NativePtr);
        }

        public static bool IsPlacementCapped(string placementName)
        {
            return InterstitialAdIsPlacementCapped(placementName);
        }

        public override void Dispose()
        {
            m_InterstitialListener?.Dispose();
            m_InterstitialListener = null;
            base.Dispose();
        }

        internal void InvokeLoadedEvent(string adInfo)
        {
            ThreadUtil.Post(state => OnAdLoaded?.Invoke(new com.unity3d.mediation.LevelPlayAdInfo(adInfo)));
        }

        internal void InvokeFailedLoadEvent(string error)
        {
            ThreadUtil.Post(state => OnAdLoadFailed?.Invoke(new com.unity3d.mediation.LevelPlayAdError(error)));
        }

        internal void InvokeClickedEvent(string adInfo)
        {
            ThreadUtil.Post(state => OnAdClicked?.Invoke(new com.unity3d.mediation.LevelPlayAdInfo(adInfo)));
        }

        internal void InvokeDisplayedEvent(string adInfo)
        {
            ThreadUtil.Post(state => OnAdDisplayed?.Invoke(new com.unity3d.mediation.LevelPlayAdInfo(adInfo)));
        }

        internal void InvokeFailedDisplayEvent(string adInfo, string error)
        {
            var errorInfo = new com.unity3d.mediation.LevelPlayAdDisplayInfoError(new LevelPlayAdInfo(adInfo), new LevelPlayAdError(error));
            ThreadUtil.Post(state => OnAdDisplayFailed?.Invoke(errorInfo));
        }

        internal void InvokeClosedEvent(string adInfo)
        {
            ThreadUtil.Post(state => OnAdClosed?.Invoke(new com.unity3d.mediation.LevelPlayAdInfo(adInfo)));
        }

        internal void InvokeOnAdInfoChangedEvent(string adInfo)
        {
            ThreadUtil.Post(state => OnAdInfoChanged?.Invoke(new com.unity3d.mediation.LevelPlayAdInfo(adInfo)));
        }

        private string GetAdId()
        {
            if (CheckDisposedAndLogError("Cannot get Interstitial ad Id")) return "";
            return InterstitialAdId(NativePtr);
        }

        ~IosInterstitialAd()
        {
            Dispose();
        }

        [DllImport("__Internal", EntryPoint = "LPMInterstitialAdCreate")]
        static extern IntPtr InterstitialAdCreate(string adUnitId);

        [DllImport("__Internal", EntryPoint = "LPMInterstitialAdSetDelegate")]
        static extern void InterstitialAdSetDelegate(IntPtr interstitialAd, IntPtr interstitialAdListener);

        [DllImport("__Internal", EntryPoint = "LPMInterstitialAdLoadAd")]
        static extern void InterstitialAdLoadAd(IntPtr interstitialAd);

        [DllImport("__Internal", EntryPoint = "LPMInterstitialAdShowAd")]
        static extern void InterstitialAdShowAd(IntPtr interstitialAd, string placementName);

        [DllImport("__Internal", EntryPoint = "LPMInterstitialAdIsAdReady")]
        static extern bool InterstitialAdIsAdReady(IntPtr interstitialAd);

        [DllImport("__Internal", EntryPoint = "LPMInterstitialAdIsPlacementCapped")]
        static extern bool InterstitialAdIsPlacementCapped(string placementName);

        [DllImport("__Internal", EntryPoint = "LPMInterstitialAdAdId")]
        static extern string InterstitialAdId(IntPtr interstitialAd);
    }
}
#endif
