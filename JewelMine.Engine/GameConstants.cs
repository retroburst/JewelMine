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
        public const int GAME_NUM_JEWELS_FOR_GROUP_COLLISION = 3;
        public const string GAME_MESSAGE_SAVE_GAME_PATTERN = "Saved {0}";
        public const string GAME_MESSAGE_LOAD_GAME_PATTERN = "Loaded from {0}";
        public const string GAME_MESSAGE_SAVE_GAME_FAILED_PATTERN = "Save failed [{0}]";
        public const string GAME_MESSAGE_LOAD_GAME_FAILED_PATTERN = "Loaded failed [{0}]";
        public const string GAME_MESSAGE_CHANGED_DIFFICULTY_PATTERN = "Difficulty set to {0} [{1:N0} Levels]";
        public const string GAME_MESSAGE_RESTARTED = "Restarted";
        public const string GAME_MESSAGE_POINTS_SCORED_PATTERN = "{0:N0} points scored";
        public const string GAME_DEFAULT_SAVE_GAME_FILENAME = "Saved.Game.data";
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
