using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JewelMine.Engine;

namespace JewelMine.View.Forms
{
    /// <summary>
    /// Constants for use by the view.
    /// </summary>
    public static class ViewConstants
    {
        // Window
        public const int WINDOW_PREFERRED_WIDTH = 790;
        public const int WINDOW_PREFERRED_HEIGHT = 820;
    
        // Images
        public const string IMAGE_RESOURCE_PATTERN_NAMESPACE = "JewelMine.Resources.Images";
        public const string JEWEL_IMAGE_RESOURCE_PATTERN = IMAGE_RESOURCE_PATTERN_NAMESPACE + ".{0}.gif";
        public const string BACKGROUND_IMAGE_RESOURCE_PATTERN = IMAGE_RESOURCE_PATTERN_NAMESPACE + ".{0}.jpg";
        public static string[] BACKGROUND_TEXTURE_NAMES = new string[] { 
            "Background.Cave.1", "Background.Cave.2", 
            "Background.Cave.3", "Background.Cave.4", 
            "Background.Stone.1", "Background.Stone.2", 
            "Background.Stone.3" };
        
        // Music
        public const string BACKGROUND_MUSIC_FILENAME = "Music/Ambient.wav";
        
        // Sounds
        public const string SOUND_COLLISION_FILENAME = "Sounds/Collision.wav";
        public const string SOUND_SWAP_FILENAME = "Sounds/Swap.wav";
        public const string SOUND_STATIONARY_FILENAME = "Sounds/Stationary.wav";
        public const string SOUND_LEVELUP_FILENAME = "Sounds/LevelUp.wav";
        
        // Text
        public const string GAME_PAUSED_TEXT = "PAUSED";
        public const string GAME_PAUSED_SUBTEXT = "Press any key...";

        public const string GAME_OVER_TEXT = "GAME OVER!";
        public const string GAME_OVER_SUBTEXT = "Press CTRL+R to restart or CTLR+Q to quit.";

        public const string GAME_WON_TEXT = "GAME WON! CONGRATS!";
        public const string GAME_WON_SUBTEXT = "Press CTLR+R to restart or CTLR+Q to quit.";

        public const string GAME_START_TEXT = "JEWEL MINE";
        public const string GAME_START_SUBTEXT = "Press any key...";

        public const string SCORE_PATTERN = "Score//{0}";
        public const string LEVEL_PATTERN = "Level//{0}";
    }
}
