using JewelMine.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine
{
    /// <summary>
    /// Represents updates in the game logic that
    /// views may need to be aware of for region
    /// invalidation.
    /// </summary>
    public class GameLogicUpdate
    {
        /// <summary>
        /// Gets the delta movements.
        /// </summary>
        /// <value>
        /// The delta movements.
        /// </value>
        public List<JewelMovement> JewelMovements { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameLogicUpdate"/> class.
        /// </summary>
        public GameLogicUpdate()
        {
            JewelMovements = new List<JewelMovement>();
        }
    }

    /// <summary>
    /// Represents a delta movement.
    /// </summary>
    public class JewelMovement
    {
        public Jewel Jewel { get; set; }
        public Coordinates Original { get; set; }
        public Coordinates New { get; set; }
    }
}
