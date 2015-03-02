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
        /// Gets the new jewels.
        /// </summary>
        /// <value>
        /// The new jewels.
        /// </value>
        public List<NewJewel> NewJewels { get; private set; }

        /// <summary>
        /// Gets the jewel movements.
        /// </summary>
        /// <value>
        /// The jewel movements.
        /// </value>
        public List<JewelMovement> JewelMovements { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameLogicUpdate"/> class.
        /// </summary>
        public GameLogicUpdate()
        {
            NewJewels = new List<NewJewel>();
            JewelMovements = new List<JewelMovement>();
        }
    }

    /// <summary>
    /// Represents a new jewel added.
    /// </summary>
    public class NewJewel
    {
        public JewelModel Jewel { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }

    /// <summary>
    /// Represents a jewel movement.
    /// </summary>
    public class JewelMovement
    {
        public JewelModel Jewel { get; set; }
        public int OriginalX { get; set; }
        public int OriginalY { get; set; }
        public int NewX { get; set; }
        public int NewY { get; set; }
    }
}
