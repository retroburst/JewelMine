using JewelMine.Engine.Models;
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
        private static HashSet<char> vowels = new HashSet<char>("aeiou"); 

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

        /// <summary>
        /// Calculates the score.
        /// </summary>
        /// <param name="groups">The groups.</param>
        /// <returns></returns>
        internal static long CalculateScore(MarkedCollisionGroup[] groups)
        {
            long score = 0;
            foreach(MarkedCollisionGroup group in groups)
            {
                // default for a 3 jewel collision
                long groupScore = GameConstants.GAME_DEFAULT_COLLISION_SCORE;
                // give extra points for additional collision on top of 3 jewels
                int extraCollisions = group.Members.Count - 3;
                if (extraCollisions > 0) groupScore += (groupScore * extraCollisions);
                // if diagonal then double it
                if (group.Direction == CollisionDirection.DiagonallyLeft || group.Direction == CollisionDirection.DiagonallyRight) groupScore = groupScore * 10;
                score += groupScore;
            }
            return (score);
        }

        /// <summary>
        /// Shortens the name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string ShortenName(string name)
        {
            if (string.IsNullOrEmpty(name)) return (name);
            string result = new string(name.Where(x => !vowels.Contains(x)).ToArray());
            return (result);
        }



    }
}
