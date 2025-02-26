package com.ironsource.unity.androidbridge;

import static com.ironsource.unity.androidbridge.AndroidBridgeConstants.WATERFALL_CONFIG_CEILING_KEY;
import static com.ironsource.unity.androidbridge.AndroidBridgeConstants.WATERFALL_CONFIG_FLOOR_KEY;
import static com.ironsource.unity.androidbridge.AndroidBridgeUtilities.postBackgroundTask;

import android.app.Activity;
import android.graphics.Color;
import android.os.Handler;
import android.os.Looper;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.Gravity;
import android.view.ViewGroup;
import android.view.WindowManager;
import android.widget.FrameLayout;
import android.view.View;
import android.view.Display;

import com.ironsource.adapters.supersonicads.SupersonicConfig;
import com.ironsource.mediationsdk.ISBannerSize;
import com.ironsource.mediationsdk.ISContainerParams;
import com.ironsource.mediationsdk.IronSource;
import com.ironsource.mediationsdk.IronSourceBannerLayout;
import com.ironsource.mediationsdk.IronSourceSegment;
import com.ironsource.mediationsdk.WaterfallConfiguration;
import com.ironsource.mediationsdk.adunit.adapter.utility.AdInfo;
import com.ironsource.mediationsdk.config.ConfigFile;
import com.ironsource.mediationsdk.impressionData.ImpressionData;
import com.ironsource.mediationsdk.impressionData.ImpressionDataListener;
import com.ironsource.mediationsdk.integration.IntegrationHelper;
import com.ironsource.mediationsdk.logger.IronSourceError;
import com.ironsource.mediationsdk.model.Placement;
import com.ironsource.mediationsdk.sdk.LevelPlayBannerListener;
import com.ironsource.mediationsdk.sdk.SegmentListener;
import com.ironsource.mediationsdk.sdk.InitializationListener;

import com.unity3d.player.UnityPlayer;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.concurrent.Callable;
import java.util.concurrent.FutureTask;
import java.util.concurrent.TimeUnit;
import java.util.Arrays;

public class AndroidBridge implements InitializationListener, ImpressionDataListener, SegmentListener {
    private static final AndroidBridge mInstance = new AndroidBridge();
    private Handler mUIHandler;

    //Uses to determine whether loadBanner was called- prevents multiple calls
    private boolean mIsBannerLoadCalled;

    // Uses to determine whether onBannerAdLoaded was called,
    // prevents from removing banner view that was already loaded and reload failed
    private boolean mIsBannerLoadedFirst;

    private FrameLayout mBannerContainer;
    private IronSourceBannerLayout mBanner;
    private int mBannerVisibilityState;

    private UnitySegmentListener mUnitySegmentListener;
    private UnityImpressionDataListener mUnityImpressionDataListener;
    private UnityInitializationListener mUnityInitializationListener;
    private UnityLevelPlayBannerListener mUnityLevelPlayBannerListener;

    private LevelPlayRewardedVideoWrapper mLevelPlayRewardedVideoWrapper;
    private LevelPlayInterstitialWrapper mLevelPlayInterstitialWrapper;

    public static synchronized AndroidBridge getInstance() {
        return mInstance;
    }

    private AndroidBridge() {
        IronSource.addImpressionDataListener(this);
        IronSource.setSegmentListener(this);

        mLevelPlayRewardedVideoWrapper = new LevelPlayRewardedVideoWrapper();
        mLevelPlayInterstitialWrapper = new LevelPlayInterstitialWrapper();

        mUIHandler = new Handler(Looper.getMainLooper());
        mBannerContainer = null;
        mBanner = null;
        mIsBannerLoadedFirst = false;
        mIsBannerLoadCalled = false;
        mBannerVisibilityState = View.VISIBLE;
    }
    // region Set Java Listeners

    public void setUnityInitializationListener(UnityInitializationListener listener) {
        this.mUnityInitializationListener = listener;
    }

    public void setUnityRewardedVideoLevelPlayListener(UnityLevelPlayRewardedVideoListener listener) {
        mLevelPlayRewardedVideoWrapper.setLevelPlayRewardedVideoListener(listener);
    }

    public void setUnityRewardedVideoManualLevelPlayListener(UnityLevelPlayRewardedVideoManualListener listener) {
        mLevelPlayRewardedVideoWrapper.setLevelPlayManualRewardedVideoListener(listener);
    }

