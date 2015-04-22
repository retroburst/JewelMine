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
        public DifficultyLevel DifficultyLevel { get; private set; }
        public int LevelIncrementScoreThreshold { get; private set; }
        public double LevelIncrementSpeedChange { get; private set; }
        public double TickSpeedMilliseconds { get; private set; }
        public double TickSpeedMillisecondsFloor { get; private set; }
        public int CollisionFinaliseTickCount { get; private set; }
        public TimeSpan DeltaStationaryTimeSpan { get; private set; }
        public double DeltaDoubleJewelChance { get; private set; }
        public double DeltaTripleJewelChance { get; private set; }
        public double DeltaDoubleJewelChanceFloor { get; private set; }
        public double DeltaTripleJewelChanceFloor { get; private set; }
        public int GroupCollisionScore { get; private set; }
        public int LastLevel { get; private set; }
        public int InitialLines { get; private set; }
        private static DifficultyLevel[] levels = (DifficultyLevel[])Enum.GetValues(typeof(DifficultyLevel)).Cast<DifficultyLevel>().ToArray();

        /// <summary>
        /// Initializes a new instance of the <see cref="GameDifficulty"/> class.
        /// </summary>
        /// <param name="level">The level.</param>
        public GameDifficulty(DifficultyLevel level)
        {
            switch (level)
            {
                case Engine.DifficultyLevel.Easy: SetEasySettings(); break;
                case Engine.DifficultyLevel.Moderate: SetModerateSettings(); break;
                case Engine.DifficultyLevel.Hard: SetHardSettings(); break;
                case Engine.DifficultyLevel.Impossible: SetImpossibleSettings(); break;
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
                if (levels[i] == target) break;
            }
            if (i == levels.Length - 1) return (levels[0]);
            else return (levels[i + 1]);
        }

        /// <summary>
        /// Sets the easy settings.
        /// </summary>
        private void SetEasySettings()
        {
            DifficultyLevel = Engine.DifficultyLevel.Easy;
            LevelIncrementScoreThreshold = 5000;
            TickSpeedMilliseconds = 240.0d;
            TickSpeedMillisecondsFloor = 120.0d;
            CollisionFinaliseTickCount = 30;
            DeltaStationaryTimeSpan = new TimeSpan(0, 0, 0, 0, 1920);
            DeltaDoubleJewelChance = 0.9d;
            DeltaDoubleJewelChanceFloor = 0.4d;
            DeltaTripleJewelChance = 0.5d;
            DeltaTripleJewelChanceFloor = 0.05d;
            GroupCollisionScore = 1000;
            LastLevel = 255;
            InitialLines = 5;
            LevelIncrementSpeedChange = CalculateLevelIncrementSpeedChange();
        }

        /// <summary>
        /// Sets the moderate settings.
        /// </summary>
        private void SetModerateSettings()
        {
            DifficultyLevel = Engine.DifficultyLevel.Moderate;
            LevelIncrementScoreThreshold = 5000;
            TickSpeedMilliseconds = 220.0d;
            TickSpeedMillisecondsFloor = 75.0d;
            CollisionFinaliseTickCount = 30;
            DeltaStationaryTimeSpan = new TimeSpan(0, 0, 0, 0, 1760);
            DeltaDoubleJewelChance = 0.75d;
            DeltaDoubleJewelChanceFloor = 0.25d;
            DeltaTripleJewelChance = 0.20d;
            DeltaTripleJewelChanceFloor = 0.04d;
            GroupCollisionScore = 2000;
            LastLevel = 500;
            InitialLines = 5;
            LevelIncrementSpeedChange = CalculateLevelIncrementSpeedChange();
        }

        /// <summary>
        /// Sets the hard settings.
        /// </summary>
        private void SetHardSettings()
        {
            DifficultyLevel = Engine.DifficultyLevel.Hard;
            LevelIncrementScoreThreshold = 5000;
            TickSpeedMilliseconds = 200.0d;
            TickSpeedMillisecondsFloor = 60.0d;
            CollisionFinaliseTickCount = 30;
            DeltaStationaryTimeSpan = new TimeSpan(0, 0, 0, 0, 1600);
            DeltaDoubleJewelChance = 0.50d;
            DeltaDoubleJewelChanceFloor = 0.05d;
            DeltaTripleJewelChance = 0.20d;
            DeltaTripleJewelChanceFloor = 0.01d;
            GroupCollisionScore = 3000;
            LastLevel = 1000;
            InitialLines = 4;
            LevelIncrementSpeedChange = CalculateLevelIncrementSpeedChange();
        }

        /// <summary>
        /// Sets the impossible settings.
        /// </summary>
        private void SetImpossibleSettings()
        {
            DifficultyLevel = Engine.DifficultyLevel.Impossible;
            LevelIncrementScoreThreshold = 5000;
            TickSpeedMilliseconds = 100.0d;
            TickSpeedMillisecondsFloor = 40.0d;
            CollisionFinaliseTickCount = 30;
            DeltaStationaryTimeSpan = new TimeSpan(0, 0, 0, 0, 800);
            DeltaDoubleJewelChance = 0.20d;
            DeltaDoubleJewelChanceFloor = 0.01d;
            DeltaTripleJewelChance = 0.10d;
            DeltaTripleJewelChanceFloor = 0.001d;
            GroupCollisionScore = 4000;
            LastLevel = 10000;
            InitialLines = 3;
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
