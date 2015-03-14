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
        /// Gets the jewel movements.
        /// </summary>
        /// <value>
        /// The jewel movements.
        /// </value>
        public List<JewelMovement> JewelMovements { get; private set; }

        /// <summary>
        /// Gets the collisions.
        /// </summary>
        /// <value>
        /// The collisions.
        /// </value>
        public List<CollisionGroup> Collisions { get; private set; }

        /// <summary>
        /// Gets the finalised collisions.
        /// </summary>
        /// <value>
        /// The finalised collisions.
        /// </value>
        public List<CollisionGroup> FinalisedCollisions { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameLogicUpdate"/> class.
        /// </summary>
        public GameLogicUpdate()
        {
            JewelMovements = new List<JewelMovement>();
            Collisions = new List<CollisionGroup>();
            FinalisedCollisions = new List<CollisionGroup>();
        }

    }
}
