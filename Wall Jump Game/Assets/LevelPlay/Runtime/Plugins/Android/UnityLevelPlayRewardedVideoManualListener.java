package com.ironsource.unity.androidbridge;

public interface UnityLevelPlayRewardedVideoManualListener {
    void onAdReady(String adInfo);

    void onAdLoadFailed(String error);
}
