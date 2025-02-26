using System;

namespace com.unity3d.mediation
{
    /// <summary>
    /// Defines the types of advertisement formats available in the LevelPlay SDK.
    /// </summary>
    [Obsolete("The namespace com.unity3d.mediation is deprecated. Use LevelPlayAdFormat under the new namespace Unity.Services.LevelPlay.")]
    public enum LevelPlayAdFormat
    {
        BANNER,
        INTERSTITIAL,
        REWARDED
    }
}

namespace Unity.Services.LevelPlay
{
    /// <summary>
    /// Defines the types of advertisement formats available in the LevelPlay SDK.
    /// </summary>
    public enum LevelPlayAdFormat
    {
        BANNER,
        INTERSTITIAL,
        REWARDED
    }
}
