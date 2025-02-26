using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.unity3d.mediation
{
    /// <summary>
    /// Represents an error received from LevelPlay
    /// </summary>
    [Obsolete("The namespace com.unity3d.mediation is deprecated. Use LevelPlayAdError under the new namespace Unity.Services.LevelPlay.")]
    public class LevelPlayAdError : Unity.Services.LevelPlay.LevelPlayAdError
    {
        internal LevelPlayAdError(string json) : base(json) {}

        [Obsolete("The constructor LevelPlayAdError will be removed in version 9.0.0.")]
        public LevelPlayAdError(string adUnitId, int errorCode, string errorMessage) : base(adUnitId, errorCode, errorMessage) {}
    }
}

namespace Unity.Services.LevelPlay
{
    /// <summary>
    /// Represents an error received from LevelPlay
    /// </summary>
    public class LevelPlayAdError
    {
        public int ErrorCode { get; }
        public string ErrorMessage { get; }
        public string AdUnitId { get; }
        public string AdId { get; }

        internal LevelPlayAdError(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return;
            }

            try
            {
#pragma warning disable 0618
                Dictionary<string, object>
                jsonDic = IronSourceJSON.Json.Deserialize(json) as Dictionary<string, object>;
#pragma warning restore 0618
                if (jsonDic.TryGetValue("errorCode", out var obj) && obj != null)
                {
                    ErrorCode = Int32.Parse(obj.ToString());
                }

                if (jsonDic.TryGetValue("errorMessage", out obj) && obj != null)
                {
                    ErrorMessage = obj.ToString();
                }

                if (jsonDic.TryGetValue("adUnitId", out obj) && obj != null)
                {
                    AdUnitId = obj.ToString();
                }

                if (jsonDic.TryGetValue("adId", out obj) && obj != null)
                {
                    AdId = obj.ToString();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to parse LevelPlayAdError: " + e.Message);
                return;
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="LevelPlayAdError"/> class  with specified details.
        /// </summary>
        /// <param name="adUnitId">The advertisement unit identifier.</param>
        /// <param name="errorCode">The error code associated with the error.</param>
        /// <param name="errorMessage">A message describing the error.</param>
        /// <param name="adId">The ad identifier.</param>
        internal LevelPlayAdError(string adUnitId, int errorCode, string errorMessage, string adId)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            AdUnitId = adUnitId;
            AdId = adId;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="LevelPlayAdError"/> class  with specified details.
        /// </summary>
        /// <param name="adUnitId">The advertisement unit identifier.</param>
        /// <param name="errorCode">The error code associated with the error.</param>
        /// <param name="errorMessage">A message describing the error.</param>
        internal LevelPlayAdError(string adUnitId, int errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            AdUnitId = adUnitId;
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="LevelPlayAdError"/>.
        /// </summary>
        /// <returns>A string that contains the error code, message, and advertisement unit identifier.</returns>
        public override string ToString()
        {
            return $"LevelPlayAdError: {ErrorCode}, {ErrorMessage}, {AdUnitId}, {AdId}";
        }
    }
}
