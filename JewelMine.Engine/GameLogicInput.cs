using JewelMine.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine
{
    /// <summary>
    /// Represents user input for use
    /// by game logic.
    /// </summary>
    public class GameLogicInput
    {
        /// <summary>
        /// Gets the delta movement.
        /// </summary>
        /// <value>
        /// The delta movement.
        /// </value>
        public MovementType? DeltaMovement = null;

        /// <summary>
        /// The delta swap jewels
        /// </summary>
        public bool DeltaSwapJewels = false; 
    }
}
