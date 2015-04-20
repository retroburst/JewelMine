using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine.Models
{
    /// <summary>
    /// Represents a group of jewels
    /// which form a collision group.
    /// </summary
    [Serializable]
    public class CollisionGroup
    {
        /// <summary>
        /// Gets the members.
        /// </summary>
        /// <value>
        /// The members.
        /// </value>
        public List<CollisionGroupMember> Members { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JewelGroup"/> class.
        /// </summary>
        /// <param name="top">The top.</param>
        /// <param name="middle">The middle.</param>
        /// <param name="bottom">The bottom.</param>
        public CollisionGroup()
        {
            Members = new List<CollisionGroupMember>();
        }

        /// <summary>
        /// Determines whether [is group member] [the specified jewel].
        /// </summary>
        /// <param name="jewel">The jewel.</param>
        /// <returns></returns>
        public bool IsGroupMember(Jewel jewel)
        {
            return (Members.Any(x => x.Jewel == jewel));
        }

    }
}
