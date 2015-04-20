using JewelMine.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine
{
    /// <summary>
    /// Represents an object that can provide
    /// the current game state.
    /// </summary>
    public interface IGameStateProvider
    {
        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        GameState State { get; }
    }
}