    public void setUnityInterstitialLevelPlayListener(UnityLevelPlayInterstitialListener listener) {
        mLevelPlayInterstitialWrapper.setInterstitialLevelPlaylistener(listener);
    }

    public void setUnitySegmentListener(UnitySegmentListener listener) {
        this.mUnitySegmentListener = listener;
    }

    public void setUnityBannerLevelPlayListener(UnityLevelPlayBannerListener listener) {
        this.mUnityLevelPlayBannerListener = listener;
    }

    public void setUnityImpressionDataListener(UnityImpressionDataListener listener) {
        this.mUnityImpressionDataListener = listener;
    }

    //endregion

    // region Impression Data Callbacks
    public void onImpressionSuccess(final ImpressionData impressionData) {
         if (mUnityImpressionDataListener != null) {
                postBackgroundTask(new Runnable() {
                    @Override
                    public void run() {
                        mUnityImpressionDataListener.onImpressionDataReady(AndroidBridgeUtilities.getImpressionDataString(impressionData)); // send callback on bg
                        mUnityImpressionDataListener.onImpressionSuccess(AndroidBridgeUtilities.getImpressionDataString(impressionData)); // send callback on ui
                    }
                });
            }
        }
    //endregion

    // region Init Callbacks
    @Override
    public void onInitializationComplete() {
        if (mUnityInitializationListener != null) {
            postBackgroundTask(new Runnable() {
                @Override
                public void run() {
                    mUnityInitializationListener.onSdkInitializationCompleted();
                }
            });
        }
    }
    //endregion

    // Segment Callbacks
    @Override
    public void onSegmentReceived(final String segment) {
        try {
            if (mUnitySegmentListener != null) {
                postBackgroundTask(new Runnable() {
                    @Override
                    public void run() {
                        mUnitySegmentListener.onSegmentRecieved(segment);
                    }
                });
            }
        } catch (Exception e) {
            e.printStackTrace();
        }

    }
    //endregion
//endregion

//region Base API

    public Activity getUnityActivity() {
        return UnityPlayer.currentActivity;
    }

    public void setPluginData(String pluginType, String pluginVersion, String pluginFrameworkVersion) {
        ConfigFile.getConfigFile().setPluginData(pluginType, pluginVersion, pluginFrameworkVersion);
    }

    public String getAdvertiserId() {
        String result = AndroidBridgeConstants.EMPTY_STRING;

        FutureTask < String > f = new FutureTask < > (new Callable < String > () {
            @Override
            public String call() throws Exception {
                return IronSource.getAdvertiserId(getUnityActivity());
            }
        });

        f.run();

        try {
            result = f.get(1, TimeUnit.SECONDS);
        } catch (Exception e) {
            e.printStackTrace();
        }

        return result;
    }

    public void validateIntegration() {
        IntegrationHelper.validateIntegration(getUnityActivity());
    }

    public void shouldTrackNetworkState(boolean track) {
        IronSource.shouldTrackNetworkState(getUnityActivity(), track);
    }

    public boolean setDynamicUserId(String dynamicUserId) {
        return IronSource.setDynamicUserId(dynamicUserId);
    }

    public void setAdaptersDebug(boolean enabled) {
        IronSource.setAdaptersDebug(enabled);
    }

    public void setManualLoadRewardedVideo(boolean isOn) {
        mLevelPlayRewardedVideoWrapper.setIronSourceManualLoadListener(isOn);
    }

    public void setNetworkData(String networkKey, String networkData) {
        if (networkKey != null && networkData != null) {
            try {
                JSONObject networkDataJson = new JSONObject(networkData);
                IronSource.setNetworkData(networkKey, networkDataJson);
            } catch (JSONException e) {
                e.printStackTrace();
            }
        }
    }

    public void onResume() {
        IronSource.onResume(getUnityActivity());
    }

    public void onPause() {
        IronSource.onPause(getUnityActivity());
    }

//endregion

    //region Init SDK
    public void setUserId(String userId) {
        IronSource.setUserId(userId);
    }

    public void init(String appKey) {
        IronSource.init(getUnityActivity(), appKey, this);
    }

