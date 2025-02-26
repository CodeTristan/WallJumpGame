package com.ironsource.unity.androidbridge;

interface IUnityRewardedAdListener {
 void onAdLoaded(String adInfo);
 void onAdLoadFailed(String error);
 void onAdDisplayed(String adInfo);
 void onAdDisplayFailed(String error, String adInfo);
 void onAdRewarded(String adInfo, String name, int amount);
 void onAdClosed(String adInfo);
 void onAdClicked(String adInfo);
 void onAdInfoChanged(String adInfo);
}
