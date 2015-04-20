using JewelMine.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine
{
    /// <summary>
    /// Represents user logicInput for use
    /// by game logic.
    /// </summary>
    public class GameLogicInput
    {
        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            DeltaMovement = null;
            DeltaSwapJewels = false;
            RestartGame = false;
            GameStarted = false;
            PauseGame = false;
            ChangeDifficulty = false;
            SaveGame = false;
            LoadGame = false;
        }

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
        public bool DeltaSwapJewels { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [restart game].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [restart game]; otherwise, <c>false</c>.
        /// </value>
        public bool RestartGame { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [pause game].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [pause game]; otherwise, <c>false</c>.
        /// </value>
        public bool PauseGame { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [game started].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [resume game]; otherwise, <c>false</c>.
        /// </value>
        public bool GameStarted { get; set; }

        /// <summary>
        /// Gets or sets the change difficulty.
        /// </summary>
        /// <value>
        /// The change difficulty.
        /// </value>
        public bool ChangeDifficulty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [save game].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [save game]; otherwise, <c>false</c>.
        /// </value>
        public bool SaveGame { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [load game].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [load game]; otherwise, <c>false</c>.
        /// </value>
        public bool LoadGame { get; set; }
    }
}