    public void init(String appKey, String[] adUnits) {
        List < IronSource.AD_UNIT > adUnitsToInit = new ArrayList < > ();

        for (String adUnit: adUnits) {
            if (IronSource.AD_UNIT.REWARDED_VIDEO.toString().equalsIgnoreCase(adUnit)) {
                adUnitsToInit.add(IronSource.AD_UNIT.REWARDED_VIDEO);
            } else if (IronSource.AD_UNIT.INTERSTITIAL.toString().equalsIgnoreCase(adUnit)) {
                adUnitsToInit.add(IronSource.AD_UNIT.INTERSTITIAL);
            } else if (IronSource.AD_UNIT.BANNER.toString().equalsIgnoreCase(adUnit)) {
                adUnitsToInit.add(IronSource.AD_UNIT.BANNER);
            }
        }

        IronSource.init(getUnityActivity(), appKey, this, adUnitsToInit.toArray(new IronSource.AD_UNIT[adUnitsToInit.size()]));
    }
//endregion

//region RewardedVideo API

    public void loadRewardedVideo() {
        IronSource.loadRewardedVideo();
    }

    public void showRewardedVideo() {
        IronSource.showRewardedVideo();
    }

    public void showRewardedVideo(String placementName) {
        IronSource.showRewardedVideo(placementName);
    }

    public boolean isRewardedVideoAvailable() {
        return IronSource.isRewardedVideoAvailable();
    }

    public boolean isRewardedVideoPlacementCapped(String placementName) {
        return IronSource.isRewardedVideoPlacementCapped(placementName);
    }

    public String getPlacementInfo(String placementName) {
        String result = null;

        Placement p = IronSource.getRewardedVideoPlacementInfo(placementName);
        HashMap < String, Object > map = new HashMap < > ();
        try {
            map.put(AndroidBridgeConstants.PLACEMENT_NAME, p.getPlacementName());
            map.put(AndroidBridgeConstants.PLACEMENT_REWARD_NAME, p.getRewardName());
            map.put(AndroidBridgeConstants.PLACEMENT_AMOUNT, p.getRewardAmount());
            result = new JSONObject(map).toString();
        } catch (Exception e) {
            e.printStackTrace();
        }
        return result;
    }

    public void setRewardedVideoServerParams(String paramsJson) {
        Map < String, String > params = AndroidBridgeUtilities.getHashMapFromJsonString(paramsJson);
        IronSource.setRewardedVideoServerParameters(params);
    }

    public void clearRewardedVideoServerParams() {
        IronSource.clearRewardedVideoServerParameters();
    }

//endregion

//region Interstitial API

    public void loadInterstitial() {
        IronSource.loadInterstitial();
    }

    public void showInterstitial() {
        IronSource.showInterstitial();
    }

    public void showInterstitial(String placementName) {
        IronSource.showInterstitial(placementName);
    }

    public boolean isInterstitialReady() {
        return IronSource.isInterstitialReady();
    }

    public boolean isInterstitialPlacementCapped(String placementName) {
        return IronSource.isInterstitialPlacementCapped(placementName);
    }

//endregion

//region Banner API

    public void loadBanner(String description, int width, int height, int position, String placementName, boolean isAdaptive,boolean isRespectCutoutsEnabled, float containerWidth, float containerHeight) {
        synchronized(mInstance) {
            if (mIsBannerLoadCalled) {
                return;
            }

            mIsBannerLoadCalled = true;
            loadAndShowBanner(description, width, height, position, placementName, isAdaptive,isRespectCutoutsEnabled, containerWidth, containerHeight);
        }
    }

