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
        public const string MUSIC_RESOURCE_PATTERN_NAMESPACE = "JewelMine.Resources.Music";
        public const string MUSIC_RESOURCE_PATTERN = MUSIC_RESOURCE_PATTERN_NAMESPACE + ".{0}.mp3";
        public const string BACKGROUND_MUSIC_TRACK_NAME = "Columns";
        
        // Sounds
        public const string SOUNDS_RESOURCE_PATTERN_NAMESPACE = "JewelMine.Resources.Sounds";
        public const string SOUND_RESOURCE_PATTERN = SOUNDS_RESOURCE_PATTERN_NAMESPACE + ".{0}.mp3";
        public const string COLLISION_SOUND_NAME = "Collision";
        public const string SWAP_SOUND_NAME = "Swap";
        public const string TICK_SOUND_NAME = "Tick";
    }
}
