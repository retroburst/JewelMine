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
    /// Also defines which jewel is the delta.
    /// </summary>
    public class MineModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MineModel"/> class.
        /// </summary>
        public MineModel()
            : this(GameConstants.MINE_DEFAULT_COLUMN_SIZE, GameConstants.MINE_DEFAULT_DEPTH_SIZE)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MineModel"/> class.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <param name="depth">The depth.</param>
        public MineModel(int columns, int depth)
        {
            if (Columns < 0) Columns = GameConstants.MINE_DEFAULT_COLUMN_SIZE;
            if (Depth < 0) Depth = GameConstants.MINE_DEFAULT_DEPTH_SIZE;
            Columns = columns;
            Depth = depth;
            Delta = null;
            DeltaX = 0;
            DeltaY = 0;
            Mine = new MineObjectModel[Columns, Depth];
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
        public JewelModel Delta { get; internal set; }

        /// <summary>
        /// Gets the delta x.
        /// </summary>
        /// <value>
        /// The delta x.
        /// </value>
        public int DeltaX { get; internal set; }

        /// <summary>
        /// Gets the delta y.
        /// </summary>
        /// <value>
        /// The delta y.
        /// </value>
        public int DeltaY { get; internal set; }

        /// <summary>
        /// Gets the mine.
        /// </summary>
        /// <value>
        /// The mine.
        /// </value>
        public MineObjectModel[,] Mine { get; private set; }
    }
}
