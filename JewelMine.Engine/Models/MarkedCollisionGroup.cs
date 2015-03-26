using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine.Models
{
    /// <summary>
    /// Represents a group of jewels
    /// which form a marked collision group.
    /// </summary>
    public class MarkedCollisionGroup : CollisionGroup
    {
        public int CollisionTickCount { get; internal set; }
        public CollisionDirection Direction { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkedCollisionGroup"/> class.
        /// </summary>
        public MarkedCollisionGroup()
            : base()
        {}

        /// <summary>
        /// Increments the collision tick count.
        /// For use in lambda expression.
        /// </summary>
        public void IncrementCollisionTickCount()
        {
            CollisionTickCount++;
        }

    }
}
