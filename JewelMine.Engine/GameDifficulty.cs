using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine
{
    /// <summary>
    /// Represents settings for game difficulty.
    /// </summary>
    public class GameDifficulty
    {
        public DifficultyLevel DifficultyLevel { get; private set; }
        public int LevelIncrementScoreThreshold { get; private set; }
        public int LevelIncrementSpeedChange { get; private set; }
        public double TickSpeedMilliseconds { get; private set; }
        public double TickSpeedMillisecondsFloor { get; private set; }
        public int CollisionFinaliseTickCount { get; private set; }
        public int DeltaStationaryTickCount { get; private set; }
        public int DeltaDoubleJewelChanceAbove { get; private set; }
        public int DeltaTripleJewelChanceAbove { get; private set; }
        public int DeltaDoubleJewelChanceAboveCeiling { get; private set; }
        public int DeltaTripleJewelChanceAboveCeiling { get; private set; }
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
            LevelIncrementSpeedChange = 1;
            TickSpeedMilliseconds = 240.0d;
            TickSpeedMillisecondsFloor = 120.0d;
            CollisionFinaliseTickCount = 30;
            DeltaStationaryTickCount = 8;
            DeltaDoubleJewelChanceAbove = 5;
            DeltaDoubleJewelChanceAboveCeiling = 60;
            DeltaTripleJewelChanceAbove = 50;
            DeltaTripleJewelChanceAboveCeiling = 95;
            GroupCollisionScore = 1000;
            LastLevel = 255;
            InitialLines = 5;
        }

        /// <summary>
        /// Sets the moderate settings.
        /// </summary>
        private void SetModerateSettings()
        {
            DifficultyLevel = Engine.DifficultyLevel.Moderate;
            LevelIncrementScoreThreshold = 5000;
            LevelIncrementSpeedChange = 1;
            TickSpeedMilliseconds = 210.0d;
            TickSpeedMillisecondsFloor = 75.0d;
            CollisionFinaliseTickCount = 30;
            DeltaStationaryTickCount = 8;
            DeltaDoubleJewelChanceAbove = 30;
            DeltaDoubleJewelChanceAboveCeiling = 90;
            DeltaTripleJewelChanceAbove = 70;
            DeltaTripleJewelChanceAboveCeiling = 99;
            GroupCollisionScore = 1000;
            LastLevel = 500;
            InitialLines = 5;
        }

        /// <summary>
        /// Sets the hard settings.
        /// </summary>
        private void SetHardSettings()
        {
            DifficultyLevel = Engine.DifficultyLevel.Hard;
            LevelIncrementScoreThreshold = 5000;
            LevelIncrementSpeedChange = 2;
            TickSpeedMilliseconds = 200.0d;
            TickSpeedMillisecondsFloor = 60.0d;
            CollisionFinaliseTickCount = 30;
            DeltaStationaryTickCount = 8;
            DeltaDoubleJewelChanceAbove = 50;
            DeltaDoubleJewelChanceAboveCeiling = 95;
            DeltaTripleJewelChanceAbove = 80;
            DeltaTripleJewelChanceAboveCeiling = 99;
            GroupCollisionScore = 1000;
            LastLevel = 1000;
            InitialLines = 8;
        }

        /// <summary>
        /// Sets the impossible settings.
        /// </summary>
        private void SetImpossibleSettings()
        {
            DifficultyLevel = Engine.DifficultyLevel.Impossible;
            LevelIncrementScoreThreshold = 5000;
            LevelIncrementSpeedChange = 1;
            TickSpeedMilliseconds = 100.0d;
            TickSpeedMillisecondsFloor = 50.0d;
            CollisionFinaliseTickCount = 30;
            DeltaStationaryTickCount = 8;
            DeltaDoubleJewelChanceAbove = 80;
            DeltaDoubleJewelChanceAboveCeiling = 99;
            DeltaTripleJewelChanceAbove = 90;
            DeltaTripleJewelChanceAboveCeiling = 99;
            GroupCollisionScore = 1000;
            LastLevel = 10000;
            InitialLines = 10;
        }

    }
}
