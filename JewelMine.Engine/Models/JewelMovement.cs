using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine.Models
{
    /// <summary>
    /// Represents a jewel movement.
    /// </summary>
    public class JewelMovement
    {
        /// <summary>
        /// Gets or sets the jewel.
        /// </summary>
        /// <value>
        /// The jewel.
        /// </value>
        public Jewel Jewel { get; set; }

        /// <summary>
        /// Gets or sets the original coordinates.
        /// </summary>
        /// <value>
        /// The original.
        /// </value>
        public Coordinates Original { get; set; }

        /// <summary>
        /// Gets or sets the new coordinates.
        /// </summary>
        /// <value>
        /// The new.
        /// </value>
        public Coordinates New { get; set; }
    }
}
