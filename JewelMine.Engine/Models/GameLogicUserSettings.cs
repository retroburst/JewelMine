using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine.Models
{
    /// <summary>
    /// Stores user settings from the user.config for the game logic engine.
    /// </summary>
    public class GameLogicUserSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameLogicUserSettings"/> class.
        /// </summary>
        public GameLogicUserSettings()
        { }

        /// <summary>
        /// Gets or sets the user preferred difficulty.
        /// </summary>
        /// <value>
        /// The user preferred difficulty.
        /// </value>
        public DifficultyLevel UserPreferredDifficulty { get; set; }
    }
}
