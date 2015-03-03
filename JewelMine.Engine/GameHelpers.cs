using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine
{
    /// <summary>
    /// Helper methods for game logic and views.
    /// </summary>
    public static class GameHelpers
    {
        /// <summary>
        /// Peforms a for each over an IEnumerable collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) return;
            for (int i = source.Count()-1; i >= 0; i--)
            {
                action(source.ElementAt(i));
            }
        }
    }
}
