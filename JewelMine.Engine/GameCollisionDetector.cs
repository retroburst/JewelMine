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
            foreach (MarkedCollisionGroup group in MarkedCollisions)
            {
                foreach (CollisionGroupMember member in group.Members)
                {
                    MineObject target = state.Mine[member.Coordinates];
                    if (target == null || target != member.Jewel)
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
            // check for new collisions
            for (int x = state.Mine.Columns - 1; x >= 0; x--)
            {
                for (int y = state.Mine.Depth - 1; y >= 0; y--)
                {
                    MineObject mineObject = state.Mine.Grid[x, y];
                    if (mineObject == null || mineObject.GetType() != typeof(Jewel)) continue;
                    Jewel target = (Jewel)mineObject;
                    if (IsAlreadyMarkedCollision(target)) continue;
                    if (state.Mine.Delta != null && state.Mine.Delta.IsGroupMember(target)) continue;



                    // check up
                    List<CollisionGroupMember> found = new List<CollisionGroupMember>();
                    found.Add(new CollisionGroupMember(target, new Coordinates(x, y)));
                    // make sure can check up
                    if ((y - 1) >= 0)
                    {
                        // see how many in a row up that are not already marked
                        for (int searchY = y - 1; searchY >= 0; searchY--)
                        {
                            if (state.Mine.Grid[x, searchY] != null && state.Mine.Grid[x, searchY] is Jewel)
                            {
                                Jewel searchJewel = (Jewel)state.Mine.Grid[x, searchY];
                                if (searchJewel.JewelType == target.JewelType)
                                {
                                    found.Add(new CollisionGroupMember(searchJewel, new Coordinates(x, searchY)));
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (found.Count >= 3)
                    {
                        MarkedCollisionGroup group = new MarkedCollisionGroup();
                        group.Members.AddRange(found);
                        group.CollisionTickCount = 0;
                        MarkedCollisions.Add(group);
                    }

                    // check down

                    // check left

                    // check right

                    // check diagonally upper left 

                    // check diagonally upper right

                    // check diagonally lower left

                    // check diagnoally lower right

                }
            }
            // check for new additions to existing marked collisions
        }

        /// <summary>
        /// Determines whether [is already marked collision] [the specified target].
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        private bool IsAlreadyMarkedCollision(Jewel target)
        {
            return (MarkedCollisions.Any(x => x.IsGroupMember(target)));
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
            if (target != null && target == member.Jewel)
            {
                state.Mine[member.Coordinates] = null;
            }
        }

    }
}
