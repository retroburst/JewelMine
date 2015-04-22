using JewelMine.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine
{
    /// <summary>
    /// Comparer for marked collisions.
    /// </summary>
    public class CollisionGroupComparer : IComparer<MarkedCollisionGroup>
    {
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.
        /// </returns>
        public int Compare(MarkedCollisionGroup x, MarkedCollisionGroup y)
        {
            if (x == y) return 0;
            if (y == null) return 1;
            if (x == null) return -1;

            if (x.Members.Count > y.Members.Count) return 1;
            if (x.Members.Count < y.Members.Count) return -1;
            else
            {
                // count is the same so let's compare direction
                if (x.Direction == CollisionDirection.DiagonallyLeft || x.Direction == CollisionDirection.DiagonallyRight) return 1;
                if (y.Direction == CollisionDirection.DiagonallyLeft || y.Direction == CollisionDirection.DiagonallyRight) return -1;
                else return 0;
            }
        }
    }
}
