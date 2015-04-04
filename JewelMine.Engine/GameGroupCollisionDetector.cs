using JewelMine.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine
{
    /// <summary>
    /// Game group collision detector finds and marks
    /// new group collisions, stores and removes
    /// finalised collisions.
    /// </summary>
    public class GameGroupCollisionDetector
    {
        private GameState state = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameGroupCollisionDetector"/> class.
        /// </summary>
        /// <param name="gameState">State of the game.</param>
        public GameGroupCollisionDetector(GameState gameState)
        {
            state = gameState;
        }

        /// <summary>
        /// Checks the marked collision groups are stil valid.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        private void CheckMarkedCollisionGroupsStillValid(GameLogicUpdate logicUpdate)
        {
            logicUpdate.InvalidCollisions.Clear();
            // for each collision group, check that each jewel is still
            // in it's position since the collision, if not remove it
            foreach (MarkedCollisionGroup group in state.Mine.MarkedCollisions)
            {
                foreach (CollisionGroupMember member in group.Members)
                {
                    MineObject target = state.Mine[member.Coordinates];
                    if (target == null || target != member.Jewel)
                    {
                        logicUpdate.InvalidCollisions.Add(group);
                        break;
                    }
                }
            }
            if (logicUpdate.InvalidCollisions.Count > 0) state.Mine.MarkedCollisions.RemoveAll(x => logicUpdate.InvalidCollisions.Contains(x));
        }

        /// <summary>
        /// Marks the group collisions.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        public void MarkGroupCollisions(GameLogicUpdate logicUpdate)
        {
            // find new collisions and additions to existing marked collisions and mark
            // add new marks to logic update
            CheckMarkedCollisionGroupsStillValid(logicUpdate);
            // check for new collisions
            AddNewMarkedCollisionGroups();
            // check for new additions to existing marked collisions
            // loop through marked collisions collection
            // based on direction of collision group look on ends for new collisions
            // add to group if there and not in another group
            AddNewMembersToMarkedCollisions();
            // add marked collisions to the logic update
            logicUpdate.Collisions.Clear();
            logicUpdate.Collisions.AddRange(state.Mine.MarkedCollisions);
        }

        /// <summary>
        /// Adds the new members to marked collision.
        /// </summary>
        private void AddNewMembersToMarkedCollisions()
        {
            foreach (MarkedCollisionGroup group in state.Mine.MarkedCollisions)
            {
                if (group == null || group.Members.Count == 0) continue;
                switch (group.Direction)
                {
                    case CollisionDirection.Horizontal:
                        // find largest and smallest X coordinate
                        CollisionGroupMember left = group.Members.OrderBy(x => x.Coordinates.X).First();
                        CollisionGroupMember right = group.Members.OrderByDescending(x => x.Coordinates.X).First();
                        AddNewMembersToMarkedCollisionGroupByDirection(right.Jewel, right.Coordinates, coordinates => new Coordinates(coordinates.X + 1, coordinates.Y), group);
                        AddNewMembersToMarkedCollisionGroupByDirection(left.Jewel, left.Coordinates, coordinates => new Coordinates(coordinates.X - 1, coordinates.Y), group);
                        break;
                    case CollisionDirection.Vertical:
                        // find largest and smallest Y coordinate
                        CollisionGroupMember top = group.Members.OrderBy(x => x.Coordinates.Y).First();
                        CollisionGroupMember bottom = group.Members.OrderByDescending(x => x.Coordinates.Y).First();
                        AddNewMembersToMarkedCollisionGroupByDirection(bottom.Jewel, bottom.Coordinates, coordinates => new Coordinates(coordinates.X, coordinates.Y + 1), group);
                        AddNewMembersToMarkedCollisionGroupByDirection(top.Jewel, top.Coordinates, coordinates => new Coordinates(coordinates.X, coordinates.Y - 1), group);
                        break;
                    case CollisionDirection.DiagonallyLeft:
                        // find largest and smallest Y coordinate
                        CollisionGroupMember topRight = group.Members.OrderBy(x => x.Coordinates.Y).First();
                        CollisionGroupMember bottomLeft = group.Members.OrderByDescending(x => x.Coordinates.Y).First();
                        AddNewMembersToMarkedCollisionGroupByDirection(bottomLeft.Jewel, bottomLeft.Coordinates, coordinates => new Coordinates(coordinates.X - 1, coordinates.Y - 1), group);
                        AddNewMembersToMarkedCollisionGroupByDirection(topRight.Jewel, topRight.Coordinates, coordinates => new Coordinates(coordinates.X + 1, coordinates.Y + 1), group);
                        break;
                    case CollisionDirection.DiagonallyRight:
                        // find largest and smallest Y coordinate
                        CollisionGroupMember topLeft = group.Members.OrderBy(x => x.Coordinates.Y).First();
                        CollisionGroupMember bottomRight = group.Members.OrderByDescending(x => x.Coordinates.Y).First();
                        AddNewMembersToMarkedCollisionGroupByDirection(bottomRight.Jewel, bottomRight.Coordinates, coordinates => new Coordinates(coordinates.X - 1, coordinates.Y + 1), group);
                        AddNewMembersToMarkedCollisionGroupByDirection(topLeft.Jewel, topLeft.Coordinates, coordinates => new Coordinates(coordinates.X + 1, coordinates.Y - 1), group);
                        break;
                }
            }
        }

        /// <summary>
        /// Adds the new members to marked collision by direction.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="targetCoordinates">The target coordinates.</param>
        /// <param name="moveSearch">The move search.</param>
        /// <param name="group">The group.</param>
        private void AddNewMembersToMarkedCollisionGroupByDirection(Jewel target, Coordinates targetCoordinates, Func<Coordinates, Coordinates> moveSearch, MarkedCollisionGroup group)
        {
            Coordinates coordinates = moveSearch(targetCoordinates);
            while (state.Mine.CoordinatesInBounds(coordinates))
            {
                // see how many in a row up that are not already marked
                if (state.Mine[coordinates] != null && state.Mine[coordinates] is Jewel)
                {
                    Jewel searchJewel = (Jewel)state.Mine[coordinates];
                    if (searchJewel.JewelType == target.JewelType
                        && !IsAlreadyMarkedCollision(searchJewel)
                        && (state.Mine.Delta == null || !state.Mine.Delta.IsGroupMember(searchJewel)))
                    {
                        group.Members.Add(new CollisionGroupMember(searchJewel, coordinates));
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
                coordinates = moveSearch(coordinates);
            }
        }

        /// <summary>
        /// Adds the new marked collision groups.
        /// </summary>
        private void AddNewMarkedCollisionGroups()
        {
            for (int x = state.Mine.Columns - 1; x >= 0; x--)
            {
                for (int y = state.Mine.Depth - 1; y >= 0; y--)
                {
                    List<MarkedCollisionGroup> foundCollisionGroups = new List<MarkedCollisionGroup>();
                    MineObject mineObject = state.Mine.Grid[x, y];
                    if (mineObject == null || mineObject.GetType() != typeof(Jewel)) continue;
                    Jewel target = (Jewel)mineObject;
                    if (IsAlreadyMarkedCollision(target)) continue;
                    if (state.Mine.Delta != null && state.Mine.Delta.IsGroupMember(target)) continue;

                    MarkedCollisionGroup foundVertical = new MarkedCollisionGroup() { Direction = CollisionDirection.Vertical };
                    MarkedCollisionGroup foundHorizontal = new MarkedCollisionGroup() { Direction = CollisionDirection.Horizontal };
                    MarkedCollisionGroup foundDiagonallyLeft = new MarkedCollisionGroup() { Direction = CollisionDirection.DiagonallyLeft };
                    MarkedCollisionGroup foundDiagonllyRight = new MarkedCollisionGroup() { Direction = CollisionDirection.DiagonallyRight };

                    foundVertical.Members.AddRange(FindCollisions(target, new Coordinates(x, y), coordinates => new Coordinates(coordinates.X, coordinates.Y - 1), coordinates => new Coordinates(coordinates.X, coordinates.Y + 1)));
                    foundHorizontal.Members.AddRange(FindCollisions(target, new Coordinates(x, y), coordinates => new Coordinates(coordinates.X - 1, coordinates.Y), coordinates => new Coordinates(coordinates.X + 1, coordinates.Y)));
                    foundDiagonallyLeft.Members.AddRange(FindCollisions(target, new Coordinates(x, y), coordinates => new Coordinates(coordinates.X - 1, coordinates.Y - 1), coordinates => new Coordinates(coordinates.X + 1, coordinates.Y + 1)));
                    foundDiagonllyRight.Members.AddRange(FindCollisions(target, new Coordinates(x, y), coordinates => new Coordinates(coordinates.X + 1, coordinates.Y - 1), coordinates => new Coordinates(coordinates.X - 1, coordinates.Y + 1)));

                    foundCollisionGroups.Add(foundVertical);
                    foundCollisionGroups.Add(foundHorizontal);
                    foundCollisionGroups.Add(foundDiagonallyLeft);
                    foundCollisionGroups.Add(foundDiagonllyRight);

                    var largestCollisionGroup = foundCollisionGroups.OrderByDescending(group => group.Members.Count).FirstOrDefault();
                    if (largestCollisionGroup != null && largestCollisionGroup.Members.Count >= 3)
                    {
                        state.Mine.MarkedCollisions.Add(largestCollisionGroup);
                    }
                }
            }
        }

        /// <summary>
        /// Finds the collisions.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="coordinates">The coordinates.</param>
        /// <param name="incrementSearch">The increment search.</param>
        /// <param name="decrementSearch">The decrement search.</param>
        /// <returns></returns>
        private List<CollisionGroupMember> FindCollisions(Jewel target, Coordinates coordinates, Func<Coordinates, Coordinates> incrementSearch, Func<Coordinates, Coordinates> decrementSearch)
        {
            List<CollisionGroupMember> found = new List<CollisionGroupMember>();
            found.Add(new CollisionGroupMember(target, coordinates));
            // check by increment (up)
            FindCollisionsByDirection(target, coordinates, incrementSearch, found);
            // check by decrement (down)
            FindCollisionsByDirection(target, coordinates, decrementSearch, found);
            return (found);
        }

        /// <summary>
        /// Finds the collisions by direction.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="targetCoordinates">The target coordinates.</param>
        /// <param name="moveSearch">The move search.</param>
        /// <param name="foundCollisions">The found collisions.</param>
        private void FindCollisionsByDirection(Jewel target, Coordinates targetCoordinates, Func<Coordinates, Coordinates> moveSearch, List<CollisionGroupMember> foundCollisions)
        {
            Coordinates coordinates = moveSearch(targetCoordinates);
            while (state.Mine.CoordinatesInBounds(coordinates))
            {
                // see how many in a row up that are not already marked
                if (state.Mine[coordinates] != null && state.Mine[coordinates] is Jewel)
                {
                    Jewel searchJewel = (Jewel)state.Mine[coordinates];
                    if (searchJewel.JewelType == target.JewelType
                        && !IsAlreadyMarkedCollision(target)
                        && !IsAlreadyMarkedCollision(searchJewel)
                        && (state.Mine.Delta == null || !state.Mine.Delta.IsGroupMember(searchJewel)))
                    {
                        foundCollisions.Add(new CollisionGroupMember(searchJewel, coordinates));
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
                coordinates = moveSearch(coordinates);
            }
        }

        /// <summary>
        /// Determines whether [is already marked collision] [the specified target].
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        private bool IsAlreadyMarkedCollision(Jewel target)
        {
            return (state.Mine.MarkedCollisions.Any(x => x.IsGroupMember(target)));
        }

        /// <summary>
        /// Finalises the collision groups.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        public void FinaliseCollisionGroups(GameLogicUpdate logicUpdate, params CollisionGroup[] collisions)
        {
            // move any marked collisions to finalised collisions
            // add new finalised collisions to logic update
            // remove finalised jewels from mine grid
            logicUpdate.FinalisedCollisions.Clear();
            state.Mine.FinalisedCollisions.Clear();
            state.Mine.FinalisedCollisions.AddRange(collisions);
            state.Mine.MarkedCollisions.RemoveAll(x => collisions.Contains(x));
            state.Mine.FinalisedCollisions.ForEach(x => x.Members.ForEach(y => RemoveFromMine(y)));
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
