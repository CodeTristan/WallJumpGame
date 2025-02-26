package com.ironsource.unity.androidbridge;

import android.app.Activity;
import com.unity3d.mediation.LevelPlayAdError;
import com.unity3d.mediation.LevelPlayAdInfo;
import com.unity3d.mediation.rewarded.LevelPlayReward;
import com.unity3d.mediation.rewarded.LevelPlayRewardedAd;
import com.unity3d.mediation.rewarded.LevelPlayRewardedAdListener;
import com.unity3d.player.UnityPlayer;

public class RewardedAd {
   Activity mActivity;

   LevelPlayRewardedAd mRewardedAd;

   public RewardedAd(String adUnitId, IUnityRewardedAdListener rewardedAdListener) {
      this.mActivity = UnityPlayer.currentActivity;

      this.mRewardedAd = new LevelPlayRewardedAd(adUnitId);
      this.mRewardedAd.setListener(new LevelPlayRewardedAdListener() {
         @Override
         public void onAdLoaded(LevelPlayAdInfo levelPlayAdInfo) {
            if (rewardedAdListener != null) {
               rewardedAdListener.onAdLoaded(LevelPlayUtils.adInfoToString(levelPlayAdInfo));
            }
         }

         @Override
         public void onAdLoadFailed(LevelPlayAdError levelPlayAdError) {
            if (rewardedAdListener != null) {
               rewardedAdListener.onAdLoadFailed(LevelPlayUtils.adErrorToString(levelPlayAdError));
            }
         }

         @Override
         public void onAdDisplayed(LevelPlayAdInfo levelPlayAdInfo) {
             if (rewardedAdListener != null) {
                rewardedAdListener.onAdDisplayed(LevelPlayUtils.adInfoToString(levelPlayAdInfo));
             }
         }

         @Override
         public void onAdRewarded(LevelPlayReward levelPlayReward,
             LevelPlayAdInfo levelPlayAdInfo) {
            if (rewardedAdListener != null) {
               rewardedAdListener.onAdRewarded(LevelPlayUtils.adInfoToString(levelPlayAdInfo), levelPlayReward.getName(), levelPlayReward.getAmount());
            }
         }

         @Override
         public void onAdDisplayFailed(LevelPlayAdError levelPlayAdError, LevelPlayAdInfo levelPlayAdInfo) {
            if (rewardedAdListener != null) {
               rewardedAdListener.onAdDisplayFailed(LevelPlayUtils.adErrorToString(levelPlayAdError), LevelPlayUtils.adInfoToString(levelPlayAdInfo));
            }
         }

         @Override
         public void onAdClosed(LevelPlayAdInfo levelPlayAdInfo) {
            if (rewardedAdListener != null) {
               rewardedAdListener.onAdClosed(LevelPlayUtils.adInfoToString(levelPlayAdInfo));
            }
         }

         @Override
         public void onAdInfoChanged(LevelPlayAdInfo levelPlayAdInfo) {
            if (rewardedAdListener != null) {
               rewardedAdListener.onAdInfoChanged(LevelPlayUtils.adInfoToString(levelPlayAdInfo));
            }
         }

         @Override
         public void onAdClicked(LevelPlayAdInfo levelPlayAdInfo) {
            if (rewardedAdListener != null) {
               rewardedAdListener.onAdClicked(LevelPlayUtils.adInfoToString(levelPlayAdInfo));
            }
         }
      });
   }

   public void loadAd(){
      this.mRewardedAd.loadAd();
   }

   public void showAd(String placementName) {
      this.mRewardedAd.showAd(mActivity, placementName);
   }

   public boolean isAdReady() {
      return this.mRewardedAd.isAdReady();
   }

   public static boolean isPlacementCapped(String placementName) {
      return LevelPlayRewardedAd.isPlacementCapped(placementName);
   }

   public String getAdId() {
      return this.mRewardedAd.getAdId();
   }
}
