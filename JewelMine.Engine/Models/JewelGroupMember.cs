using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine.Models
{
    /// <summary>
    /// Represents a single delta member of a group.
    /// </summary>
    public class JewelGroupMember
    {
        /// <summary>
        /// Gets the delta.
        /// </summary>
        /// <value>
        /// The delta.
        /// </value>
        public Jewel Jewel { get; internal set; }

        /// <summary>
        /// Gets the coordinates.
        /// </summary>
        /// <value>
        /// The coordinates.
        /// </value>
        public Coordinates Coordinates { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this instance has entered bounds.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has entered bounds; otherwise, <c>false</c>.
        /// </value>
        public bool HasEnteredBounds { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JewelGroupMember"/> class.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="coordinates">The coordinates.</param>
        public JewelGroupMember(Jewel jewel, Coordinates coordinates)
        {
            Jewel = jewel;
            Coordinates = coordinates;
            HasEnteredBounds = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JewelGroupMember"/> class.
        /// </summary>
        /// <param name="jewel">The jewel.</param>
        /// <param name="coordinates">The coordinates.</param>
        /// <param name="hasEnteredBounds">if set to <c>true</c> [has entered bounds].</param>
        public JewelGroupMember(Jewel jewel, Coordinates coordinates, bool hasEnteredBounds)
        {
            Jewel = jewel;
            Coordinates = coordinates;
            HasEnteredBounds = hasEnteredBounds;
        }

    }
}
