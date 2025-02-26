using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.unity3d.mediation
{
    /// <summary>
    /// Represents an initialization error for the LevelPlay SDK.
    /// </summary>
    [Obsolete("The namespace com.unity3d.mediation is deprecated. Use LevelPlayInitError under the new namespace Unity.Services.LevelPlay.")]
    public class LevelPlayInitError : Unity.Services.LevelPlay.LevelPlayInitError
    {
        internal LevelPlayInitError(string json) : base(json) {}
    }
}

namespace Unity.Services.LevelPlay
{
    /// <summary>
    /// Represents an initialization error for the LevelPlay SDK.
    /// </summary>
    public class LevelPlayInitError
    {
        public int ErrorCode {get;}
        public string ErrorMessage {get;}

        internal LevelPlayInitError(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return;
            }

            try
            {
#pragma warning disable 0618
                Dictionary<string, object> jsonDic =
                    IronSourceJSON.Json.Deserialize(json) as Dictionary<string, object>;
#pragma warning restore 0618
                if (jsonDic.TryGetValue("errorCode", out var obj) && obj != null)
                {
                    ErrorCode = Int32.Parse(obj.ToString());
                }
                if (jsonDic.TryGetValue("errorMessage", out obj) && obj != null)
                {
                    ErrorMessage = obj.ToString();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to parse LevelPlayInitError: " + e.Message);
            }
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="LevelPlayInitError"/>.
        /// </summary>
        /// <returns>A string that contains the error code and message.</returns>
        public override string ToString()
        {
            return $"LevelPlayInitError: {ErrorCode}, {ErrorMessage}";
        }
    }
}
