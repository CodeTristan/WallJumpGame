using System;

namespace com.unity3d.mediation
{
    /// <summary>
    /// Represents the reward from LevelPlay Rewarded Ad, including a name and the amount
    /// </summary>
    [Obsolete("The namespace com.unity3d.mediation is deprecated. Use LevelPlayReward under the new namespace Unity.Services.LevelPlay.")]
    public class LevelPlayReward : Unity.Services.LevelPlay.LevelPlayReward
    {
        internal LevelPlayReward(string name, int amount) : base(name, amount) {}
    }
}

namespace Unity.Services.LevelPlay
{
    /// <summary>
    /// Represents the reward from LevelPlay Rewarded Ad, including a name and the amount
    /// </summary>
    public class LevelPlayReward
    {
        /// <summary>
        /// The name of the reward.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The amount of the reward.
        /// </summary>
        public int Amount { get; }

        internal LevelPlayReward(string name, int amount)
        {
            Name = name;
            Amount = amount;
        }
    }
}
