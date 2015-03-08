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
        public const string IMAGE_RESOURCE_PATTERN = "JewelMine.Resources.Images";
        public const string BACKGROUND_IMAGE_RESOURCE_PATTERN = IMAGE_RESOURCE_PATTERN + ".{0}.jpg";
        public const string JEWEL_IMAGE_RESOURCE_PATTERN = IMAGE_RESOURCE_PATTERN + ".{0}.gif";
        public static string[] BACKGROUND_TEXTURE_NAMES = new string[] { 
            "Background.Cave.1", "Background.Cave.2", 
            "Background.Cave.3", "Background.Cave.4", 
            "Background.Stone.1", "Background.Stone.2", 
            "Background.Stone.3" };
    }
}
