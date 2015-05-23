using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine
{
    /// <summary>
    /// Contract for a provider of game diificulty settings.
    /// </summary>
    public interface IGameDifficultySettingsProvider
    {
        /// <summary>
        /// Gets the level increment score threshold.
        /// </summary>
        /// <value>
        /// The level increment score threshold.
        /// </value>
        int LevelIncrementScoreThreshold { get; }

        /// <summary>
        /// Gets the tick speed milliseconds.
        /// </summary>
        /// <value>
        /// The tick speed milliseconds.
        /// </value>
        double TickSpeedMilliseconds { get; }

        /// <summary>
        /// Gets the tick speed milliseconds floor.
        /// </summary>
        /// <value>
        /// The tick speed milliseconds floor.
        /// </value>
        double TickSpeedMillisecondsFloor { get; }

        /// <summary>
        /// Gets the collision finalise tick count.
        /// </summary>
        /// <value>
        /// The collision finalise tick count.
        /// </value>
        int CollisionFinaliseTickCount { get; }

        /// <summary>
        /// Gets the delta stationary in milliseconds.
        /// </summary>
        /// <value>
        /// The delta stationary in milliseconds.
        /// </value>
        int DeltaStationaryInMilliseconds { get; }

        /// <summary>
        /// Gets the delta double jewel chance.
        /// </summary>
        /// <value>
        /// The delta double jewel chance.
        /// </value>
        double DeltaDoubleJewelChance { get; }

        /// <summary>
        /// Gets the delta triple jewel chance.
        /// </summary>
        /// <value>
        /// The delta triple jewel chance.
        /// </value>
        double DeltaTripleJewelChance { get; }

        /// <summary>
        /// Gets the delta double jewel chance floor.
        /// </summary>
        /// <value>
        /// The delta double jewel chance floor.
        /// </value>
        double DeltaDoubleJewelChanceFloor { get; }

        /// <summary>
        /// Gets the delta triple jewel chance floor.
        /// </summary>
        /// <value>
        /// The delta triple jewel chance floor.
        /// </value>
        double DeltaTripleJewelChanceFloor { get; }

        /// <summary>
        /// Gets the group collision score.
        /// </summary>
        /// <value>
        /// The group collision score.
        /// </value>
        int GroupCollisionScore { get; }

        /// <summary>
        /// Gets the last level.
        /// </summary>
        /// <value>
        /// The last level.
        /// </value>
        int LastLevel { get; }

        /// <summary>
        /// Gets the initial lines.
        /// </summary>
        /// <value>
        /// The initial lines.
        /// </value>
        int InitialLines { get; }
    }
}
