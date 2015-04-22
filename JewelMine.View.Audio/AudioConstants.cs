using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JewelMine.Engine;

namespace JewelMine.View.Audio
{
    /// <summary>
    /// Constants for use by the audio system.
    /// </summary>
    public static class AudioConstants
    {
        // Music
        public const string BACKGROUND_MUSIC_FILENAME = "Music/Ambient.wav";

        // Sounds
        public const string SOUND_COLLISION_FILENAME = "Sounds/Collision.wav";
        public const string SOUND_SWAP_FILENAME = "Sounds/Swap.wav";
        public const string SOUND_STATIONARY_FILENAME = "Sounds/Stationary.wav";
        public const string SOUND_LEVELUP_FILENAME = "Sounds/LevelUp.wav";

        // Messages
        public const string GAME_MESSAGE_TOGGLE_MUSIC_PATTERN = "Music {0}";
        public const string GAME_TOGGLE_SOUND_PATTERN = "Sound {0}";
    }
}
