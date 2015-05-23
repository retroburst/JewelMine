using JewelMine.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine
{
    /// <summary>
    /// Provides settings for game difficulty levels.
    /// </summary>
    public class GameDifficultySettingsProvider : IGameDifficultySettingsProvider
    {
        /// <summary>
        /// The default easy settings.
        /// </summary>
        public static readonly GameDifficultySettingsProvider DEFAULT_EASY_SETTINGS = new GameDifficultySettingsProvider()
        {
            LevelIncrementScoreThreshold = 5000,
            TickSpeedMilliseconds = 240.0d,
            TickSpeedMillisecondsFloor = 120.0d,
            CollisionFinaliseTickCount = 30,
            DeltaStationaryInMilliseconds = 1920,
            DeltaDoubleJewelChance = 0.9d,
            DeltaDoubleJewelChanceFloor = 0.4d,
            DeltaTripleJewelChance = 0.5d,
            DeltaTripleJewelChanceFloor = 0.05d,
            GroupCollisionScore = 1000,
            LastLevel = 255,
            InitialLines = 5
        };

        /// <summary>
        /// The default moderate settings.
        /// </summary>
        public static readonly GameDifficultySettingsProvider DEFAULT_MODERATE_SETTINGS = new GameDifficultySettingsProvider()
        {
            LevelIncrementScoreThreshold = 5000,
            TickSpeedMilliseconds = 220.0d,
            TickSpeedMillisecondsFloor = 75.0d,
            CollisionFinaliseTickCount = 30,
            DeltaStationaryInMilliseconds = 1760,
            DeltaDoubleJewelChance = 0.75d,
            DeltaDoubleJewelChanceFloor = 0.25d,
            DeltaTripleJewelChance = 0.20d,
            DeltaTripleJewelChanceFloor = 0.04d,
            GroupCollisionScore = 2000,
            LastLevel = 500,
            InitialLines = 5
        };

        /// <summary>
        /// The default hard settings.
        /// </summary>
        public static readonly GameDifficultySettingsProvider DEFAULT_HARD_SETTINGS = new GameDifficultySettingsProvider()
        {
            LevelIncrementScoreThreshold = 5000,
            TickSpeedMilliseconds = 200.0d,
            TickSpeedMillisecondsFloor = 60.0d,
            CollisionFinaliseTickCount = 30,
            DeltaStationaryInMilliseconds = 1600,
            DeltaDoubleJewelChance = 0.50d,
            DeltaDoubleJewelChanceFloor = 0.05d,
            DeltaTripleJewelChance = 0.20d,
            DeltaTripleJewelChanceFloor = 0.01d,
            GroupCollisionScore = 3000,
            LastLevel = 1000,
            InitialLines = 4
        };

        /// <summary>
        /// The default impossible settings.
        /// </summary>
        public static readonly GameDifficultySettingsProvider DEFAULT_IMPOSSIBLE_SETTINGS = new GameDifficultySettingsProvider()
        {
            LevelIncrementScoreThreshold = 5000,
            TickSpeedMilliseconds = 100.0d,
            TickSpeedMillisecondsFloor = 40.0d,
            CollisionFinaliseTickCount = 30,
            DeltaStationaryInMilliseconds = 800,
            DeltaDoubleJewelChance = 0.20d,
            DeltaDoubleJewelChanceFloor = 0.01d,
            DeltaTripleJewelChance = 0.10d,
            DeltaTripleJewelChanceFloor = 0.001d,
            GroupCollisionScore = 4000,
            LastLevel = 10000,
            InitialLines = 3
        };

        /// <summary>
        /// Gets the level increment score threshold.
        /// </summary>
        /// <value>
        /// The level increment score threshold.
        /// </value>
        public int LevelIncrementScoreThreshold
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the tick speed milliseconds.
        /// </summary>
        /// <value>
        /// The tick speed milliseconds.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public double TickSpeedMilliseconds
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the tick speed milliseconds floor.
        /// </summary>
        /// <value>
        /// The tick speed milliseconds floor.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public double TickSpeedMillisecondsFloor
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the collision finalise tick count.
        /// </summary>
        /// <value>
        /// The collision finalise tick count.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public int CollisionFinaliseTickCount
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the delta stationary in milliseconds.
        /// </summary>
        /// <value>
        /// The delta stationary in milliseconds.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public int DeltaStationaryInMilliseconds
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the delta double jewel chance.
        /// </summary>
        /// <value>
        /// The delta double jewel chance.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public double DeltaDoubleJewelChance
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the delta triple jewel chance.
        /// </summary>
        /// <value>
        /// The delta triple jewel chance.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public double DeltaTripleJewelChance
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the delta double jewel chance floor.
        /// </summary>
        /// <value>
        /// The delta double jewel chance floor.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public double DeltaDoubleJewelChanceFloor
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the delta triple jewel chance floor.
        /// </summary>
        /// <value>
        /// The delta triple jewel chance floor.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public double DeltaTripleJewelChanceFloor
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the group collision score.
        /// </summary>
        /// <value>
        /// The group collision score.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public int GroupCollisionScore
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the last level.
        /// </summary>
        /// <value>
        /// The last level.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public int LastLevel
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the initial lines.
        /// </summary>
        /// <value>
        /// The initial lines.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public int InitialLines
        {
            get;
            internal set;
        }
    }
}
