using JewelMine.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace JewelMine.Engine
{
    /// <summary>
    /// Encapsulates the game state models
    /// and all the logic and functionality for game play.
    /// </summary>
    public class GameLogic
    {
        public Random Random { get; private set; }
        private string[] jewelNames = null;
        private GameState state = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameLogic"/> class.
        /// </summary>
        public GameLogic()
        {
            jewelNames = Enum.GetNames(typeof(JewelType)).Where(x => x != JewelType.Unknown.ToString()).ToArray();
            state = new GameState();
            Random = new Random();
        }

        /// <summary>
        /// Gets the game state model.
        /// </summary>
        /// <value>
        /// The game state model.
        /// </value>
        public GameState GameStateModel
        {
            get { return (state); }
        }

        // TODO update game states
        /// <summary>
        /// Starts the game.
        /// </summary>
        public void StartGame()
        {
            state.GamePlayState = GamePlayState.Playing;
        }

        /// <summary>
        /// Stops the game.
        /// </summary>
        public void StopGame()
        {
            state.GamePlayState = GamePlayState.NotStarted;
        }

        /// <summary>
        /// Pauses the game.
        /// </summary>
        public void PauseGame()
        {
            state.GamePlayState = GamePlayState.Paused;
        }

        /// <summary>
        /// Games the over.
        /// </summary>
        public void GameOver()
        {
            state.GamePlayState = GamePlayState.GameOver;
        }

        /// <summary>
        /// Performs the game logic.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException">
        /// Game Over
        /// or
        /// Game Over
        /// </exception>
        public GameLogicUpdate PerformGameLogic(GameLogicInput input)
        {
            GameLogicUpdate logicUpdate = new GameLogicUpdate();
            // check for jewels that need to move down because of successful collisions

            

            // move delta based on input or down by default if no input
            bool userInputMovement = input.DeltaMovement.HasValue;
            MovementType deltaMovement = input.DeltaMovement.HasValue ? input.DeltaMovement.Value : MovementType.Down;
            if (state.Mine.Delta != null)
            {
                if (input.DeltaSwapJewels) { SwapDeltaJewels(); }
                bool deltaStationary = false;
                int numPositionsToMove = 1;
                // if delta is up against a boundary on either side that the movement is towards, override and move delta down instead
                if (IsDeltaAgainstBoundary(deltaMovement)) deltaMovement = MovementType.Down;
                // if delta is up against another block or bounadry on underside ignore input - do not move delta and add new delta
                if (userInputMovement && deltaMovement == MovementType.Down) numPositionsToMove = 2;
                MoveDelta(deltaMovement, logicUpdate, numPositionsToMove);
                if (IsDeltaStationary()) deltaStationary = true;
                if (deltaStationary)
                {
                    if (state.Mine.Delta.StationaryTickCount >= 5)
                    {
                        // delta is now in position and a new one will be added
                        state.Mine.Delta = null;
                    }
                    else
                    {
                        state.Mine.Delta.StationaryTickCount++;
                    }
                }
                else
                {
                    state.Mine.Delta.StationaryTickCount = 0;
                }
            }

            // check for new collisions - ONLY DO THE BELOW IF NO COLLISIONS!!!!!!!!!!!

            // if no delta and no collision, add a new one
            if (state.Mine.Delta == null)
            {
                bool added = AddDelta(logicUpdate);
                if (!added) { throw new NotImplementedException("Game Over"); }
            }
            return (logicUpdate);
        }

        /// <summary>
        /// Swaps the delta jewels downwards.
        /// </summary>
        private void SwapDeltaJewels()
        {
            JewelGroup delta = state.Mine.Delta;
            Jewel top = delta.Top.Jewel;
            delta.Top.Jewel = delta.Bottom.Jewel;
            delta.Bottom.Jewel = delta.Middle.Jewel;
            delta.Middle.Jewel = top;
            if (CoordinatesInBounds(delta.Top.Coordinates)) state.Mine[delta.Top.Coordinates] = delta.Top.Jewel;
            if (CoordinatesInBounds(delta.Middle.Coordinates)) state.Mine[delta.Middle.Coordinates] = delta.Middle.Jewel;
            if (CoordinatesInBounds(delta.Bottom.Coordinates)) state.Mine[delta.Bottom.Coordinates] = delta.Bottom.Jewel;
        }

        /// <summary>
        /// Determines whether [is delta against boundary] [the specified delta movement].
        /// </summary>
        /// <param name="deltaMovement">The delta movement.</param>
        /// <returns></returns>
        private bool IsDeltaAgainstBoundary(MovementType deltaMovement)
        {
            bool result = false;
            JewelGroup delta = state.Mine.Delta;
            if ((deltaMovement == MovementType.Left && delta.Bottom.Coordinates.X == 0)
            || (deltaMovement == MovementType.Right && delta.Bottom.Coordinates.X == state.Mine.Grid.GetUpperBound(0)))
            {
                result = true;
            }
            return (result);
        }

        /// <summary>
        /// Determines whether [is delta stationary].
        /// </summary>
        /// <returns></returns>
        private bool IsDeltaStationary()
        {
            JewelGroup delta = state.Mine.Delta;
            return (delta.Bottom.Coordinates.Y == state.Mine.Grid.GetUpperBound(1)
                || state.Mine.Grid[delta.Bottom.Coordinates.X, delta.Bottom.Coordinates.Y + 1] != null);
        }

        /// <summary>
        /// Adds the delta.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        /// <returns></returns>
        private bool AddDelta(GameLogicUpdate logicUpdate)
        {
            int[] free = FindFreeCoordinatesForDelta();
            if (free.Length == 0)
            {
                return (false);
            }
            int randomIndex = Random.Next(0, free.Length);
            int targetCoorindinate = free[randomIndex];
            JewelGroup delta = GenerateRandomDeltaJewelGroup();
            state.Mine.Delta = delta;
            state.Mine.Grid[targetCoorindinate, 0] = delta.Bottom.Jewel;
            delta.Bottom.Coordinates.X = targetCoorindinate;
            delta.Bottom.Coordinates.Y = 0;
            delta.Bottom.HasEnteredBounds = true;
            logicUpdate.JewelMovements.Add(new JewelMovement() { Jewel = delta.Bottom.Jewel, Original = Coordinates.CreateInvalidatedCoordinates(), New = new Coordinates(targetCoorindinate, 0) });
            return (true);
        }

        /// <summary>
        /// Finds the free coordinates for delta.
        /// </summary>
        /// <returns></returns>
        private int[] FindFreeCoordinatesForDelta()
        {
            List<int> free = new List<int>();
            for (int i = 0; i < state.Mine.Columns; i++)
            {
                if (state.Mine.Grid[i, 0] == null) free.Add(i);
            }
            return (free.ToArray());
        }

        /// <summary>
        /// Generates the random delta delta.
        /// </summary>
        /// <returns></returns>
        private JewelGroup GenerateRandomDeltaJewelGroup()
        {
            Jewel[] randomJewels = new Jewel[3];
            for (int i = 0; i < randomJewels.Length; i++)
            {
                int randomIndex = Random.Next(0, jewelNames.Length);
                JewelType type = (JewelType)Enum.Parse(typeof(JewelType), jewelNames[randomIndex]);
                randomJewels[i] = new Jewel(type);
            }
            return (new JewelGroup(randomJewels[0], randomJewels[1], randomJewels[2]));
        }

        /// <summary>
        /// Moves the delta.
        /// </summary>
        /// <param name="movement">The movement.</param>
        /// <param name="logicUpdate">The logic update.</param>
        /// <param name="numPositionsToMove">The number positions to move.</param>
        /// <returns></returns>
        private bool MoveDelta(MovementType movement, GameLogicUpdate logicUpdate, int numPositionsToMove)
        {
            JewelGroup delta = state.Mine.Delta;
            Coordinates targetCoordinates = null;
            switch (movement)
            {
                case MovementType.Down:
                    targetCoordinates = FindClosestDownPositionForDelta(delta, numPositionsToMove); break;
                case MovementType.Left:
                    targetCoordinates = FindClosestLeftPositionForDelta(delta, numPositionsToMove); break;
                case MovementType.Right:
                    targetCoordinates = FindClosestRightPositionForDelta(delta, numPositionsToMove); break;
            }

            if (targetCoordinates == null) return (false);
            // save current positions
            Coordinates originalTop = delta.Top.Coordinates;
            Coordinates originalMiddle = delta.Middle.Coordinates;
            Coordinates originalBottom = delta.Bottom.Coordinates;
            // clear grid
            ClearGridPosition(delta.Top.Coordinates);
            ClearGridPosition(delta.Middle.Coordinates);
            ClearGridPosition(delta.Bottom.Coordinates);
            // shuffle down
            delta.Top.Coordinates = new Coordinates(targetCoordinates.X, targetCoordinates.Y - 2);
            delta.Middle.Coordinates = new Coordinates(targetCoordinates.X, targetCoordinates.Y - 1);
            delta.Bottom.Coordinates = targetCoordinates;
            if (CoordinatesInBounds(delta.Top.Coordinates))
            {
                if (!delta.Top.HasEnteredBounds) delta.Top.HasEnteredBounds = true;
                state.Mine[delta.Top.Coordinates] = delta.Top.Jewel;
                logicUpdate.JewelMovements.Add(new JewelMovement() { Jewel = delta.Top.Jewel, Original = originalTop, New = delta.Top.Coordinates });
            }
            if (CoordinatesInBounds(delta.Middle.Coordinates))
            {
                if (!delta.Middle.HasEnteredBounds) delta.Middle.HasEnteredBounds = true;
                state.Mine[delta.Middle.Coordinates] = delta.Middle.Jewel;
                logicUpdate.JewelMovements.Add(new JewelMovement() { Jewel = delta.Middle.Jewel, Original = originalMiddle, New = delta.Middle.Coordinates });
            }
            if (CoordinatesInBounds(delta.Bottom.Coordinates))
            {
                if (!delta.Bottom.HasEnteredBounds) delta.Bottom.HasEnteredBounds = true;
                state.Mine[delta.Bottom.Coordinates] = delta.Bottom.Jewel;
                logicUpdate.JewelMovements.Add(new JewelMovement() { Jewel = delta.Bottom.Jewel, Original = originalBottom, New = delta.Bottom.Coordinates });
            }
            return (true);
        }

        /// <summary>
        /// Finds the closest down position for delta.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="numPositionsToMove">The number positions to move.</param>
        /// <returns></returns>
        private Coordinates FindClosestDownPositionForDelta(JewelGroup delta, int numPositionsToMove)
        {
            Coordinates result = null;
            for (int i = 1; i <= numPositionsToMove; i++)
            {
                Coordinates newBottom = new Coordinates(delta.Bottom.Coordinates.X, delta.Bottom.Coordinates.Y + i);
                Coordinates newMiddle = new Coordinates(delta.Middle.Coordinates.X, delta.Middle.Coordinates.Y + i);
                Coordinates newTop = new Coordinates(delta.Top.Coordinates.X, delta.Top.Coordinates.Y + i);

                if (CoordinatesInBounds(newBottom) && CoordinatesAvailable(newBottom)
                    && (!delta.Middle.HasEnteredBounds || (CoordinatesInBounds(newMiddle)))
                    && (!delta.Top.HasEnteredBounds || (CoordinatesInBounds(newTop))))
                {
                    result = newBottom;
                }
                else
                {
                    break;
                }
            }
            return (result);
        }

        /// <summary>
        /// Finds the closest left position for delta.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="numPositionsToMove">The number positions to move.</param>
        /// <returns></returns>
        private Coordinates FindClosestLeftPositionForDelta(JewelGroup delta, int numPositionsToMove)
        {
            Coordinates result = null;
            for (int i = 1; i <= numPositionsToMove; i++)
            {
                Coordinates newBottom = new Coordinates(delta.Bottom.Coordinates.X - i, delta.Bottom.Coordinates.Y);
                Coordinates newMiddle = new Coordinates(delta.Middle.Coordinates.X - i, delta.Middle.Coordinates.Y);
                Coordinates newTop = new Coordinates(delta.Top.Coordinates.X - i, delta.Top.Coordinates.Y);

                if (CoordinatesInBounds(newBottom) && CoordinatesAvailable(newBottom)
                    && (!delta.Middle.HasEnteredBounds || (CoordinatesInBounds(newMiddle) && CoordinatesAvailable(newMiddle)))
                    && (!delta.Top.HasEnteredBounds || (CoordinatesInBounds(newTop) && CoordinatesAvailable(newTop))))
                {
                    result = newBottom;
                }
                else
                {
                    break;
                }
            }
            return (result);
        }

        /// <summary>
        /// Finds the closest right position for delta.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="numPositionsToMove">The number positions to move.</param>
        /// <returns></returns>
        private Coordinates FindClosestRightPositionForDelta(JewelGroup delta, int numPositionsToMove)
        {
            Coordinates result = null;
            for (int i = 1; i <= numPositionsToMove; i++)
            {
                Coordinates newBottom = new Coordinates(delta.Bottom.Coordinates.X + i, delta.Bottom.Coordinates.Y);
                Coordinates newMiddle = new Coordinates(delta.Middle.Coordinates.X + i, delta.Middle.Coordinates.Y);
                Coordinates newTop = new Coordinates(delta.Top.Coordinates.X + i, delta.Top.Coordinates.Y);

                if (CoordinatesInBounds(newBottom) && CoordinatesAvailable(newBottom)
                    && (!delta.Middle.HasEnteredBounds || (CoordinatesInBounds(newMiddle) && CoordinatesAvailable(newMiddle)))
                    && (!delta.Top.HasEnteredBounds || (CoordinatesInBounds(newTop) && CoordinatesAvailable(newTop))))
                {
                    result = newBottom;
                }
                else
                {
                    break;
                }
            }
            return (result);
        }

        /// <summary>
        /// Moves the delta.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <param name="movement">The movement.</param>
        /// <param name="logicUpdate">The logic update.</param>
        /// <returns></returns>
        private bool MoveJewel(Coordinates coordinates, MovementType movement, GameLogicUpdate logicUpdate)
        {
            if (coordinates == null) throw new ArgumentException("Argument cannot be null.", "coordinates");
            if (CoordinatesInBounds(coordinates))
            {
                Coordinates targetCoordinates = null;
                switch (movement)
                {
                    case MovementType.Down:
                        targetCoordinates = FindClosestDownPosition(coordinates); break;
                    case MovementType.Left:
                        targetCoordinates = FindClosestLeftPosition(coordinates); break;
                    case MovementType.Right:
                        targetCoordinates = FindClosestRightPosition(coordinates); break;
                }
                if (targetCoordinates == null) return (false);
                // otherwise we can move the delta to the required or closest position
                Jewel target = (Jewel)state.Mine[coordinates];
                state.Mine[targetCoordinates] = target;
                state.Mine[coordinates] = null;
                logicUpdate.JewelMovements.Add(new JewelMovement() { Jewel = target, Original = coordinates, New = targetCoordinates });
                return (true);
            }
            return (false);
        }

        /// <summary>
        /// Finds the closest down position.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="numPositionsToMove">The number positions to move.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Argument cannot be null.;target</exception>
        private Coordinates FindClosestDownPosition(Coordinates target, int numPositionsToMove = 1)
        {
            if (target == null) throw new ArgumentException("Argument cannot be null.", "target");
            Coordinates result = null;
            int? closestY = null;
            for (int i = 1; i <= numPositionsToMove; i++)
            {
                if (CoordinatesInBounds(new Coordinates(target.X, target.Y + i))
                    && CoordinatesAvailable(new Coordinates(target.X, target.Y + i))) closestY = target.Y + i;
                else break;
            }
            if (closestY.HasValue) result = new Coordinates(target.X, closestY.Value);
            return (result);
        }

        /// <summary>
        /// Finds the closest left position.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="numPositionsToMove">The number positions to move.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Argument cannot be null.;target</exception>
        private Coordinates FindClosestLeftPosition(Coordinates target, int numPositionsToMove = 1)
        {
            if (target == null) throw new ArgumentException("Argument cannot be null.", "target");
            Coordinates result = null;
            int? closestX = null;
            for (int i = 1; i <= numPositionsToMove; i++)
            {
                if (CoordinatesInBounds(new Coordinates(target.X - i, target.Y))
                    && CoordinatesAvailable(new Coordinates(target.X - i, target.Y))) closestX = target.X - i;
                else break;
            }
            if (closestX.HasValue) result = new Coordinates(closestX.Value, target.Y);
            return (result);
        }

        /// <summary>
        /// Finds the closest right position.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="numPositionsToMove">The number positions to move.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Argument cannot be null.;target</exception>
        private Coordinates FindClosestRightPosition(Coordinates target, int numPositionsToMove = 1)
        {
            if (target == null) throw new ArgumentException("Argument cannot be null.", "target");
            Coordinates result = null;
            int? closestX = null;
            for (int i = 1; i <= numPositionsToMove; i++)
            {
                if (CoordinatesInBounds(new Coordinates(target.X + i, target.Y))
                    && CoordinatesAvailable(new Coordinates(target.X + i, target.Y))) closestX = target.X + i;
                else break;
            }
            if (closestX.HasValue) result = new Coordinates(closestX.Value, target.Y);
            return (result);
        }

        /// <summary>
        /// Coordinateses the available.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public bool CoordinatesAvailable(Coordinates target)
        {
            if (target == null) throw new ArgumentException("Argument cannot be null.", "target");
            return (state.Mine.Grid[target.X, target.Y] == null);
        }

        /// <summary>
        /// Coordinateses the in bounds.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public bool CoordinatesInBounds(Coordinates target)
        {
            if (target == null) throw new ArgumentException("Argument cannot be null.", "target");
            return (target.X >= 0 && target.X < state.Mine.Columns
                && target.Y >= 0 && target.Y < state.Mine.Depth);
        }

        /// <summary>
        /// Clears the grid position.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        private void ClearGridPosition(Coordinates coordinates)
        {
            if (coordinates != null && CoordinatesInBounds(coordinates))
            {
                state.Mine[coordinates] = null;
            }
        }

    }
}
