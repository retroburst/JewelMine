using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine.Models
{
    /// <summary>
    /// Represents a set of coordinates for a point.
    /// </summary>
    public class Coordinates
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Coordinates"/> class.
        /// </summary>
        public Coordinates() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Coordinates"/> class.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <param name="y">The y.</param>
        public Coordinates(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Coordinates"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public Coordinates(Coordinates source)
        {
            X = source.X;
            Y = source.Y;
        }

        /// <summary>
        /// Creates a set of invalidated coordinates.
        /// </summary>
        /// <returns></returns>
        public static Coordinates CreateInvalidatedCoordinates()
        {
            return new Coordinates(-1, -1);
        }

        /// <summary>
        /// Copies from another source set of coordinates.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <exception cref="ArgumentException">Argument cannot be null.;target</exception>
        public void CopyFrom(Coordinates source)
        {
            if (source == null) throw new ArgumentException("Argument cannot be null.", "source");
            X = source.X;
            Y = source.Y;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public Coordinates Clone()
        {
            return (new Coordinates(X, Y));
        }

        /// <summary>
        /// Invalidates this instance.
        /// </summary>
        public void Invalidate()
        {
            X = -1;
            Y = -1;
        }


        /// <summary>
        /// Determines whether this instance is invalidated.
        /// </summary>
        /// <returns></returns>
        public bool IsInvalidated()
        {
            return (X == -1 || Y == -1);
        }

        /// <summary>
        /// Gets or sets the coordinates.
        /// </summary>
        /// <value>
        /// The coordinates.
        /// </value>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public int Y { get; set; }
    }
}
