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
        public GameStateModel GameStateModel { get; private set; }
        public Random Random { get; private set; }
        private string[] jewelNames = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameLogic"/> class.
        /// </summary>
        public GameLogic()
        {
            jewelNames = Enum.GetNames(typeof(JewelType)).Where(x => x != JewelType.Unknown.ToString()).ToArray();
            GameStateModel = new GameStateModel();
            Random = new Random();
        }

        // TODO update game states
        /// <summary>
        /// Starts the game.
        /// </summary>
        public void StartGame()
        {
            GameStateModel.GamePlayState = GamePlayState.Playing;
        }

        /// <summary>
        /// Stops the game.
        /// </summary>
        public void StopGame()
        {
            GameStateModel.GamePlayState = GamePlayState.NotStarted;
        }

        /// <summary>
        /// Pauses the game.
        /// </summary>
        public void PauseGame()
        {
            GameStateModel.GamePlayState = GamePlayState.Paused;
        }

        /// <summary>
        /// Games the over.
        /// </summary>
        public void GameOver()
        {
            GameStateModel.GamePlayState = GamePlayState.GameOver;
        }

        /// <summary>
        /// Performs the game logic.
        /// </summary>
        /// <param name="inputBuffer">The input buffer.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">
        /// Game Over
        /// or
        /// Game Over
        /// </exception>
        /// <exception cref="System.NotImplementedException">Game Over
        /// or
        /// Game Over</exception>
        public GameLogicUpdate PerformGameLogic(Queue<MovementType> inputBuffer)
        {
            GameLogicUpdate logicUpdate = new GameLogicUpdate();
            // check for blocks that need to move down

            // check for collisions

            if(GameStateModel.MineModel.Delta == null)
            {
                bool added = AddDeltaJewel(logicUpdate);
                if (!added) { throw new NotImplementedException("Game Over"); }
            }
            else
            {
                // move delta based on input or down by default if no input
                MovementType deltaMovement = MovementType.Down;
                if (inputBuffer.Count == 0) inputBuffer.Enqueue(MovementType.Down);

                
                //TODO: if delta is up against another block or bounadry on underside ignore input - do not move delta and add new delta 
                foreach (var movement in inputBuffer)
                {
                    deltaMovement = movement;
                    // if delta is up against a boundary on either side that the movement is towards, move delta down instead
                    if (deltaMovement == MovementType.Left && GameStateModel.MineModel.DeltaX == 0) deltaMovement = MovementType.Down;
                    else if (deltaMovement == MovementType.Right && GameStateModel.MineModel.DeltaX == GameStateModel.MineModel.Mine.GetUpperBound(0)) deltaMovement = MovementType.Down;


                    bool moved = MoveJewel(GameStateModel.MineModel.DeltaX, GameStateModel.MineModel.DeltaY, deltaMovement, true, logicUpdate);
                    if (!moved && deltaMovement == MovementType.Down)
                    {
                        GameStateModel.MineModel.Delta = null;
                        GameStateModel.MineModel.DeltaX = 0;
                        GameStateModel.MineModel.DeltaY = 0;
                        bool added = AddDeltaJewel(logicUpdate);
                        //TODO
                        if (!added) { throw new NotImplementedException("Game Over"); }
                    }
                }
            }
            return (logicUpdate);
        }

        /// <summary>
        /// Adds the delta jewel.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        /// <returns></returns>
        private bool AddDeltaJewel(GameLogicUpdate logicUpdate)
        {
            int[] free = FindFreeCoordinatesForDelta();
            if (free.Length == 0)
            {
                return (false);
            }
            int randomIndex = Random.Next(0, free.Length);
            int targetCoorindinate = free[randomIndex];
            JewelModel jewel = GenerateRandomDeltaJewel();
            GameStateModel.MineModel.Delta = jewel;
            GameStateModel.MineModel.Mine[targetCoorindinate, 0] = jewel;
            GameStateModel.MineModel.DeltaX = targetCoorindinate;
            GameStateModel.MineModel.DeltaY = 0;
            logicUpdate.NewJewels.Add(new NewJewel() { Jewel = jewel, X = targetCoorindinate, Y = 0 });
            return (true);
        }

        /// <summary>
        /// Finds the free coordinates for delta.
        /// </summary>
        /// <returns></returns>
        private int[] FindFreeCoordinatesForDelta()
        {
            List<int> free = new List<int>();
            for(int i = 0; i < GameStateModel.MineModel.Columns; i++)
            {
                if (GameStateModel.MineModel.Mine[i, 0] == null) free.Add(i);
            }
            return(free.ToArray());
        }

        /// <summary>
        /// Generates the random delta jewel.
        /// </summary>
        /// <returns></returns>
        private JewelModel GenerateRandomDeltaJewel()
        {
            int randomIndex = Random.Next(0, jewelNames.Length);
            JewelType type = (JewelType)Enum.Parse(typeof(JewelType), jewelNames[randomIndex]);
            JewelModel jewel = new JewelModel(type);
            return (jewel);
        }

        /// <summary>
        /// Moves the jewel.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="movement">The movement.</param>
        /// <param name="isDelta">if set to <c>true</c> [is delta].</param>
        /// <param name="logicUpdate">The logic update.</param>
        /// <returns></returns>
        private bool MoveJewel(int x, int y, MovementType movement, bool isDelta, GameLogicUpdate logicUpdate)
        {
            bool moved = false;
            if (CoordinatesInBounds(x, y))
            {
                int targetX = x;
                int targetY = y;

                switch (movement)
                {
                    case MovementType.Down:
                        if (CoordinatesInBounds(x, y + 1)) targetY++; break;
                    case MovementType.Left:
                        if (CoordinatesInBounds(x-1, y)) targetX--; break;
                    case MovementType.Right:
                        if (CoordinatesInBounds(x + 1, y)) targetX++; break;
                }
                if (CoordinatesInBounds(targetX, targetY) && CoordinatesAvailable(targetX, targetY))
                {
                    JewelModel target = (JewelModel)GameStateModel.MineModel.Mine[x, y];
                    GameStateModel.MineModel.Mine[targetX, targetY] = target;
                    GameStateModel.MineModel.Mine[x, y] = null;
                    logicUpdate.JewelMovements.Add(new JewelMovement() { Jewel = target, OriginalX = x, OriginalY = y, NewX = targetX, NewY = targetY });
                    moved = true;
                    if(isDelta)
                    {
                        GameStateModel.MineModel.DeltaX = targetX;
                        GameStateModel.MineModel.DeltaY = targetY;
                    }
                }
            }
            return (moved);
        }

        /// <summary>
        /// Coordinateses the available.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        private bool CoordinatesAvailable(int x, int y)
        {
            return (GameStateModel.MineModel.Mine[x, y] == null);
        }

        /// <summary>
        /// Coordinateses the in bounds.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        private bool CoordinatesInBounds(int x, int y)
        {
            return (x >= 0 && x < GameStateModel.MineModel.Columns
                && y >= 0 && y < GameStateModel.MineModel.Depth);
        }

    }
}
