using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine.Models
{
    /// <summary>
    /// Represents a group of jewels
    /// which move together as the delta.
    /// </summary>
    public class JewelGroup
    {
        public JewelGroupMember Top { get; private set; }
        public JewelGroupMember Middle { get; private set; }
        public JewelGroupMember Bottom { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JewelGroup"/> class.
        /// </summary>
        /// <param name="top">The top.</param>
        /// <param name="middle">The middle.</param>
        /// <param name="bottom">The bottom.</param>
        public JewelGroup(JewelGroupMember top, JewelGroupMember middle, JewelGroupMember bottom)
        {
            Top = top;
            Middle = middle;
            Bottom = bottom;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JewelGroup"/> class.
        /// This constructor gives default coordinates of (0,0).
        /// </summary>
        /// <param name="top">The top.</param>
        /// <param name="middle">The middle.</param>
        /// <param name="bottom">The bottom.</param>
        public JewelGroup(Jewel top, Jewel middle, Jewel bottom)
        {
            Top = new JewelGroupMember(top, new Coordinates());
            Middle = new JewelGroupMember(middle, new Coordinates());
            Bottom = new JewelGroupMember(bottom, new Coordinates());
        }

    }
}
