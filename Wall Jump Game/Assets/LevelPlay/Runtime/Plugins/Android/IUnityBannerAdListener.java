package com.ironsource.unity.androidbridge;

public interface IUnityBannerAdListener {
     void onAdLoaded(String adInfo);
     void onAdLoadFailed(String error);
     void onAdDisplayed(String adInfo);
     void onAdDisplayFailed(String adInfo, String error);
     void onAdClicked(String adInfo);
     void onAdExpanded(String adInfo);
     void onAdCollapsed(String adInfo);
     void onAdLeftApplication(String adInfo);
}
