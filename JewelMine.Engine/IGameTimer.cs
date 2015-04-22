using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine
{
    /// <summary>
    /// Represents the contract for a game timer.
    /// </summary>
    public interface IGameTimer
    {
        /// <summary>
        /// Gets the elapsed milliseconds.
        /// </summary>
        /// <value>
        /// The elapsed milliseconds.
        /// </value>
        long ElapsedMilliseconds { get; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops this instance.
        /// </summary>
        void Stop();
    }
}
