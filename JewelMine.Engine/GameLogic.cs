﻿using JewelMine.Engine.Models;
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
        public event EventHandler<GameStateModel> GameStateChanged;
        public Random Random { get; private set; }
        private string[] jewelNames = null;
        private bool tickerActive = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameLogic"/> class.
        /// </summary>
        public GameLogic()
        {
            jewelNames = Enum.GetNames(typeof(JewelType)).Where(x => x != JewelType.Unknown.ToString()).ToArray();
            GameStateModel = new GameStateModel();
            GameTicker = new Timer(GameStateModel.GameTickSpeedMilliseconds);
            GameTicker.Elapsed += GameTickEventHandler;
            Random = new Random();
        }

        // TODO update game states
        /// <summary>
        /// Starts the game.
        /// </summary>
        public void StartGame()
        {
            GameTicker.Start();
        }

        /// <summary>
        /// Stops the game.
        /// </summary>
        public void StopGame()
        {
            GameTicker.Stop();
        }

        /// <summary>
        /// Handles the game tick event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void GameTickEventHandler(object sender, ElapsedEventArgs e)
        {
            if (tickerActive) return;
            tickerActive = true;
            if(GameStateModel.MineModel.Delta == null)
            {
                bool added = AddDeltaJewel();
                if (!added) { throw new NotImplementedException("Game Over"); }
            }
            else
            {
                // move delta down
                bool moved = MoveJewel(GameStateModel.MineModel.DeltaX, GameStateModel.MineModel.DeltaY, MovementType.Down, true);
                if(!moved)
                {
                    GameStateModel.MineModel.Delta = null;
                    GameStateModel.MineModel.DeltaX = 0;
                    GameStateModel.MineModel.DeltaY = 0;
                    bool added = AddDeltaJewel();
                    //TODO
                    if (!added) { throw new NotImplementedException("Game Over"); }
                }
            }
            if (GameStateChanged != null) GameStateChanged(this, GameStateModel);
            tickerActive = false;
        }

        private Timer GameTicker { get; set; }
        public GameStateModel GameStateModel { get; private set; }

        /// <summary>
        /// Adds the delta jewel.
        /// </summary>
        /// <returns></returns>
        private bool AddDeltaJewel()
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
        /// <returns></returns>
        private bool MoveJewel(int x, int y, MovementType movement, bool isDelta)
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
                    MineObjectModel target = GameStateModel.MineModel.Mine[x, y];
                    GameStateModel.MineModel.Mine[targetX, targetY] = target;
                    GameStateModel.MineModel.Mine[x, y] = null;
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