    private void loadAndShowBanner(final String description, final int width, final int height, final int position, final String placementName, final boolean isAdaptive, final boolean isRespectCutoutsEnabled, final float containerWidth, final float containerHeight) {
        mUIHandler.post(new Runnable() {
            @Override
            public void run() {
                synchronized(mInstance) {
                    try {
                        // create banner container
                        if (mBannerContainer == null) {
                            mBannerContainer = new FrameLayout(UnityPlayer.currentActivity);
                            mBannerContainer.setBackgroundColor(Color.TRANSPARENT);
                            mBannerContainer.setVisibility(mBannerVisibilityState);
                            FrameLayout.LayoutParams params = new FrameLayout.LayoutParams(FrameLayout.LayoutParams.MATCH_PARENT, FrameLayout.LayoutParams.WRAP_CONTENT);
                            // set banner position
                            params.gravity = (position == AndroidBridgeConstants.BANNER_POSITION_TOP) ? Gravity.TOP : Gravity.BOTTOM;
                            UnityPlayer.currentActivity.addContentView(mBannerContainer, params);
                        }

                        // create banner and container
                        ISBannerSize size = getBannerSize(description, width, height);

                        //Handle the new Adaptive Banner
                        if (isAdaptive){
                            size.setAdaptive(isAdaptive);
                            float widthx = containerWidth;
                            float heightx = containerHeight;

                            if (widthx <= 0) {
                                widthx = getDeviceScreenWidth();
                            }
                            if (heightx <= 0) {
                                heightx = getMaximalAdaptiveHeight(widthx);
                            }
                            ISContainerParams isContainerParams = new ISContainerParams((int)widthx, (int)heightx);
                            size.setContainerParams(isContainerParams);
                        }

                        //Handle the new Respect Android Cutouts
                        if (isRespectCutoutsEnabled){
                            if (android.os.Build.VERSION.SDK_INT >= 28) {
                                mBannerContainer.setFitsSystemWindows(true);
                                mBannerContainer.setSystemUiVisibility(View.SYSTEM_UI_FLAG_LAYOUT_STABLE | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN);
                            }
                        }


                        mBanner = IronSource.createBanner(getUnityActivity(), size);

                        // banner layout params
                        FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(FrameLayout.LayoutParams.MATCH_PARENT, FrameLayout.LayoutParams.WRAP_CONTENT);

                        // set banner position
                        layoutParams.gravity = (position == AndroidBridgeConstants.BANNER_POSITION_TOP) ? Gravity.TOP : Gravity.BOTTOM;

                        // add banner to container
                        if (mBannerContainer != null) {
                            mBannerContainer.addView(mBanner, layoutParams);
                        }

                        // adding hierarchy change listener to fix Unity issue of views changing visibility and not rendering the view
                        mBanner.setOnHierarchyChangeListener(new ViewGroup.OnHierarchyChangeListener() {
                            @Override
                            public void onChildViewAdded(View view, View view1) {
                                // we post on UI and change visibility according to the visibility state
                                mUIHandler.post(new Runnable() {
                                    @Override
                                    public void run() {
                                        synchronized(mInstance) {
                                            if (mBanner != null) {
                                                mBanner.setVisibility(mBannerVisibilityState);
                                            }

                                            // request layout
                                            if(mBannerContainer != null) {
                                                mBannerContainer.requestLayout();
                                            }
                                        }
                                    }
                                });
                            }

                            @Override
                            public void onChildViewRemoved(View view, View view1) {

                            }
                        });

                        // set banner listener
                        if (mUnityLevelPlayBannerListener != null) {
                            mBanner.setLevelPlayBannerListener(new LevelPlayBannerListener() {
                                @Override
                                public void onAdLoaded(final AdInfo adInfo) {
                                    mIsBannerLoadedFirst = true;

                                     if (mUnityLevelPlayBannerListener != null) {
                                            postBackgroundTask(new Runnable() {
                                                @Override
                                                public void run() {
                                                    if (mUnityLevelPlayBannerListener != null) {
                                                        mUnityLevelPlayBannerListener.onAdLoaded(AndroidBridgeUtilities.getAdInfoString(adInfo));
                                                    }
                                                }
                                            });
                                        }

                                }

                                @Override
                                public void onAdLoadFailed(final IronSourceError ironSourceError) {
                                    if (!mIsBannerLoadedFirst) {
                                        mUIHandler.post(new Runnable() {
                                            @Override
                                            public void run() {
                                                synchronized(mInstance) {
                                                    if(mBannerContainer != null) {
                                                        mBannerContainer.removeAllViews();
                                                    }
                                                    if (mBanner != null) {
                                                        mBanner = null;
                                                    }

                                                    mIsBannerLoadCalled = false;
                                                }
                                            }
                                        });
                                    }

                                        if (mUnityLevelPlayBannerListener != null) {
                                            postBackgroundTask(new Runnable() {
                                                @Override
                                                public void run() {
                                                    mUnityLevelPlayBannerListener.onAdLoadFailed(AndroidBridgeUtilities.parseIronSourceError(ironSourceError));
                                                }
                                            });
                                        }

                                }

                                @Override
                                public void onAdClicked(final AdInfo adInfo) {

                                        postBackgroundTask(new Runnable() {
                                            @Override
                                            public void run() {
                                                if (mUnityLevelPlayBannerListener != null) {
                                                    mUnityLevelPlayBannerListener.onAdClicked(AndroidBridgeUtilities.getAdInfoString(adInfo));
                                                }
                                            }
                                        });

                                }

                                @Override
                                public void onAdLeftApplication(final AdInfo adInfo) {

                                        postBackgroundTask(new Runnable() {
                                            @Override
                                            public void run() {
                                                if (mUnityLevelPlayBannerListener != null) {
                                                    mUnityLevelPlayBannerListener.onAdLeftApplication(AndroidBridgeUtilities.getAdInfoString(adInfo));
                                                }
                                            }
                                        });

                                }

                                @Override
                                public void onAdScreenPresented(final AdInfo adInfo) {
                                    postBackgroundTask(new Runnable() {
                                            @Override
                                            public void run() {
                                                if (mUnityLevelPlayBannerListener != null) {
                                                    mUnityLevelPlayBannerListener.onAdScreenPresented(AndroidBridgeUtilities.getAdInfoString(adInfo));
                                                }
                                            }
                                        });
                                }

                                @Override
                                public void onAdScreenDismissed(final AdInfo adInfo) {
                                     postBackgroundTask(new Runnable() {
                                            @Override
                                            public void run() {
                                                if (mUnityLevelPlayBannerListener != null) {
                                                    mUnityLevelPlayBannerListener.onAdScreenDismissed(AndroidBridgeUtilities.getAdInfoString(adInfo));
                                                }
                                            }
                                        });
                                }
                            });
                        }

                        if (placementName != null) {
                            IronSource.loadBanner(mBanner, placementName);
                        } else {
                            IronSource.loadBanner(mBanner);
                        }

                    } catch (Throwable ex) {
                        if (mUnityLevelPlayBannerListener != null) {
                            mUnityLevelPlayBannerListener.onAdLoadFailed(AndroidBridgeUtilities.parseErrorToEvent(IronSourceError.ERROR_CODE_NO_ADS_TO_SHOW, ex.getMessage()));
                        }
                    }
                }
            }
        });
    }

