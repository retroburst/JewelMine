﻿using System;
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
        public const int WINDOW_PREFERRED_WIDTH = 733;
        public const int WINDOW_PREFERRED_HEIGHT = 755;
        public const int DEBUG_RECTANGLE_WIDTH = 280;
        public const int DEBUG_RECTANGLE_HEIGHT_OFFSET = 50;
        public const int DEBUG_RECTANGLE_PADDING = 5;
        public const int MESSAGE_Y_OFFSET = 30;
        public const int DEFAULT_X_OFFSET = 5;
        public const int DEFAULT_Y_OFFSET = 5;

        // Images
        public const string IMAGE_RESOURCE_PATTERN_NAMESPACE = "JewelMine.Resources.Images";
        public const string JEWEL_IMAGE_RESOURCE_PATTERN = IMAGE_RESOURCE_PATTERN_NAMESPACE + ".{0}.gif";
        public const string BACKGROUND_IMAGE_RESOURCE_PATTERN = IMAGE_RESOURCE_PATTERN_NAMESPACE + ".{0}.jpg";
        public static string[] BACKGROUND_TEXTURE_NAMES = new string[] { 
            "Background.Cave.1", 
            "Background.Cave.2", 
            "Background.Cave.3", 
            "Background.Cave.4", 
            "Background.Stone.1", 
            "Background.Stone.2", 
            "Background.Stone.3" 
        };

        // Text
        public static readonly TimeSpan GAME_MESSAGE_VISIBLE_TIME = new TimeSpan(0, 0, 2);
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
        public const string SCORE_FORMAT_STRING = "00000000";
    }
}
