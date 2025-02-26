package com.ironsource.unity.androidbridge;

import static com.ironsource.unity.androidbridge.AndroidBridgeUtilities.postBackgroundTask;

import com.ironsource.mediationsdk.IronSource;
import com.ironsource.mediationsdk.adunit.adapter.utility.AdInfo;
import com.ironsource.mediationsdk.logger.IronSourceError;
import com.ironsource.mediationsdk.sdk.LevelPlayInterstitialListener;

class LevelPlayInterstitialWrapper implements LevelPlayInterstitialListener {
    private UnityLevelPlayInterstitialListener mUnityLevelPlayInterstitialListener;

    public LevelPlayInterstitialWrapper() {
        IronSource.setLevelPlayInterstitialListener(this);
    }

    public void setInterstitialLevelPlaylistener(UnityLevelPlayInterstitialListener listener) {
        mUnityLevelPlayInterstitialListener = listener;
    }

    // region Interstitial callbacks
    @Override
    public void onAdReady(final AdInfo adInfo) {
         if (mUnityLevelPlayInterstitialListener != null) {
                postBackgroundTask(new Runnable() {
                    @Override
                    public void run() {
                        mUnityLevelPlayInterstitialListener.onAdReady(AndroidBridgeUtilities.getAdInfoString(adInfo));
                    }
                });
            }
        }

    @Override
    public void onAdLoadFailed(final IronSourceError ironSourceError) {
         if (mUnityLevelPlayInterstitialListener != null) {
                postBackgroundTask(new Runnable() {
                    @Override
                    public void run() {
                        mUnityLevelPlayInterstitialListener.onAdLoadFailed(AndroidBridgeUtilities.parseIronSourceError(ironSourceError));
                    }
                });
            }
    }

    @Override
    public void onAdOpened(final AdInfo adInfo) {
         if (mUnityLevelPlayInterstitialListener != null) {
                postBackgroundTask(new Runnable() {
                    @Override
                    public void run() {
                        mUnityLevelPlayInterstitialListener.onAdOpened(AndroidBridgeUtilities.getAdInfoString(adInfo));
                    }
                });
            }
    }

    @Override
    public void onAdShowSucceeded(final AdInfo adInfo) {
         if (mUnityLevelPlayInterstitialListener != null) {
                postBackgroundTask(new Runnable() {
                    @Override
                    public void run() {
                        mUnityLevelPlayInterstitialListener.onAdShowSucceeded(AndroidBridgeUtilities.getAdInfoString(adInfo));
                    }
                });
            }
        }

    @Override
    public void onAdShowFailed(final IronSourceError ironSourceError,final  AdInfo adInfo) {
         if (mUnityLevelPlayInterstitialListener != null) {
                postBackgroundTask(new Runnable() {
                    @Override
                    public void run() {
                        mUnityLevelPlayInterstitialListener.onAdShowFailed(AndroidBridgeUtilities.parseIronSourceError(ironSourceError), AndroidBridgeUtilities.getAdInfoString(adInfo));
                    }
                });
            }
        }

    @Override
    public void onAdClicked(final AdInfo adInfo) {
         if (mUnityLevelPlayInterstitialListener != null) {
                postBackgroundTask(new Runnable() {
                    @Override
                    public void run() {
                        mUnityLevelPlayInterstitialListener.onAdClicked(AndroidBridgeUtilities.getAdInfoString(adInfo));
                    }
                });
            }
        }

    @Override
    public void onAdClosed(final AdInfo adInfo) {
         if (mUnityLevelPlayInterstitialListener != null) {
                postBackgroundTask(new Runnable() {
                    @Override
                    public void run() {
                        mUnityLevelPlayInterstitialListener.onAdClosed(AndroidBridgeUtilities.getAdInfoString(adInfo));
                    }
                });
            }

        }
    //endregion
}