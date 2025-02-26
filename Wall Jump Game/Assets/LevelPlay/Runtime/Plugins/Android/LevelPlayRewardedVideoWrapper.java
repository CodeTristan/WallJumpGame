package com.ironsource.unity.androidbridge;

import static com.ironsource.unity.androidbridge.AndroidBridgeUtilities.postBackgroundTask;

import com.ironsource.mediationsdk.IronSource;
import com.ironsource.mediationsdk.adunit.adapter.utility.AdInfo;
import com.ironsource.mediationsdk.logger.IronSourceError;
import com.ironsource.mediationsdk.model.Placement;
import com.ironsource.mediationsdk.sdk.LevelPlayRewardedVideoListener;
import com.ironsource.mediationsdk.sdk.LevelPlayRewardedVideoBaseListener;
import com.ironsource.mediationsdk.sdk.LevelPlayRewardedVideoManualListener;

class LevelPlayRewardedVideoWrapper implements LevelPlayRewardedVideoListener, LevelPlayRewardedVideoManualListener {
    private UnityLevelPlayRewardedVideoListener mUnityLevelPlayRewardedVideoListener;
    private UnityLevelPlayRewardedVideoManualListener mUnityLevelPlayManualRewardedVideoListener;

    public LevelPlayRewardedVideoWrapper() {
        IronSource.setLevelPlayRewardedVideoListener(this);
    }

    public void setIronSourceManualLoadListener(boolean isOn) {
        IronSource.setLevelPlayRewardedVideoManualListener((isOn) ? this : null);
    }
    // region Set level play rewarded video listeners

    public void setLevelPlayRewardedVideoListener(UnityLevelPlayRewardedVideoListener listener) {
        mUnityLevelPlayRewardedVideoListener = listener;
    }

    public void setLevelPlayManualRewardedVideoListener(UnityLevelPlayRewardedVideoManualListener listener) {
        mUnityLevelPlayManualRewardedVideoListener = listener;
    }

    //endregion

    // region Rewarded Video Automatic Callbacks

    @Override
    public void onAdAvailable(final AdInfo adInfo) {
         if (mUnityLevelPlayRewardedVideoListener != null) {
                postBackgroundTask(new Runnable() {
                    @Override
                    public void run() {
                      if (mUnityLevelPlayRewardedVideoListener != null) {
                        mUnityLevelPlayRewardedVideoListener.onAdAvailable(AndroidBridgeUtilities.getAdInfoString(adInfo));
                      }
                    }
                });
            }
        }

    @Override
    public void onAdUnavailable() {
         if (mUnityLevelPlayRewardedVideoListener != null) {
                postBackgroundTask(new Runnable() {
                    @Override
                    public void run() {
                      if (mUnityLevelPlayRewardedVideoListener != null) {
                        mUnityLevelPlayRewardedVideoListener.onAdUnavailable();
                      }
                    }
                });
            }
        }

    //endregion

    // region Rewarded Video  Callbacks

    @Override
    public void onAdOpened(final AdInfo adInfo) {

            if (mUnityLevelPlayRewardedVideoListener != null) {
                postBackgroundTask(new Runnable() {
                    @Override
                    public void run() {
                      if (mUnityLevelPlayRewardedVideoListener != null) {
                        mUnityLevelPlayRewardedVideoListener.onAdOpened(AndroidBridgeUtilities.getAdInfoString(adInfo));
                      }
                    }
                });
            }

    }

    @Override
    public void onAdShowFailed(final IronSourceError ironSourceError,final AdInfo adInfo) {
        if (mUnityLevelPlayRewardedVideoListener != null) {
                postBackgroundTask(new Runnable() {
                    @Override
                    public void run() {
                      if (mUnityLevelPlayRewardedVideoListener != null) {
                        mUnityLevelPlayRewardedVideoListener.onAdShowFailed(AndroidBridgeUtilities.parseIronSourceError(ironSourceError), AndroidBridgeUtilities.getAdInfoString(adInfo));
                      }
                    }
                });

            }
    }

    @Override
    public void onAdClicked(Placement placement, final AdInfo adInfo) {
        final String argsJson = AndroidBridgeUtilities.getPlacememtJson(placement);
        if (mUnityLevelPlayRewardedVideoListener != null) {
                postBackgroundTask(new Runnable() {
                    @Override
                    public void run() {
                      if (mUnityLevelPlayRewardedVideoListener != null) {
                        mUnityLevelPlayRewardedVideoListener.onAdClicked(argsJson, AndroidBridgeUtilities.getAdInfoString(adInfo));
                      }
                    }
                });
            }

    }

    @Override
    public void onAdRewarded(Placement placement,final AdInfo adInfo) {
        final String argsJson = AndroidBridgeUtilities.getPlacememtJson(placement);
         if (mUnityLevelPlayRewardedVideoListener != null) {
                postBackgroundTask(new Runnable() {
                    @Override
                    public void run() {
                      if (mUnityLevelPlayRewardedVideoListener != null) {
                        mUnityLevelPlayRewardedVideoListener.onAdRewarded(argsJson, AndroidBridgeUtilities.getAdInfoString(adInfo));
                      }
                    }
                });
            }
        }

    @Override
    public void onAdClosed(final AdInfo adInfo) {
         if (mUnityLevelPlayRewardedVideoListener != null) {
                postBackgroundTask(new Runnable() {
                    @Override
                    public void run() {
                      if (mUnityLevelPlayRewardedVideoListener != null) {
                        mUnityLevelPlayRewardedVideoListener.onAdClosed(AndroidBridgeUtilities.getAdInfoString(adInfo));
                      }
                    }
                });
            }

    }

    @Override
    public void onAdReady(final AdInfo adInfo) {
         if (mUnityLevelPlayRewardedVideoListener != null) {
                postBackgroundTask(new Runnable() {
                    @Override
                    public void run() {
                      if (mUnityLevelPlayRewardedVideoListener != null) {
                        mUnityLevelPlayManualRewardedVideoListener.onAdReady(AndroidBridgeUtilities.getAdInfoString(adInfo));
                      }
                    }
                });
            }

    }

    @Override
    public void onAdLoadFailed(final IronSourceError ironSourceError) {
         if (mUnityLevelPlayRewardedVideoListener != null) {
                postBackgroundTask(new Runnable() {
                    @Override
                    public void run() {
                      if (mUnityLevelPlayRewardedVideoListener != null) {
                        mUnityLevelPlayManualRewardedVideoListener.onAdLoadFailed(AndroidBridgeUtilities.parseIronSourceError(ironSourceError));
                      }
                    }
                });
            }
        }
    //endregion
}