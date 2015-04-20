using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine.Models
{
    /// <summary>
    /// Represents a saved game state.
    /// </summary>
    [Serializable]
    public class SavedGameState
    {
        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public GameState State { get; set; }

        /// <summary>
        /// Gets or sets the saved on date and time.
        /// </summary>
        /// <value>
        /// The saved on.
        /// </value>
        public DateTime SavedOn { get; set; }
    }
}
