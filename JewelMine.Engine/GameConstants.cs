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
        public const int GAME_MINE_DEFAULT_COLUMN_SIZE = 21;
        public const int GAME_MINE_DEFAULT_DEPTH_SIZE = 21;
        public const int GAME_DEFAULT_LEVEL = 1;
        public const int GAME_MINE_DEFAULT_INITIAL_LINES = 5;
    }

    /// <summary>
    /// State of gameplay.
    /// </summary>
    public enum GamePlayState
    {
        NotStarted,
        Playing,
        Paused,
        GameOver,
        GameWon
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

    /// <summary>
    /// Difficulty level settings.
    /// </summary>
    public enum DifficultyLevel
    {
        Easy,
        Moderate,
        Hard,
        Impossible
    }
}