    private ISBannerSize getBannerSize(String description, int width, int height) {
        if (description.equals(AndroidBridgeConstants.BANNER_SIZE_CUSTOM)) {
            return new ISBannerSize(width, height);
        }

        if (description.equals(AndroidBridgeConstants.BANNER_SIZE_SMART)) {
            return ISBannerSize.SMART;
        }

        if (description.equals(AndroidBridgeConstants.BANNER_SIZE_RECTANGLE)) {
            return ISBannerSize.RECTANGLE;
        }

        if (description.equals(AndroidBridgeConstants.BANNER_SIZE_LARGE)) {
            return ISBannerSize.LARGE;
        } else {
            return ISBannerSize.BANNER;
        }
    }

    public void destroyBanner() {
        synchronized(mInstance) {
            mUIHandler.post(new Runnable() {
                @Override
                public void run() {
                    synchronized(mInstance) {
                        try {
                            if (mBannerContainer != null) {
                                mBannerContainer.removeAllViews();
                            }
                            if (mBanner != null) {
                                IronSource.destroyBanner(mBanner);
                                mBanner = null;
                                mBannerVisibilityState = View.VISIBLE;
                            }

                            if (mBannerContainer != null) {
                                mBannerContainer.setVisibility(View.VISIBLE);
                                mBannerContainer = null;
                            }

                        } catch (Exception ex) {

                        }

                        mIsBannerLoadCalled = false;
                        mIsBannerLoadedFirst = false;
                    }
                }
            });
        }
    }

    public void displayBanner() {
        synchronized(mInstance) {
            mUIHandler.post(new Runnable() {
                @Override
                public void run() {
                    synchronized(mInstance) {
                        try {
                            mBannerVisibilityState = View.VISIBLE;
                            if (mBannerContainer != null) {
                                mBanner.setVisibility(View.VISIBLE);
                                mBannerContainer.setVisibility(View.VISIBLE);
                            }
                        } catch (Exception ex) {

                        }
                    }
                }
            });
        }
    }

    public void hideBanner() {
        synchronized(mInstance) {
            mUIHandler.post(new Runnable() {
                @Override
                public void run() {
                    synchronized(mInstance) {
                        try {
                            mBannerVisibilityState = View.GONE;
                            if (mBannerContainer != null) {
                                mBanner.setVisibility(View.GONE);
                                mBannerContainer.setVisibility(View.GONE);
                            }
                        } catch (Exception ex) {

                        }
                    }
                }
            });
        }
    }

    public boolean isBannerPlacementCapped(String placementName) {
        return IronSource.isBannerPlacementCapped(placementName);
    }

