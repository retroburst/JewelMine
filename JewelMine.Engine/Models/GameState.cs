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
            GamePlayState = GamePlayState.NotStarted;
            GameLevel = GameConstants.GAME_DEFAULT_LEVEL;
            GameTickSpeedMilliseconds = GameConstants.GAME_DEFAULT_TICK_SPEED_MILLISECONDS;
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
        public GamePlayState GamePlayState { get; internal set; }

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
        public int GameLevel { get; internal set; }

        /// <summary>
        /// Gets the game tick speed milliseconds.
        /// </summary>
        /// <value>
        /// The game tick speed milliseconds.
        /// </value>
        public double GameTickSpeedMilliseconds { get; internal set; }

        /// <summary>
        /// Gets the game play state string.
        /// </summary>
        /// <value>
        /// The game play state string.
        /// </value>
        public string GamePlayStateString
        {
            get
            {
                return (GamePlayState.ToString());
            }
        }
    }
}
