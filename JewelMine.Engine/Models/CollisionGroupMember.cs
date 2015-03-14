using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine.Models
{
    /// <summary>
    /// Represents a single collision group member.
    /// </summary>
    public class CollisionGroupMember
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
        /// Initializes a new instance of the <see cref="CollisionGroupMember"/> class.
        /// </summary>
        /// <param name="jewel">The jewel.</param>
        /// <param name="coordinates">The coordinates.</param>
        public CollisionGroupMember(Jewel jewel, Coordinates coordinates)
        {
            Jewel = jewel;
            Coordinates = coordinates;
        }

    }
}
