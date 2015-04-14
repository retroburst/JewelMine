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
        public int InitialLineCount { get; private set; }
        public int GroupCollisionScore { get; private set; }
        public int LastLevel { get; private set; }
        private DifficultyLevel[] levels = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameDifficulty"/> class.
        /// </summary>
        public GameDifficulty()
        {
            levels = (DifficultyLevel[])Enum.GetValues(typeof(DifficultyLevel)).Cast<DifficultyLevel>().ToArray();
            SetEasySettings();
        }

        /// <summary>
        /// Sets the difficulty.
        /// </summary>
        public void ChangeDifficulty()
        {
            DifficultyLevel nextLevel = FindNextDifficultyLevel();
            switch (nextLevel)
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
        private DifficultyLevel FindNextDifficultyLevel()
        {
            int i = 0;
            for (i = 0; i < levels.Length; i++)
            {
                if (levels[i] == DifficultyLevel) break;
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
            TickSpeedMillisecondsFloor = 80.0d;
            CollisionFinaliseTickCount = 30;
            DeltaStationaryTickCount = 8;
            DeltaDoubleJewelChanceAbove = 10;
            DeltaDoubleJewelChanceAboveCeiling = 85;
            DeltaTripleJewelChanceAbove = 50;
            DeltaTripleJewelChanceAboveCeiling = 95;
            InitialLineCount = 5;
            GroupCollisionScore = 1000;
            LastLevel = 255;
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
            InitialLineCount = 6;
            GroupCollisionScore = 1000;
            LastLevel = 500;
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
            InitialLineCount = 10;
            GroupCollisionScore = 1000;
            LastLevel = 1000;
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
            InitialLineCount = 12;
            GroupCollisionScore = 1000;
            LastLevel = 10000;
        }

    }
}