    public float getMaximalAdaptiveHeight(float width) {
        int widthInt = (int)width;
        return ISBannerSize.getMaximalAdaptiveHeight(widthInt);
    }

    public float getDeviceScreenWidth() {
        Activity activity = getUnityActivity();
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


//endregion

//region Mediation Setters

    public void setSegment(String segmentJson) {
        try {
            IronSource.setSegmentListener((SegmentListener) this);
            JSONObject json = new JSONObject(segmentJson);
            IronSourceSegment ironSegment = new IronSourceSegment();

            Iterator < String > iter = json.keys();
            while (iter.hasNext()) {
                String key = iter.next();
                if (key.equals(AndroidBridgeConstants.SEGMENT_AGE)) {
                    ironSegment.setAge(json.optInt(key));
                } else if (key.equals(AndroidBridgeConstants.SEGMENT_GENDER)) {
                    ironSegment.setGender(json.optString(key));
                } else if (key.equals(AndroidBridgeConstants.SEGMENT_LEVEL)) {
                    ironSegment.setLevel(json.optInt(key));
                } else if (key.equals(AndroidBridgeConstants.SEGMENT_PAYING)) {
                    ironSegment.setIsPaying(json.optInt(key) != 0);
                } else if (key.equals(AndroidBridgeConstants.SEGMENT_CREATION_DATE)) {
                    ironSegment.setUserCreationDate(json.optLong(key));
                } else if (key.equals(AndroidBridgeConstants.SEGMENT_NAME)) {
                    ironSegment.setSegmentName(json.optString(key));
                } else if (key.equals(AndroidBridgeConstants.SEGMENT_IAPT))
                    ironSegment.setIAPTotal(json.optDouble(key));
                else // customs
                {
                    ironSegment.setCustom(key, json.optString(key));
                }
            }
            IronSource.setSegment(ironSegment);

        } catch (Exception e) {
            e.printStackTrace();
        }

    }

    public void setConsent(boolean consent) {
        IronSource.setConsent(consent);
    }

    public void setMetaData(String key, String value) {
        IronSource.setMetaData(key, value);
    }

    public void setMetaData(String key, String[] values) {
        List < String > valuesList = new ArrayList < > ();
        for (String value: values) {
            valuesList.add(value);
        }
        IronSource.setMetaData(key, valuesList);
    }

    public void setClientSideCallbacks(boolean status) {
        SupersonicConfig.getConfigObj().setClientSideCallbacks(status);
    }

    public void setRewardedVideoCustomParams(String paramsJson) {
    }

    public void setLanguage(String language) {
    }

    /**
     * Sets the waterfall configuration for a specific ad unit.
     *
     * @param configurationParams The configuration parameters as a string.
     * @param adUnit The ad unit identifier.
     */
    public void setWaterfallConfiguration(String configurationParams, String adUnit) {
        try {
            WaterfallConfiguration.WaterfallConfigurationBuilder builder =
                    WaterfallConfiguration.builder();

            JSONObject jsonConfig = new JSONObject(configurationParams);

            if (jsonConfig.has(WATERFALL_CONFIG_CEILING_KEY)) {
                double ceiling = jsonConfig.getDouble(WATERFALL_CONFIG_CEILING_KEY);
                builder.setCeiling(ceiling);
            }

            if (jsonConfig.has(WATERFALL_CONFIG_FLOOR_KEY)) {
                double floor = jsonConfig.getDouble(WATERFALL_CONFIG_FLOOR_KEY);
                builder.setFloor(floor);
            }

            WaterfallConfiguration waterfallConfiguration = builder.build();

            IronSource.setWaterfallConfiguration(waterfallConfiguration, IronSource.AD_UNIT.valueOf(adUnit));
        } catch (JSONException e) {
            String logMessage = String.format("Internal exception occurred while parsing "
                    + "configuration parameters for ad unit: %s. Please check the format "
                    + "of the configuration parameters.", adUnit);
            Log.e("LevelPlayAndroidBridge", logMessage);
        }
    }

//endregion

//region ILRD

    public void setAdRevenueData(String dataSource, String paramsJson) {
        HashMap < String, String > impressionData = AndroidBridgeUtilities.getHashMapFromJsonString(paramsJson);
        JSONObject argsJson = new JSONObject(impressionData);
        IronSource.setAdRevenueData(dataSource, argsJson);
    }

//endregion

//region TestSuite

    public void launchTestSuite() {
        IronSource.launchTestSuite(getUnityActivity());
    }

//endregion

}