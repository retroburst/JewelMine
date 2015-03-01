using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace JewelMine.Engine
{
    /// <summary>
    /// Game timer with better accuracy then the .NET system timer.
    /// Went with this approach instead of picking a way into windows
    /// subsyetem, as this might work on other operating systems.
    /// </summary>
    public class GameTimer
    {
        Stopwatch stopwatch = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameTimer"/> class.
        /// </summary>
        public GameTimer()
        {
            stopwatch = new Stopwatch();
            stopwatch.Reset();
        }

        /// <summary>
        /// Gets the elapsed milliseconds.
        /// </summary>
        /// <value>
        /// The elapsed milliseconds.
        /// </value>
        public long ElapsedMilliseconds
        {
            get { return stopwatch.ElapsedMilliseconds; }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            if (!stopwatch.IsRunning)
            {
                stopwatch.Reset();
                stopwatch.Start();
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            stopwatch.Stop();
        }
    }
}
