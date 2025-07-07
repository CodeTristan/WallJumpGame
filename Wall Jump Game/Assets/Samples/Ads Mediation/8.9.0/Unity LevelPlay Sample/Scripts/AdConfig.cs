public static class AdConfig
{
    public static string AppKey => GetAppKey();
    public static string BannerAdUnitId => GetBannerAdUnitId();
    public static string InterstitalAdUnitId => GetInterstitialAdUnitId();
    public static string RewardedVideoAdUnitId => GetRewardedVideoAdUnitId();

    static string GetAppKey()
    {
        #if UNITY_ANDROID
            return "85460dcd";
        #elif UNITY_IPHONE
            return "8545d445";
        #else
            return "unexpected_platform";
        #endif
    }

    static string GetBannerAdUnitId()
    {
        #if UNITY_ANDROID
            return "thnfvcsog13bhn08";
        #elif UNITY_IPHONE
            return "iep3rxsyp9na3rw8";
        #else
            return "unexpected_platform";
        #endif
    }
    static string GetInterstitialAdUnitId()
    {
        #if UNITY_ANDROID
            return "aeyqi3vqlv6o8sh9";
        #elif UNITY_IPHONE
            return "wmgt0712uuux8ju4";
        #else
            return "unexpected_platform";
        #endif
    }

    static string GetRewardedVideoAdUnitId()
    {
        #if UNITY_ANDROID
            return "76yy3nay3ceui2a3";
        #elif UNITY_IPHONE
            return "qwouvdrkuwivay5q";
        #else
            return "unexpected_platform";
        #endif
    }
}
