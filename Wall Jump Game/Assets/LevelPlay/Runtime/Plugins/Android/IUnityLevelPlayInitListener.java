package com.ironsource.unity.androidbridge;

import com.unity3d.mediation.LevelPlayConfiguration;
import com.unity3d.mediation.LevelPlayInitError;

interface IUnityLevelPlayInitListener {
   void onInitSuccess(String configuration);
   void onInitFailed(String initError);
}
