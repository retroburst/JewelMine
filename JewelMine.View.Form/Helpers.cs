using JewelMine.Engine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.View.Forms
{
    /// <summary>
    /// Utility helper methods for game view.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Resizes the image.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="maximumWidth">The maximum width.</param>
        /// <param name="maximumHeight">The maximum height.</param>
        /// <param name="enforceRatio">if set to <c>true</c> [enforce ratio].</param>
        /// <param name="addPadding">if set to <c>true</c> [add padding].</param>
        /// <returns></returns>
        public static Bitmap ResizeImage(Bitmap source, int maximumWidth, int maximumHeight, bool enforceRatio, bool addPadding)
        {
            ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 50L);
            int canvasWidth = maximumWidth;
            int canvasHeight = maximumHeight;
            int newImageWidth = maximumWidth;
            int newImageHeight = maximumHeight;
            int xPosition = 0;
            int yPosition = 0;

            if (enforceRatio)
            {
                var ratioX = maximumWidth / (double)source.Width;
                var ratioY = maximumHeight / (double)source.Height;
                var ratio = ratioX < ratioY ? ratioX : ratioY;
                newImageHeight = (int)(source.Height * ratio);
                newImageWidth = (int)(source.Width * ratio);

                if (addPadding)
                {
                    xPosition = (int)((maximumWidth - (source.Width * ratio)) / 2);
                    yPosition = (int)((maximumHeight - (source.Height * ratio)) / 2);
                }
                else
                {
                    canvasWidth = newImageWidth;
                    canvasHeight = newImageHeight;
                }
            }

            Bitmap target = new Bitmap(canvasWidth, canvasHeight);
            Graphics graphics = Graphics.FromImage(target);

            if (enforceRatio && addPadding)
            {
                graphics.Clear(Color.Transparent);
            }

            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.DrawImage(source, xPosition, yPosition, newImageWidth, newImageHeight);

            return (target);
        }

        /// <summary>
        /// Generates the image resource dictionary.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<JewelType, Bitmap> GenerateJewelImageResourceDictionary()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            Dictionary<JewelType, Bitmap> result = new Dictionary<JewelType, Bitmap>();
            string[] names = Enum.GetNames(typeof(JewelType)).Where(x => x != JewelType.Unknown.ToString()).ToArray();
            foreach (string name in names)
            {
                JewelType type = (JewelType)Enum.Parse(typeof(JewelType), name);
                string resourceName = string.Format(ViewConstants.JEWEL_IMAGE_RESOURCE_PATTERN, name);
                Bitmap bitmap = new Bitmap(a.GetManifestResourceStream(resourceName), true);
                result.Add(type, bitmap);
            }
            return (result);
        }

        /// <summary>
        /// Generates the background image array.
        /// </summary>
        /// <returns></returns>
        public static Bitmap[] GenerateBackgroundImageArray()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            List<Bitmap> result = new List<Bitmap>();
            string[] names = ViewConstants.BACKGROUND_TEXTURE_NAMES;
            foreach (string name in names)
            {
                string resourceName = string.Format(ViewConstants.BACKGROUND_IMAGE_RESOURCE_PATTERN, name);
                Bitmap bitmap = new Bitmap(a.GetManifestResourceStream(resourceName), true);
                result.Add(bitmap);
            }
            return (result.ToArray());
        }

        /// <summary>
        /// Generates the random index.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="rand">The rand.</param>
        /// <returns></returns>
        public static int GenerateRandomIndex(Array target, Random rand)
        {
            int result = 0;
            if(target != null && target.Length > 0)
            {
                result = rand.Next(0, target.Length - 1);
            }
            return (result);
        }

        /// <summary>
        /// Generates the resized jewel image resource dictionary.
        /// </summary>
        /// <param name="resources">The resources.</param>
        /// <param name="cellWidth">Width of the cell.</param>
        /// <param name="cellHeight">Height of the cell.</param>
        /// <param name="bitmapOffset">The bitmap offset.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static Dictionary<JewelType, Bitmap> GenerateResizedJewelImageResourceDictionary(Dictionary<JewelType, Bitmap> resources, int cellWidth, int cellHeight, int bitmapOffset)
        {
            Dictionary<JewelType, Bitmap> result = new Dictionary<JewelType, Bitmap>();
            foreach(var pair in resources)
            {
                Bitmap resized = ResizeImage(pair.Value, cellWidth - bitmapOffset, cellHeight - bitmapOffset, true, true);
                result.Add(pair.Key, resized);
            }
            return (result);
        }
    }
}
