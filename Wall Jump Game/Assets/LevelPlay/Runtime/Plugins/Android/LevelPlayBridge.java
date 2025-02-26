package com.ironsource.unity.androidbridge;

import static com.ironsource.unity.androidbridge.AndroidBridgeUtilities.postBackgroundTask;

import com.ironsource.mediationsdk.config.ConfigFile;
import com.unity3d.mediation.LevelPlay;
import com.unity3d.mediation.LevelPlay.AdFormat;
import com.unity3d.mediation.LevelPlayConfiguration;
import com.unity3d.mediation.LevelPlayInitError;
import  com.unity3d.mediation.LevelPlayInitListener;
import com.unity3d.mediation.LevelPlayInitRequest;
import com.unity3d.mediation.LevelPlayInitRequest.Builder;
import com.unity3d.player.UnityPlayer;
import java.security.KeyStore.TrustedCertificateEntry;
import java.util.ArrayList;
import java.util.List;


public class LevelPlayBridge implements LevelPlayInitListener {
	private IUnityLevelPlayInitListener mUnityLevelPlayInitListener;

	private static final LevelPlayBridge mInstance = new LevelPlayBridge();
	private LevelPlayBridge() {

	}

	public static synchronized LevelPlayBridge getInstance() {
		return mInstance;
	}
	public void initialize(String appKey, String userId, String[] adFormats, IUnityLevelPlayInitListener listener){
		List<LevelPlay.AdFormat> adFormatList = getAdFormatList(adFormats);

		Builder requestBuilder = new LevelPlayInitRequest.Builder(appKey);
		if (userId != null && userId != "") {
			requestBuilder.withUserId(userId);
		}

		if (adFormatList != null) {
			requestBuilder.withLegacyAdFormats(adFormatList);
		}
		LevelPlayInitRequest initRequest = requestBuilder.build();

		mUnityLevelPlayInitListener = listener;
		LevelPlay.init(UnityPlayer.currentActivity, initRequest, this);
	}

	public void setPluginData(String pluginType, String pluginVersion, String pluginFrameworkVersion) {
		ConfigFile.getConfigFile().setPluginData(pluginType, pluginVersion, pluginFrameworkVersion);
	}


	@Override
	public void onInitFailed(LevelPlayInitError initError) {
		if (mUnityLevelPlayInitListener != null) {
			postBackgroundTask(new Runnable() {
				@Override
				public void run() {
					mUnityLevelPlayInitListener.onInitFailed(LevelPlayUtils.initErrorToString(initError));
				}
			});
		}
	}

	@Override
	public void onInitSuccess(LevelPlayConfiguration configuration) {
			if (mUnityLevelPlayInitListener != null) {
				postBackgroundTask(new Runnable() {
					@Override
					public void run() {
						mUnityLevelPlayInitListener.onInitSuccess(LevelPlayUtils.configurationToString(configuration));
					}
				});
			}
	}

	private List<LevelPlay.AdFormat> getAdFormatList(String[] adFormats) {
		if(adFormats == null)
			return null;
		List<LevelPlay.AdFormat> adFormatList = new ArrayList<>();
		for (String adFormat : adFormats) {
			adFormatList.add(LevelPlay.AdFormat.valueOf(adFormat.toUpperCase()));
		}
		return adFormatList;
	}
}
