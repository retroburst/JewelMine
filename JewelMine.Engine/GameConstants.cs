﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine
{
    /// <summary>
    /// Constants for game logic and state.
    /// </summary>
    public static class GameConstants
    {
        public const int MINE_DEFAULT_COLUMN_SIZE = 16;
        public const int MINE_DEFAULT_DEPTH_SIZE = 16;
        public const int GAME_DEFAULT_LEVEL = 1;
        public const int GAME_LEVEL_INCREMENT_SCORE_THRESHOLD = 50000;
        public const int GAME_LEVEL_INCREMENT_SPEED_CHANGE = 10;
        public const double GAME_DEFAULT_TICK_SPEED_MILLISECONDS = 280.0d;
        public const int GAME_COLLISION_FINALISE_TICK_COUNT = 20;
        public const int GAME_DELTA_STATIONARY_TICK_COUNT = 5;
        public const int GAME_DOUBLE_JEWEL_DELTA_CHANCE_ABOVE = 70;
        public const int GAME_TRIPLE_JEWEL_DELTA_CHANCE_ABOVE = 90; 
    }

    /// <summary>
    /// State of gameplay.
    /// </summary>
    public enum GamePlayState
    {
        NotStarted,
        Playing,
        Paused,
        GameOver
    }

    /// <summary>
    /// All available wall types.
    /// </summary>
    public enum WallType
    {
        Unknown,
        Stone,
        Metal,
        Wood
    }

    /// <summary>
    /// All available delta types.
    /// </summary>
    public enum JewelType
    {
        Unknown,
        Diamond,
        Sapphire,
        Emerald,
        Topaz,
        Ruby,
        Amethyst,
        Citrine,
        SpessartiteGarnet
    }

    /// <summary>
    /// Movement directions for delta.
    /// </summary>
    public enum MovementType
    {
        Down,
        Left,
        Right
    }

    /// <summary>
    /// Collision directions.
    /// </summary>
    public enum CollisionDirection
    {
        Vertical,
        Horizontal,
        DiagonallyLeft,
        DiagonallyRight
    }
}
