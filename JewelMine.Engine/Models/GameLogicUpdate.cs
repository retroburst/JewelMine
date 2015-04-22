using JewelMine.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine.Models
{
    /// <summary>
    /// Represents updates in the game logic that
    /// views may need to be aware of.
    /// </summary>
    public class GameLogicUpdate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameLogicUpdate"/> class.
        /// </summary>
        public GameLogicUpdate()
        {
            JewelMovements = new List<JewelMovement>();
            Collisions = new List<MarkedCollisionGroup>();
            InvalidCollisions = new List<MarkedCollisionGroup>();
            FinalisedCollisions = new List<CollisionGroup>();
            Messages = new List<string>();
        }

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
        public List<MarkedCollisionGroup> Collisions { get; private set; }

        /// <summary>
        /// Gets the invalid collisions.
        /// </summary>
        /// <value>
        /// The invalid collisions.
        /// </value>
        public List<MarkedCollisionGroup> InvalidCollisions { get; private set; }

        /// <summary>
        /// Gets the finalised collisions.
        /// </summary>
        /// <value>
        /// The finalised collisions.
        /// </value>
        public List<CollisionGroup> FinalisedCollisions { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [delta stationary].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [delta stationary]; otherwise, <c>false</c>.
        /// </value>
        public bool DeltaStationary { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [delta jewels swapped].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [delta jewels swapped]; otherwise, <c>false</c>.
        /// </value>
        public bool DeltaJewelsSwapped { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [level incremented].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [level incremented]; otherwise, <c>false</c>.
        /// </value>
        public bool LevelIncremented { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [game loaded].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [game loaded]; otherwise, <c>false</c>.
        /// </value>
        public bool GameLoaded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [game restarted].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [game restarted]; otherwise, <c>false</c>.
        /// </value>
        public bool GameStarted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [game won].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [game won]; otherwise, <c>false</c>.
        /// </value>
        public bool GameWon { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [game paused].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [game paused]; otherwise, <c>false</c>.
        /// </value>
        public bool GamePaused { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [game over].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [game over]; otherwise, <c>false</c>.
        /// </value>
        public bool GameOver { get; set; }

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        public List<string> Messages { get; set; }

    }
}
