package com.ironsource.unity.androidbridge;

public interface UnityLevelPlayInterstitialListener {
    void onAdShowFailed(String error, String adInfo);

    void onAdLoadFailed(String error);

    void onAdReady(String adInfo);

    void onAdOpened(String adInfo);

    void onAdClosed(String adInfo);

    void onAdShowSucceeded(String adInfo);

    void onAdClicked(String adInfo);
}
