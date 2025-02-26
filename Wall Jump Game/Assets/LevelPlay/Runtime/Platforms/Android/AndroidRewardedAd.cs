using System;
using UnityEngine;

namespace Unity.Services.LevelPlay
{
    class AndroidRewardedAd : IPlatformRewardedAd, IUnityRewardedAdListener
    {
        const string k_AndroidRewardedAdClass = "com.ironsource.unity.androidbridge.RewardedAd";
        const string k_AndroidLoadAdFunction = "loadAd";
        const string k_AndroidShowAdFunction = "showAd";
        const string k_IsAdReadyFunction = "isAdReady";
        const string k_IsPlacementCappedStaticFunction = "isPlacementCapped";
        const string k_FuncGetAdId       = "getAdId";

        const string k_ErrorDisposed = "Instance is disposed. Please create a new instance in order to call any method.";

#pragma warning disable 0618
        public event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdLoaded;
        public event Action<com.unity3d.mediation.LevelPlayAdError> OnAdLoadFailed;
        public event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdDisplayed;
        public event Action<com.unity3d.mediation.LevelPlayAdDisplayInfoError> OnAdDisplayFailed;
        public event Action<com.unity3d.mediation.LevelPlayAdInfo, com.unity3d.mediation.LevelPlayReward> OnAdRewarded;
        public event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdClicked;
        public event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdClosed;
        public event Action<com.unity3d.mediation.LevelPlayAdInfo> OnAdInfoChanged;
#pragma warning restore 0618

        AndroidJavaObject m_RewardedAdJavaObject;
        IUnityRewardedAdListener m_RewardedAdListener;

        volatile bool m_Disposed;
        volatile bool m_IsReady;

        public string AdUnitId { get; }

        public string AdId { get {return m_RewardedAdJavaObject.Call<string>(k_FuncGetAdId);} }

        internal AndroidRewardedAd(string adUnitId)
        {
            AdUnitId = adUnitId;
            ThreadUtil.Send(state =>
            {
                try
                {
                    if (m_RewardedAdListener == null)
                    {
                        m_RewardedAdListener =
                            new UnityRewardedAdListener(this);
                    }
                    m_RewardedAdJavaObject =
                        new AndroidJavaObject(k_AndroidRewardedAdClass, adUnitId, m_RewardedAdListener);
                }
                catch (Exception e)
                {
                    LevelPlayLogger.LogException(e);
                }
            });
        }

        public void LoadAd()
        {
            if (!CheckDisposedAndLogError())
            {
                ThreadUtil.Post(state =>
                {
                    try
                    {
                        m_RewardedAdJavaObject.Call(k_AndroidLoadAdFunction);
                    }
                    catch (Exception e)
                    {
                        LevelPlayLogger.LogException(e);
                    }
                });
            }
        }

        public void ShowAd(string placementName)
        {
            if (!CheckDisposedAndLogError())
            {
                ThreadUtil.Post(state =>
                {
                    try
                    {
                        m_RewardedAdJavaObject.Call(k_AndroidShowAdFunction, placementName);
                    }
                    catch (Exception e)
                    {
                        LevelPlayLogger.LogException(e);
                    }
                });
            }
        }

        public bool IsAdReady()
        {
            if (!CheckDisposedAndLogError())
            {
                ThreadUtil.Send(state =>
                {
                    try
                    {
                        m_IsReady = m_RewardedAdJavaObject.Call<bool>(k_IsAdReadyFunction);
                    }
                    catch (Exception e)
                    {
                        LevelPlayLogger.LogException(e);
                    }
                });
            }
            return m_IsReady;
        }

        public static bool IsPlacementCapped(string placementName)
        {
            var isPlacementCapped = false;
            try
            {
                using (var rewardedAdJavaClass = new AndroidJavaClass(k_AndroidRewardedAdClass))
                {
                    isPlacementCapped = rewardedAdJavaClass.CallStatic<bool>(k_IsPlacementCappedStaticFunction, placementName);
                }
            }
            catch (Exception e)
            {
                LevelPlayLogger.LogException(e);
            }
            return isPlacementCapped;
        }

#pragma warning disable 0618
        public void onAdLoaded(string adInfo)
        {
            OnAdLoaded?.Invoke(new com.unity3d.mediation.LevelPlayAdInfo(adInfo));
        }

        public void onAdLoadFailed(string error)
        {
            OnAdLoadFailed?.Invoke(new com.unity3d.mediation.LevelPlayAdError(error));
        }

        public void onAdDisplayed(string adInfo)
        {
            OnAdDisplayed?.Invoke(new com.unity3d.mediation.LevelPlayAdInfo(adInfo));
        }

        public void onAdDisplayFailed(string error, string adInfo)
        {
            OnAdDisplayFailed?.Invoke(new com.unity3d.mediation.LevelPlayAdDisplayInfoError(new LevelPlayAdInfo(adInfo), new LevelPlayAdError(error)));
        }

        public void onAdRewarded(string adInfo, string rewardName, int rewardAmount)
        {
            OnAdRewarded?.Invoke(new com.unity3d.mediation.LevelPlayAdInfo(adInfo), new com.unity3d.mediation.LevelPlayReward(rewardName, rewardAmount));
        }

        public void onAdClicked(string adInfo)
        {
            OnAdClicked?.Invoke(new com.unity3d.mediation.LevelPlayAdInfo(adInfo));
        }

        public void onAdClosed(string adInfo)
        {
            OnAdClosed?.Invoke(new com.unity3d.mediation.LevelPlayAdInfo(adInfo));
        }

        public void onAdInfoChanged(string adInfo)
        {
            OnAdInfoChanged?.Invoke(new com.unity3d.mediation.LevelPlayAdInfo(adInfo));
        }

#pragma warning restore 0618

        void Dispose(bool disposing)
        {
            if (m_Disposed) return;
            m_Disposed = true;
            if (disposing)
            {
                ThreadUtil.Post(state =>
                {
                    m_RewardedAdJavaObject?.Dispose();
                    m_IsReady = false;
                    m_RewardedAdListener = null;
                    m_RewardedAdJavaObject = null;
                });
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AndroidRewardedAd()
        {
            Dispose(false);
        }

        bool CheckDisposedAndLogError()
        {
            if (m_Disposed)
            {
                LevelPlayLogger.LogError(k_ErrorDisposed);
            }
            return m_Disposed;
        }
    }
}
