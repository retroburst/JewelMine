using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine.Models
{
    /// <summary>
    /// Represents a jewel in the mine.
    /// </summary>
    public class JewelModel : MineObjectModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JewelModel"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public JewelModel(JewelType type)
        {
            JewelType = type;
        }

        /// <summary>
        /// Gets the type of the jewel.
        /// </summary>
        /// <value>
        /// The type of the jewel.
        /// </value>
        public JewelType JewelType { get; private set; }

        /// <summary>
        /// Gets the jewel type string.
        /// </summary>
        /// <value>
        /// The jewel type string.
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
