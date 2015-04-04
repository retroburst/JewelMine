using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JewelMine.Engine;

namespace JewelMine.Engine.Models
{
    /// <summary>
    /// Represents the game state at any given time.
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameState"/> class.
        /// </summary>
        public GameState()
        {
            Mine = new Mine();
            PlayState = GamePlayState.NotStarted;
            Level = GameConstants.GAME_DEFAULT_LEVEL;
            TickSpeedMilliseconds = GameConstants.GAME_DEFAULT_TICK_SPEED_MILLISECONDS;
            Score = 0;
            DeltaStationaryTickCount = GameConstants.GAME_DELTA_STATIONARY_TICK_COUNT;
            CollisionFinailseTickCount = GameConstants.GAME_COLLISION_FINALISE_TICK_COUNT;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameState"/> class.
        /// </summary>
        /// <param name="mineModel">The mine model.</param>
        public GameState(Mine mineModel)
            : this()
        {
            Mine = mineModel;
        }

        /// <summary>
        /// Gets the state of the game play.
        /// </summary>
        /// <value>
        /// The state of the game play.
        /// </value>
        public GamePlayState PlayState { get; internal set; }

        /// <summary>
        /// Gets the grid.
        /// </summary>
        /// <value>
        /// The grid.
        /// </value>
        public Mine Mine { get; private set; }

        /// <summary>
        /// Gets the game level.
        /// </summary>
        /// <value>
        /// The game level.
        /// </value>
        public int Level { get; internal set; }

        /// <summary>
        /// Gets the game score.
        /// </summary>
        /// <value>
        /// The game score.
        /// </value>
        public long Score { get; internal set; }

        /// <summary>
        /// Gets the game tick speed milliseconds.
        /// </summary>
        /// <value>
        /// The game tick speed milliseconds.
        /// </value>
        public double TickSpeedMilliseconds { get; internal set; }

        /// <summary>
        /// Gets the collision finalise tick count.
        /// </summary>
        /// <value>
        /// The collision finalise tick count.
        /// </value>
        public int CollisionFinailseTickCount { get; internal set; }

        /// <summary>
        /// Gets the delta stationary tick count.
        /// </summary>
        /// <value>
        /// The delta stationary tick count.
        /// </value>
        public int DeltaStationaryTickCount { get; internal set; }

    }
}
