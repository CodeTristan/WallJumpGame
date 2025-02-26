package com.ironsource.unity.androidbridge;

import android.app.Activity;
import android.graphics.Color;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.view.ViewGroup.OnHierarchyChangeListener;
import android.widget.FrameLayout;
import android.widget.FrameLayout.LayoutParams;
import com.unity3d.mediation.LevelPlayAdError;
import com.unity3d.mediation.LevelPlayAdInfo;
import com.unity3d.mediation.LevelPlayAdSize;
import com.unity3d.mediation.banner.LevelPlayBannerAdView;
import com.unity3d.mediation.banner.LevelPlayBannerAdViewListener;
import com.unity3d.player.UnityPlayer;

public class BannerAd {
    Activity mActivity;
    LevelPlayBannerAdView mBannerAdView;
    int mBannerAdViewVisibilityState = View.INVISIBLE;

   public BannerAd(String adUnitId, String sizeDescription, int sizeWidth, int sizeHeight, int customWidth, int position, String placementName, boolean displayOnLoad, boolean respectSafeArea, IUnityBannerAdListener bannerListener) {
        this.mActivity = UnityPlayer.currentActivity;

        this.mBannerAdView = new LevelPlayBannerAdView(mActivity, adUnitId);

        LevelPlayAdSize size = BannerUtils.getAdSize(sizeDescription,sizeWidth, sizeHeight, customWidth);
        if (size != null) {
          this.mBannerAdView.setAdSize(size);
        }

        if (placementName != null && placementName != "") {
          this.mBannerAdView.setPlacementName(placementName);
        }

        this.mBannerAdView.setBackgroundColor(Color.TRANSPARENT);

        if(displayOnLoad) {
          mBannerAdView.setVisibility(View.VISIBLE);
          mBannerAdViewVisibilityState = View.VISIBLE;
        } else{
          mBannerAdView.setVisibility(View.GONE);
          mBannerAdViewVisibilityState = View.GONE;
        }

       if (respectSafeArea){
           if (android.os.Build.VERSION.SDK_INT >= 28) {
               mBannerAdView.setFitsSystemWindows(true);
               mBannerAdView.setSystemUiVisibility(View.SYSTEM_UI_FLAG_LAYOUT_STABLE | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN);
               mBannerAdView.setOnApplyWindowInsetsListener((v, insets) -> {
                   if(insets != null) {
                       mBannerAdView.setPadding(insets.getSystemWindowInsetLeft(),
                               insets.getSystemWindowInsetTop(),
                               insets.getSystemWindowInsetRight(),
                               insets.getSystemWindowInsetBottom());
                   }
                   return insets;
               });
           }
       }

        setPosition(position);

        this.mBannerAdView.setBannerListener(new LevelPlayBannerAdViewListener() {
          @Override
          public void onAdLoaded(LevelPlayAdInfo levelPlayAdInfo) {
            if(bannerListener != null)
              bannerListener.onAdLoaded(LevelPlayUtils.adInfoToString(levelPlayAdInfo));
          }

          @Override
          public void onAdLoadFailed(LevelPlayAdError adError) {
            if(bannerListener != null)
              bannerListener.onAdLoadFailed(LevelPlayUtils.adErrorToString(adError));
          }

          @Override
          public void onAdDisplayed(LevelPlayAdInfo levelPlayAdInfo) {
            if(bannerListener != null)
              bannerListener.onAdDisplayed(LevelPlayUtils.adInfoToString(levelPlayAdInfo));
          }

          @Override
          public void onAdDisplayFailed(LevelPlayAdInfo levelPlayAdInfo,
              LevelPlayAdError adError) {
            if(bannerListener != null)  
              bannerListener.onAdDisplayFailed(LevelPlayUtils.adInfoToString(levelPlayAdInfo), LevelPlayUtils.adErrorToString(adError));
          }

          @Override
          public void onAdClicked(LevelPlayAdInfo levelPlayAdInfo) {
            if(bannerListener != null)
              bannerListener.onAdClicked(LevelPlayUtils.adInfoToString(levelPlayAdInfo));
          }

          @Override
          public void onAdExpanded(LevelPlayAdInfo levelPlayAdInfo) {
            if(bannerListener != null)
              bannerListener.onAdExpanded(LevelPlayUtils.adInfoToString(levelPlayAdInfo));
          }

          @Override
          public void onAdCollapsed(LevelPlayAdInfo levelPlayAdInfo) {
            if(bannerListener != null)
              bannerListener.onAdCollapsed(LevelPlayUtils.adInfoToString(levelPlayAdInfo));
          }

          @Override
          public void onAdLeftApplication(LevelPlayAdInfo levelPlayAdInfo) {
            if(bannerListener != null)
              bannerListener.onAdLeftApplication(LevelPlayUtils.adInfoToString(levelPlayAdInfo));
          }
        });
    }

    public void load() {
        this.mBannerAdView.loadAd();
    }

    public void destroy() {
        this.mBannerAdView.destroy();
    }

    public void showAd() {
      mActivity.runOnUiThread(new Runnable() {
        @Override
        public void run() {
          if (mBannerAdView != null) {
            mBannerAdView.setVisibility(View.VISIBLE);
          }
          mBannerAdViewVisibilityState = View.VISIBLE;
        }
      });
    }

    public void hideAd() {
      mActivity.runOnUiThread(new Runnable() {
        @Override
        public void run() {
          if (mBannerAdView != null) {
            mBannerAdView.setVisibility(View.GONE);
          }
          mBannerAdViewVisibilityState = View.GONE;
        }
      });
    }

    public void resumeAutoRefresh() {
        this.mBannerAdView.resumeAutoRefresh();
    }

    public void pauseAutoRefresh() {
        this.mBannerAdView.pauseAutoRefresh();
    }

    public String getAdId() {
        return this.mBannerAdView.getAdId();
    }

    private void setPosition(int position) {
      mActivity.runOnUiThread(new Runnable() {
        @Override
        public void run() {
          if (mBannerAdView.getParent() == null) {
            mActivity.addContentView(mBannerAdView, new LayoutParams(
                ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.WRAP_CONTENT));
          }
          setPositionInternal(position, 0, 0);

          mBannerAdView.setOnHierarchyChangeListener(new OnHierarchyChangeListener() {
            @Override
            public void onChildViewAdded(View parent, View child) {
              mActivity.runOnUiThread(new Runnable() {
                @Override
                public void run() {
                  if(mBannerAdView != null) {
                    mBannerAdView.setVisibility(mBannerAdViewVisibilityState);
                  }
                  mBannerAdView.requestLayout();
                }
              });
            }

            @Override
            public void onChildViewRemoved(View parent, View child) {

            }
          });
        }
      });
    }

    private void setPositionInternal (int position, int offsetX, int offsetY) {
      FrameLayout.LayoutParams adLayoutParams = (FrameLayout.LayoutParams) mBannerAdView.getLayoutParams();
      if(adLayoutParams == null) return;

      adLayoutParams.gravity = (position == AndroidBridgeConstants.BANNER_POSITION_TOP) ? Gravity.TOP : Gravity.BOTTOM;;

      mBannerAdView.setLayoutParams(adLayoutParams);
    }
}
