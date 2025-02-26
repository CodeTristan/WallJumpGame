package com.ironsource.unity.androidbridge;

import com.ironsource.mediationsdk.adunit.adapter.utility.AdInfo;

public interface UnityLevelPlayRewardedVideoListener {
    void onAdOpened(String adInfo);

    void onAdClosed(String adInfo);

    void onAdRewarded(String placement, String adInfo);

    void onAdShowFailed(String error, String adInfo);

    void onAdClicked(String placement, String adInfo);

    void onAdAvailable(String adInfo);

    void onAdUnavailable();
}
