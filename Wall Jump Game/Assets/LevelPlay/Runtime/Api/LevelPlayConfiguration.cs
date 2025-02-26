using System;
using System.Collections.Generic;

namespace com.unity3d.mediation
{
    /// <summary>
    /// Represents the configuration settings for the LevelPlay mediation platform.
    /// </summary>
    [Obsolete("The namespace com.unity3d.mediation is deprecated. Use LevelPlayConfiguration under the new namespace Unity.Services.LevelPlay.")]
    public class LevelPlayConfiguration : Unity.Services.LevelPlay.LevelPlayConfiguration
    {
        internal LevelPlayConfiguration(string json) : base(json) {}
    }
}

namespace Unity.Services.LevelPlay
{
    /// <summary>
    /// Represents the configuration settings for the LevelPlay mediation platform.
    /// </summary>
    public class LevelPlayConfiguration
    {
        const string k_IsAdQualityEnabled = "isAdQualityEnabled";
        /// <summary>
        /// Indicates whether ad quality control is enabled.
        /// </summary>
        public bool IsAdQualityEnabled { get; }

        internal LevelPlayConfiguration(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return;
            }
            try
            {
                object obj;
#pragma warning disable 0618
                var jsonDic = IronSourceJSON.Json.Deserialize(json) as Dictionary<string, object>;
#pragma warning restore 0618
                if (jsonDic.TryGetValue(k_IsAdQualityEnabled, out obj) && obj != null)
                {
                    if (bool.TryParse(obj.ToString(), out var isAdQualityEnabled))
                    {
                        IsAdQualityEnabled = isAdQualityEnabled;
                    }
                    else
                    {
                        LevelPlayLogger.LogError("Failed to parse isAdQualityEnabled: " + obj);
                    }
                }
            }
            catch (System.Exception e)
            {
                LevelPlayLogger.LogError("Failed to parse LevelPlayConfiguration: " + e.Message);
            }
        }
    }
}
