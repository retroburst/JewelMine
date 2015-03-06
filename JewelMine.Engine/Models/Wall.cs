using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine.Models
{
    /// <summary>
    /// Represents a wall (obstruction) in the mine.
    /// </summary>
    public class Wall : MineObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Wall"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public Wall(WallType type)
        {
            WallType = type;
        }

        /// <summary>
        /// Gets the type of the wall.
        /// </summary>
        /// <value>
        /// The type of the wall.
        /// </value>
        public WallType WallType { get; private set; }

        /// <summary>
        /// Gets the wall type string.
        /// </summary>
        /// <value>
        /// The wall type string.
        /// </value>
        public string WallTypeString 
        { 
            get
            {
                return(WallType.ToString());
            }
        }
    }
}
