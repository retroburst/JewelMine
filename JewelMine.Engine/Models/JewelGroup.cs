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
    [Serializable]
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
            Top = new JewelGroupMember(top, Coordinates.CreateInvalidatedCoordinates());
            Middle = new JewelGroupMember(middle, Coordinates.CreateInvalidatedCoordinates());
            Bottom = new JewelGroupMember(bottom, Coordinates.CreateInvalidatedCoordinates());
        }

        /// <summary>
        /// Gets the stationary since.
        /// </summary>
        /// <value>
        /// The stationary since.
        /// </value>
        public Nullable<DateTime> StationarySince
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has whole group entered bounds.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has whole group entered bounds; otherwise, <c>false</c>.
        /// </value>
        public bool HasWholeGroupEnteredBounds
        {
            get
            {
                return (Top.HasEnteredBounds && Middle.HasEnteredBounds && Bottom.HasEnteredBounds);
            }
        }

        /// <summary>
        /// Determines whether [is group member] [the specified jewel].
        /// </summary>
        /// <param name="jewel">The jewel.</param>
        /// <returns></returns>
        public bool IsGroupMember(Jewel jewel)
        {
            return(jewel == Top.Jewel 
                || jewel == Middle.Jewel
                || jewel == Bottom.Jewel);
        }

    }
}
