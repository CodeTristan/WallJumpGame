package com.ironsource.unity.androidbridge;

import com.ironsource.mediationsdk.adunit.adapter.utility.AdInfo;
import com.ironsource.mediationsdk.impressionData.ImpressionData;
import com.ironsource.mediationsdk.logger.IronSourceError;
import com.ironsource.mediationsdk.model.Placement;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.HashMap;
import java.util.Iterator;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class AndroidBridgeUtilities {
    private static final ExecutorService callbackExecutor = Executors.newSingleThreadExecutor();

    public static HashMap<String, String> getHashMapFromJsonString(String jsonStr) {
        HashMap<String, String> map = new HashMap<>();

        try {
            JSONObject json = new JSONObject(jsonStr);

            Iterator<String> iter = json.keys();

            while (iter.hasNext()) {
                String key = iter.next();
                map.put(key, json.getString(key));
            }

        } catch (JSONException e) {
            e.printStackTrace();
        }

        return map;
    }

    public static String parseErrorToEvent(int code, String msg) {
        HashMap<String, Object> map = new HashMap<>();
        String result = AndroidBridgeConstants.EMPTY_STRING;
        try {
            map.put(AndroidBridgeConstants.ERROR_CODE, String.valueOf(code));
            map.put(AndroidBridgeConstants.ERROR_DESCRIPTION, msg);
            result = new JSONObject(map).toString();

        } catch (Exception e) {
            e.printStackTrace();
        }

        return result;
    }

    public static String parseIronSourceError(IronSourceError ironSourceError) {
        String params = AndroidBridgeConstants.EMPTY_STRING;
        if (ironSourceError != null) {
            int errorCode = ironSourceError.getErrorCode();
            String errorDescription = ironSourceError.getErrorMessage();
            params = parseErrorToEvent(errorCode, errorDescription);
        }
        return params;
    }

    public static String getPlacememtJson(Placement placement) {
        HashMap<String, Object> map = new HashMap();
        try {
            map.put(AndroidBridgeConstants.PLACEMENT_ID, String.valueOf(placement.getPlacementId()));
            map.put(AndroidBridgeConstants.PLACEMENT_NAME, placement.getPlacementName());
            map.put(AndroidBridgeConstants.PLACEMENT_REWARDED_AMOUNT, String.valueOf(placement.getRewardAmount()));
            map.put(AndroidBridgeConstants.PLACEMENT_REWARDED_NAME, placement.getRewardName());
            String argsJson = new JSONObject(map).toString();
            return argsJson;

        } catch (Exception e) {
            e.printStackTrace();
            return AndroidBridgeConstants.EMPTY_STRING;
        }
    }

    public static String getAdInfoString(AdInfo adInfo) {
        return (adInfo == null) ? AndroidBridgeConstants.EMPTY_STRING : adInfo.toString();
    }

    public static String getImpressionDataString(ImpressionData impressionData){
        return (impressionData == null) ? AndroidBridgeConstants.EMPTY_STRING : impressionData.getAllData().toString();
    }
    public static void postBackgroundTask(Runnable runnable) {
        if (callbackExecutor.isShutdown()) {
            return;
        }
        callbackExecutor.submit(runnable);

    }
}