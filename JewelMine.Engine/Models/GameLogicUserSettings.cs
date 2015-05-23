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

        /// <summary>
        /// Gets or sets the easy game difficulty settings.
        /// </summary>
        /// <value>
        /// The easy game difficulty settings.
        /// </value>
        public IGameDifficultySettingsProvider EasyDifficultySettings { get; set; }

        /// <summary>
        /// Gets or sets the moderte game difficulty settings.
        /// </summary>
        /// <value>
        /// The moderte game difficulty settings.
        /// </value>
        public IGameDifficultySettingsProvider ModerateDifficultySettings { get; set; }

        /// <summary>
        /// Gets or sets the hard game difficulty settings.
        /// </summary>
        /// <value>
        /// The hard game difficulty settings.
        /// </value>
        public IGameDifficultySettingsProvider HardDifficultySettings { get; set; }

        /// <summary>
        /// Gets or sets the impossible game difficulty settings.
        /// </summary>
        /// <value>
        /// The impossible game difficulty settings.
        /// </value>
        public IGameDifficultySettingsProvider ImpossibleDifficultySettings { get; set; }

        /// <summary>
        /// Gets or sets the mine columns.
        /// </summary>
        /// <value>
        /// The mine columns.
        /// </value>
        public int MineColumns { get; set; }

        /// <summary>
        /// Gets or sets the mine depth.
        /// </summary>
        /// <value>
        /// The mine depth.
        /// </value>
        public int MineDepth { get; set; }
    }
}
