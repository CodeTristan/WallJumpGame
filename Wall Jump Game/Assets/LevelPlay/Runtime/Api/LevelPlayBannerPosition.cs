using System;

namespace com.unity3d.mediation
{
    /// <summary>
    /// Represents positions on the screen where banner ads can be placed.
    /// </summary>
    [Obsolete("The namespace com.unity3d.mediation is deprecated. Use LevelPlayBannerPosition under the new namespace Unity.Services.LevelPlay.")]
    public enum LevelPlayBannerPosition
    {
        TopCenter = 1,
        BottomCenter = 2
    }
}

namespace Unity.Services.LevelPlay
{
    /// <summary>
    /// Represents positions on the screen where banner ads can be placed.
    /// </summary>
    public enum LevelPlayBannerPosition
    {
        TopCenter = 1,
        BottomCenter = 2
    }
}
