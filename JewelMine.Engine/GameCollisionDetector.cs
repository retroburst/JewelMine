using JewelMine.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine
{
    /// <summary>
    /// Game collision detector finds and marks
    /// new collisions, stores and removes
    /// finalised collisions.
    /// </summary>
    public class GameCollisionDetector
    {
        private GameState state = null;

        /// <summary>
        /// Gets or sets the marked collisions.
        /// </summary>
        /// <value>
        /// The marked collisions.
        /// </value>
        public List<MarkedCollisionGroup> MarkedCollisions { get; private set; }

        /// <summary>
        /// Gets or sets the finalised collisions.
        /// </summary>
        /// <value>
        /// The finalised collisions.
        /// </value>
        public List<CollisionGroup> FinalisedCollisions { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameCollisionDetector"/> class.
        /// </summary>
        /// <param name="gameState">State of the game.</param>
        public GameCollisionDetector(GameState gameState)
        {
            state = gameState;
            MarkedCollisions = new List<MarkedCollisionGroup>();
            FinalisedCollisions = new List<CollisionGroup>();
        }

        /// <summary>
        /// Checks the marked collisions are stil valid.
        /// </summary>
        private void CheckMarkedCollisionsStillValid()
        {
            List<MarkedCollisionGroup> invalidCollisions = new List<MarkedCollisionGroup>();
            // for each collision group, check that each jewel is still
            // in it's position since the collision, if not remove it
            foreach(MarkedCollisionGroup group in MarkedCollisions)
            {
                foreach(CollisionGroupMember member in group.Members)
                {
                    MineObject target = state.Mine[member.Coordinates];
                    if(target == null || target != member.Jewel)
                    {
                        invalidCollisions.Add(group);
                        break;
                    }
                }
            }
            if (invalidCollisions.Count > 0) MarkedCollisions.RemoveAll(x => invalidCollisions.Contains(x));
        }

        /// <summary>
        /// Marks the collisions.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        public void MarkCollisions(GameLogicUpdate logicUpdate)
        {
            // find new collisions and additions to existing marked collisions and mark
            // add new marks to logic update
            CheckMarkedCollisionsStillValid();

        }

        /// <summary>
        /// Finalises the collisions.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        public void FinaliseCollisions(GameLogicUpdate logicUpdate, params CollisionGroup[] collisions)
        {
            // move any marked collisions to finalised collisions
            // add new finalised collisions to logic update
            // remove finalised jewels from mine grid
            logicUpdate.FinalisedCollisions.Clear();
            FinalisedCollisions.Clear();
            FinalisedCollisions.AddRange(collisions);
            MarkedCollisions.RemoveAll(x => collisions.Contains(x));
            FinalisedCollisions.ForEach(x => x.Members.ForEach(y => RemoveFromMine(y)));
            logicUpdate.FinalisedCollisions.AddRange(collisions);
        }

        /// <summary>
        /// Removes from mine.
        /// </summary>
        /// <param name="member">The member.</param>
        private void RemoveFromMine(CollisionGroupMember member)
        {
            MineObject target = state.Mine[member.Coordinates];
            if(target != null && target == member.Jewel)
            {
                state.Mine[member.Coordinates] = null;
            }
        }

    }
}
