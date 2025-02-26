package com.ironsource.unity.androidbridge;

import static com.ironsource.unity.androidbridge.AndroidBridgeConstants.*;
import android.app.Activity;
import android.util.DisplayMetrics;
import android.view.Display;
import android.view.WindowManager;
import com.unity3d.mediation.LevelPlayAdSize;
import com.unity3d.player.UnityPlayer;

public class BannerUtils {
   public static LevelPlayAdSize getAdSize(String description, int width, int height, int customWidth) {
      if (description.equalsIgnoreCase(BANNER_SIZE_CUSTOM)) {
         return LevelPlayAdSize.createCustomSize(width, height);
      }

      if (description.equalsIgnoreCase(BANNER_SIZE_BANNER)) {
         return LevelPlayAdSize.BANNER;
      }

      if (description.equalsIgnoreCase(BANNER_SIZE_MEDIUM_RECTANGLE)) {
         return LevelPlayAdSize.MEDIUM_RECTANGLE;
      }

      if (description.equalsIgnoreCase(BANNER_SIZE_LARGE)) {
         return LevelPlayAdSize.LARGE;
      }

      if (description.equalsIgnoreCase(BANNER_SIZE_LEADERBOARD)) {
         return LevelPlayAdSize.LEADERBOARD;
      }

      if (description.equalsIgnoreCase(BANNER_SIZE_ADAPTIVE)) {
         customWidth = (customWidth > 0) ? customWidth : (int)getDeviceScreenWidth();
         return LevelPlayAdSize.createAdaptiveAdSize(UnityPlayer.currentActivity, customWidth);
      }
      return null;
   }

   private static float getDeviceScreenWidth() {
      Activity activity = UnityPlayer.currentActivity;
      if (activity != null) {
         WindowManager windowManager = activity.getWindowManager();
         if (windowManager != null) {
            Display display = windowManager.getDefaultDisplay();
            if (display != null) {
               DisplayMetrics displayMetrics = new DisplayMetrics();
               display.getMetrics(displayMetrics);
               int widthPixels = displayMetrics.widthPixels;
               float density = displayMetrics.density;
               return widthPixels / density;
            }
         }
      }
      return 0;
   }
}
