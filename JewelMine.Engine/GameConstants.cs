using System;
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
        public const double GAME_DEFAULT_TICK_SPEED_MILLISECONDS = 125.00;
        public const int GAME_DEFAULT_LEVEL_INITIAL_LINES = 3;
        


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
}
