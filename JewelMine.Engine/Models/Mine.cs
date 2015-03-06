using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace JewelMine.Engine.Models
{
    /// <summary>
    /// Represents the mine, all the
    /// jewels and wall positions.
    /// Also defines which delta is the delta.
    /// </summary>
    public class Mine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mine"/> class.
        /// </summary>
        public Mine()
            : this(GameConstants.MINE_DEFAULT_COLUMN_SIZE, GameConstants.MINE_DEFAULT_DEPTH_SIZE)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mine"/> class.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <param name="depth">The depth.</param>
        public Mine(int columns, int depth)
        {
            if (Columns < 0) Columns = GameConstants.MINE_DEFAULT_COLUMN_SIZE;
            if (Depth < 0) Depth = GameConstants.MINE_DEFAULT_DEPTH_SIZE;
            Columns = columns;
            Depth = depth;
            Delta = null;
            Grid = new MineObject[Columns, Depth];
        }

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public int Columns { get; private set; }

        /// <summary>
        /// Gets the depth.
        /// </summary>
        /// <value>
        /// The depth.
        /// </value>
        public int Depth { get; private set; }

        /// <summary>
        /// Gets the delta.
        /// </summary>
        /// <value>
        /// The delta.
        /// </value>
        public JewelGroup Delta { get; internal set; }

        /// <summary>
        /// Gets the grid.
        /// </summary>
        /// <value>
        /// The mine.
        /// </value>
        public MineObject[,] Grid { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="MineObject"/> with the specified coordinates.
        /// </summary>
        /// <value>
        /// The <see cref="MineObject"/>.
        /// </value>
        /// <param name="coordinates">The coordinates.</param>
        /// <returns></returns>
        public MineObject this[Coordinates coordinates]
        {
            get
            {
                return (Grid[coordinates.X, coordinates.Y]);
            }
            set
            {
                Grid[coordinates.X, coordinates.Y] = value;
            }
        }

    }
}
