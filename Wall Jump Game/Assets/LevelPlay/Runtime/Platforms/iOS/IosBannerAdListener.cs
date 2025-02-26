#if UNITY_IOS && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using AOT;
using com.unity3d.mediation;

namespace Unity.Services.LevelPlay
{
    class IosBannerAdListener : IosNativeObject
    {
        readonly DidLoadAdWithAdInfo _kLoadSuccessCallback = LoadSuccess;
        readonly DidFailToLoadAdWithAdUnitId _kFailedToLoad = LoadFailed;
        readonly DidClickWithAdInfo _kClicked = Clicked;
        readonly DidDisplayWithAdInfo _kDisplayed = Displayed;
        readonly DidFailToDisplayWithAdInfo _kDisplayFailed = DisplayFailed;
        readonly DidExpandAdWithAdInfo _kExpanded = Expanded;
        readonly DidCollapseAdWithAdInfo _kCollapsed = Collapsed;
        readonly DidLeaveAppWithAdInfo _kLeftApp = LeftApplication;

        iOSBannerAd _bannerAd;

        public IosBannerAdListener(iOSBannerAd bannerAd) : base(true)
        {
            NativePtr = LPMBannerAdViewDelegateCreate(bannerAd.NativePtr, _kLoadSuccessCallback, _kFailedToLoad, _kClicked, _kDisplayed, _kDisplayFailed, _kExpanded, _kCollapsed, _kLeftApp);
            _bannerAd = bannerAd;
        }

        public override void Dispose()
        {
            if (NativePtr != IntPtr.Zero)
            {
                LPMBannerAdViewDelegateDestroy(NativePtr);
                NativePtr = IntPtr.Zero;
                _bannerAd = null;
            }
            base.Dispose();
        }

        [DllImport("__Internal", EntryPoint = "LPMBannerAdViewDelegateCreate")]
        private static extern IntPtr LPMBannerAdViewDelegateCreate(IntPtr bannerAdPtr, DidLoadAdWithAdInfo loadSuccess, DidFailToLoadAdWithAdUnitId loadFailed, DidClickWithAdInfo clicked, DidDisplayWithAdInfo displayed, DidFailToDisplayWithAdInfo displayFailed, DidExpandAdWithAdInfo expanded, DidCollapseAdWithAdInfo collapsed, DidLeaveAppWithAdInfo leftApp);

        [DllImport("__Internal", EntryPoint = "LPMBannerAdViewDelegateDestroy")]
        private static extern void LPMBannerAdViewDelegateDestroy(IntPtr delegatePtr);


        // Events from native
        delegate void DidLoadAdWithAdInfo(IntPtr bannerPtr , string adInfoJson);

        delegate void DidFailToLoadAdWithAdUnitId(IntPtr bannerPtr, string errorPtr);

        delegate void DidClickWithAdInfo(IntPtr bannerPtr, string adInfoJson);

        delegate void DidDisplayWithAdInfo(IntPtr bannerPtr, string adInfoJson);

        delegate void DidFailToDisplayWithAdInfo(IntPtr bannerPtr, string adInfoJson, string errorPtr);

        delegate void DidExpandAdWithAdInfo(IntPtr bannerPtr, string adInfoJson);

        delegate void DidCollapseAdWithAdInfo(IntPtr bannerPtr, string adInfoJson);

        delegate void DidLeaveAppWithAdInfo(IntPtr bannerPtr, string adInfoJson);

        [MonoPInvokeCallback(typeof(DidLoadAdWithAdInfo))]
        static void LoadSuccess(IntPtr ptr , string adInfoJson)
        {
            var bannerAd = Get<iOSBannerAd>(ptr);
            var adInfo = new com.unity3d.mediation.LevelPlayAdInfo(adInfoJson);
            bannerAd?.InvokeLoadedEvent(adInfo); //iOSBanner.cs
        }

        [MonoPInvokeCallback(typeof(DidFailToLoadAdWithAdUnitId))]
        static void LoadFailed(IntPtr ptr, string errorPtr)
        {
            var bannerAd = Get<iOSBannerAd>(ptr);
            bannerAd?.InvokeFailedLoadEvent(new com.unity3d.mediation.LevelPlayAdError(errorPtr));
        }

        [MonoPInvokeCallback(typeof(DidClickWithAdInfo))]
        static void Clicked(IntPtr ptr, string adInfoJson)
        {
            var bannerAd = Get<iOSBannerAd>(ptr);
            bannerAd?.InvokeClickedEvent(new com.unity3d.mediation.LevelPlayAdInfo(adInfoJson));
        }

        [MonoPInvokeCallback(typeof(DidDisplayWithAdInfo))]
        static void Displayed(IntPtr ptr, string adInfoJson)
        {
            var bannerAd = Get<iOSBannerAd>(ptr);
            bannerAd?.InvokeDisplayedEvent(new com.unity3d.mediation.LevelPlayAdInfo(adInfoJson));
        }

        [MonoPInvokeCallback(typeof(DidFailToDisplayWithAdInfo))]
        static void DisplayFailed(IntPtr ptr, string adInfoJson, string errorPtr)
        {
            var bannerAd = Get<iOSBannerAd>(ptr);
            bannerAd?.InvokeFailedDisplayEvent(new com.unity3d.mediation.LevelPlayAdInfo(adInfoJson), new LevelPlayAdError(errorPtr));
        }

        [MonoPInvokeCallback(typeof(DidExpandAdWithAdInfo))]
        static void Expanded(IntPtr ptr, string adInfoJson)
        {
            var bannerAd = Get<iOSBannerAd>(ptr);
            bannerAd?.InvokeExpandedEvent(new com.unity3d.mediation.LevelPlayAdInfo(adInfoJson));
        }

        [MonoPInvokeCallback(typeof(DidCollapseAdWithAdInfo))]
        static void Collapsed(IntPtr ptr, string adInfoJson)
        {
            var bannerAd = Get<iOSBannerAd>(ptr);
            bannerAd?.InvokeCollapsedEvent(new com.unity3d.mediation.LevelPlayAdInfo(adInfoJson));
        }

        [MonoPInvokeCallback(typeof(DidLeaveAppWithAdInfo))]
        static void LeftApplication(IntPtr ptr, string adInfoJson)
        {
            var bannerAd = Get<iOSBannerAd>(ptr);
            bannerAd?.InvokeLeftApplicationEvent(new com.unity3d.mediation.LevelPlayAdInfo(adInfoJson));
        }
    }
}
#endif
