package com.ironsource.unity.androidbridge;

public interface UnityLevelPlayBannerListener {
    void onAdLoaded(String adInfo);

    void onAdLoadFailed(String Error);

    void onAdClicked(String adInfo);

    void onAdScreenPresented(String adInfo);

    void onAdScreenDismissed(String adInfo);

    void onAdLeftApplication(String adInfo);
}
