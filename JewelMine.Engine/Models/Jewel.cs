using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine.Models
{
    /// <summary>
    /// Represents a delta in the mine.
    /// </summary>
    public class Jewel : MineObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Jewel"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public Jewel(JewelType type)
        {
            JewelType = type;
        }

        /// <summary>
        /// Gets the type of the delta.
        /// </summary>
        /// <value>
        /// The type of the delta.
        /// </value>
        public JewelType JewelType { get; private set; }

        /// <summary>
        /// Gets the delta type string.
        /// </summary>
        /// <value>
        /// The delta type string.
        /// </value>
        public string JewelTypeString 
        { 
            get
            {
                return(JewelType.ToString());
            }
        }
    }
}
