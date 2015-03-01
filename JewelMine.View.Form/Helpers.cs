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
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
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
        public static Dictionary<JewelType, Bitmap> GenerateImageResourceDictionary()
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

        public static Bitmap GetBackgroundImage()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            string resourceName = "JewelMine.View.Forms.Resources.Cave.jpg";
            Bitmap bitmap = new Bitmap(a.GetManifestResourceStream(resourceName), true);
            return (bitmap);
        }

    }
}
