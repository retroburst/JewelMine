using JewelMine.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine
{
    /// <summary>
    /// Game collision detector finds and marks
    /// new collisions, stores and removes
    /// finalised collisions.
    /// </summary>
    public class GameCollisionDetector
    {
        private GameState state = null;
        private bool[,] markedCollisions = null;
        private bool[,] finalisedCollisions = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameCollisionDetector"/> class.
        /// </summary>
        /// <param name="gameState">State of the game.</param>
        public GameCollisionDetector(GameState gameState)
        {
            state = gameState;
        }

        /// <summary>
        /// Marks the collisions.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        public void MarkCollisions(GameLogicUpdate logicUpdate)
        {
            // find new collisions and additions to existing marked collisions and mark
            // add new marks to logic update
        }

        /// <summary>
        /// Finalises the collisions.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        public void FinaliseCollisions(GameLogicUpdate logicUpdate)
        {
            // move any marked collisions to finalised collisions
            // add new finalised collisions to logic update
        }

        /// <summary>
        /// Clears the finalised collisions.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        public void ClearFinalisedCollisions(GameLogicUpdate logicUpdate)
        {
            // remove all finalised collisions from finalised grid and actual mine grid
            // update logic update with jewel removals
        }

        /// <summary>
        /// Gets the marked collisions.
        /// </summary>
        /// <value>
        /// The marked collisions.
        /// </value>
        public bool[,] MarkedCollisions
        {
            get
            {
                return (markedCollisions);
            }
        }

        /// <summary>
        /// Gets the finalised collisions.
        /// </summary>
        /// <value>
        /// The finalised collisions.
        /// </value>
        public bool[,] FinalisedCollisions
        {
            get
            {
                return(finalisedCollisions);
            }
        }

    }
}
