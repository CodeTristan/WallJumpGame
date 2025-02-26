package com.ironsource.unity.androidbridge;

import android.app.Activity;
import com.unity3d.mediation.LevelPlayAdError;
import com.unity3d.mediation.LevelPlayAdInfo;
import com.unity3d.mediation.interstitial.LevelPlayInterstitialAd;
import com.unity3d.mediation.interstitial.LevelPlayInterstitialAdListener;
import com.unity3d.player.UnityPlayer;

public class InterstitialAd {
      Activity mActivity;
      LevelPlayInterstitialAd mInterstitialAd;

      public InterstitialAd(String adUnitId, IUnityInterstitialAdListener interstitialAdListener) {
         this.mActivity = UnityPlayer.currentActivity;

         this.mInterstitialAd = new LevelPlayInterstitialAd(adUnitId);
         this.mInterstitialAd.setListener(new LevelPlayInterstitialAdListener() {
           @Override
           public void onAdLoaded(LevelPlayAdInfo levelPlayAdInfo) {
             if (interstitialAdListener != null) {
               interstitialAdListener.onAdLoaded(LevelPlayUtils.adInfoToString(levelPlayAdInfo));
             }
           }

           @Override
           public void onAdLoadFailed(LevelPlayAdError levelPlayAdError) {
             if (interstitialAdListener != null) {
               interstitialAdListener.onAdLoadFailed(LevelPlayUtils.adErrorToString(levelPlayAdError));
             }
           }

           @Override
           public void onAdDisplayed(LevelPlayAdInfo levelPlayAdInfo) {
             if (interstitialAdListener != null) {
                interstitialAdListener.onAdDisplayed(LevelPlayUtils.adInfoToString(levelPlayAdInfo));
             }
           }

           @Override
           public void onAdClosed(LevelPlayAdInfo levelPlayAdInfo) {
              if (interstitialAdListener != null) {
                interstitialAdListener.onAdClosed(LevelPlayUtils.adInfoToString(levelPlayAdInfo));
              }
           }

           @Override
           public void onAdClicked(LevelPlayAdInfo levelPlayAdInfo) {
              if (interstitialAdListener != null) {
                interstitialAdListener.onAdClicked(LevelPlayUtils.adInfoToString(levelPlayAdInfo));
              }
           }

           @Override
           public void onAdDisplayFailed(LevelPlayAdError levelPlayAdError, LevelPlayAdInfo levelPlayAdInfo) {
             if (interstitialAdListener != null) {
               interstitialAdListener.onAdDisplayFailed(LevelPlayUtils.adErrorToString(levelPlayAdError),
                   LevelPlayUtils.adInfoToString(levelPlayAdInfo));
             }
           }

           @Override
           public void onAdInfoChanged(LevelPlayAdInfo levelPlayAdInfo) {
             if (interstitialAdListener != null) {
               interstitialAdListener.onAdInfoChanged(LevelPlayUtils.adInfoToString(levelPlayAdInfo));
             }
           }
         });
      }

      public void loadAd() {
        this.mInterstitialAd.loadAd();
      }

      public void showAd(String placementName) {
        this.mInterstitialAd.showAd(mActivity, placementName);
      }

      public boolean isAdReady() {
        return this.mInterstitialAd.isAdReady();
      }

      public static boolean isPlacementCapped(String placementName) {
          return LevelPlayInterstitialAd.isPlacementCapped(placementName);
      }

      public String getAdId() {
          return this.mInterstitialAd.getAdId();
      }
}
