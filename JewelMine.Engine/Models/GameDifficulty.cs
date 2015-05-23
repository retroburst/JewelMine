using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine.Models
{
    /// <summary>
    /// Represents settings for game difficulty.
    /// </summary>
    [Serializable]
    public class GameDifficulty
    {
        /// <summary>
        /// Gets the difficulty level.
        /// </summary>
        /// <value>The difficulty level.</value>
        public DifficultyLevel DifficultyLevel { get; private set; }

        /// <summary>
        /// Gets the level increment score threshold.
        /// </summary>
        /// <value>The level increment score threshold.</value>
        public int LevelIncrementScoreThreshold { get; private set; }

        /// <summary>
        /// Gets the level increment speed change.
        /// </summary>
        /// <value>The level increment speed change.</value>
        public double LevelIncrementSpeedChange { get; private set; }

        /// <summary>
        /// Gets the tick speed milliseconds.
        /// </summary>
        /// <value>The tick speed milliseconds.</value>
        public double TickSpeedMilliseconds { get; private set; }

        /// <summary>
        /// Gets the tick speed milliseconds floor.
        /// </summary>
        /// <value>The tick speed milliseconds floor.</value>
        public double TickSpeedMillisecondsFloor { get; private set; }

        /// <summary>
        /// Gets the collision finalise tick count.
        /// </summary>
        /// <value>The collision finalise tick count.</value>
        public int CollisionFinaliseTickCount { get; private set; }

        /// <summary>
        /// Gets the delta stationary time span.
        /// </summary>
        /// <value>The delta stationary time span.</value>
        public TimeSpan DeltaStationaryTimeSpan { get; private set; }

        /// <summary>
        /// Gets the delta double jewel chance.
        /// </summary>
        /// <value>The delta double jewel chance.</value>
        public double DeltaDoubleJewelChance { get; private set; }

        /// <summary>
        /// Gets the delta triple jewel chance.
        /// </summary>
        /// <value>The delta triple jewel chance.</value>
        public double DeltaTripleJewelChance { get; private set; }

        /// <summary>
        /// Gets the delta double jewel chance floor.
        /// </summary>
        /// <value>The delta double jewel chance floor.</value>
        public double DeltaDoubleJewelChanceFloor { get; private set; }

        /// <summary>
        /// Gets the delta triple jewel chance floor.
        /// </summary>
        /// <value>The delta triple jewel chance floor.</value>
        public double DeltaTripleJewelChanceFloor { get; private set; }

        /// <summary>
        /// Gets the group collision score.
        /// </summary>
        /// <value>The group collision score.</value>
        public int GroupCollisionScore { get; private set; }

        /// <summary>
        /// Gets the last level.
        /// </summary>
        /// <value>The last level.</value>
        public int LastLevel { get; private set; }

        /// <summary>
        /// Gets the initial lines.
        /// </summary>
        /// <value>The initial lines.</value>
        public int InitialLines { get; private set; }

        /// <summary>
        /// The levels.
        /// </summary>
        private static DifficultyLevel[] levels = (DifficultyLevel[])Enum.GetValues(typeof(DifficultyLevel)).Cast<DifficultyLevel>().ToArray();

        /// <summary>
        /// Initializes a new instance of the <see cref="JewelMine.Engine.Models.GameDifficulty"/> class.
        /// </summary>
        /// <param name="level">Level.</param>
        /// <param name="userSettings">User settings.</param>
        public GameDifficulty(DifficultyLevel level, GameLogicUserSettings userSettings)
        {
            DifficultyLevel = level;
            switch (level)
            {
                case Engine.DifficultyLevel.Easy:
                    SetFromConfigurableSettings(userSettings.EasyDifficultySettings);
                    break;
                case Engine.DifficultyLevel.Moderate:
                    SetFromConfigurableSettings(userSettings.ModerateDifficultySettings);
                    break;
                case Engine.DifficultyLevel.Hard:
                    SetFromConfigurableSettings(userSettings.HardDifficultySettings);
                    break;
                case Engine.DifficultyLevel.Impossible:
                    SetFromConfigurableSettings(userSettings.ImpossibleDifficultySettings);
                    break;
            }
        }

        /// <summary>
        /// Finds the next difficulty level.
        /// </summary>
        /// <returns></returns>
        public static DifficultyLevel FindNextDifficultyLevel(DifficultyLevel target)
        {
            int i = 0;
            for (i = 0; i < levels.Length; i++)
            {
                if (levels[i] == target)
                    break;
            }
            if (i == levels.Length - 1)
                return (levels[0]);
            else
                return (levels[i + 1]);
        }

        /// <summary>
        /// Sets from configurable settings.
        /// </summary>
        /// <param name="provider">Provider.</param>
        private void SetFromConfigurableSettings(IGameDifficultySettingsProvider provider)
        {
            LevelIncrementScoreThreshold = provider.LevelIncrementScoreThreshold;
            TickSpeedMilliseconds = provider.TickSpeedMilliseconds;
            TickSpeedMillisecondsFloor = provider.TickSpeedMillisecondsFloor;
            CollisionFinaliseTickCount = provider.CollisionFinaliseTickCount;
            DeltaStationaryTimeSpan = new TimeSpan(0, 0, 0, 0, provider.DeltaStationaryInMilliseconds);
            DeltaDoubleJewelChance = provider.DeltaDoubleJewelChance;
            DeltaDoubleJewelChanceFloor = provider.DeltaDoubleJewelChanceFloor;
            DeltaTripleJewelChance = provider.DeltaTripleJewelChance;
            DeltaTripleJewelChanceFloor = provider.DeltaTripleJewelChanceFloor;
            GroupCollisionScore = provider.GroupCollisionScore;
            LastLevel = provider.LastLevel;
            InitialLines = provider.InitialLines;
            LevelIncrementSpeedChange = CalculateLevelIncrementSpeedChange();
        }

        /// <summary>
        /// Calculates the level increment speed change.
        /// </summary>
        /// <returns></returns>
        private double CalculateLevelIncrementSpeedChange()
        {
            return ((TickSpeedMilliseconds - TickSpeedMillisecondsFloor) / LastLevel);
        }
    }
}
