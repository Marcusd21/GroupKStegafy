using System;
using Windows.UI;
using GroupKStegafy.Model;

namespace GroupKStegafy.Utility
{
    /// <summary>
    ///     Handles Image Embedding and Extraction
    /// </summary>
    public static class ImageManager
    {
        #region Methods

        /// <summary>
        ///     Gets the pixel bgra8.
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        /// <param name="pixels">The pixels.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Color object from pixel bgra8</returns>
        private static Color getPixelBgra8(byte[] pixels, int x, int y, uint width, uint height)
        {
            var offset = (x * (int) width + y) * 4;

            if (offset >= width * height * 4)
            {
                return Color.FromArgb(0, 255, 255, 255);
            }

            var r = pixels[offset + 2];
            var g = pixels[offset + 1];
            var b = pixels[offset + 0];
            return Color.FromArgb(0, r, g, b);
        }

        /// <summary>
        ///     Sets the pixel bgra8.
        ///     Precondition: none
        ///     Postcondition: pixel bgra8 is setup
        /// </summary>
        /// <param name="pixels">The pixels.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        private static void setPixelBgra8(byte[] pixels, int x, int y, Color color, uint width)
        {
            var offset = (x * (int) width + y) * 4;
            pixels[offset + 2] = color.R;
            pixels[offset + 1] = color.G;
            pixels[offset + 0] = color.B;
        }

        /// <summary>
        ///     Embeds the source image with the monochrome image.
        ///     Precondition: none
        ///     Postcondition: mono image embedded into source
        /// </summary>
        /// <param name="sourcePixels"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <param name="monoImage"></param>
        public static void EmbedImage(byte[] sourcePixels, uint imageWidth, uint imageHeight, Image monoImage)
        {
            for (var i = 0; i < imageHeight; i++)
            {
                for (var j = 0; j < imageWidth; j++)
                {
                    if (i < monoImage.ImageHeight && j < monoImage.ImageWidth)
                    {
                        var pixelColor = getPixelBgra8(sourcePixels, i, j, imageWidth, imageHeight);
                        var monoColor = getPixelBgra8(monoImage.Pixels, i, j,
                            Convert.ToUInt32(monoImage.ImageWidth), Convert.ToUInt32(monoImage.ImageHeight));
                        if (monoColor.R == 0)
                        {
                            pixelColor.B &= 0xfe;

                            setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth);
                        }
                        else
                        {
                            pixelColor.B |= 1;
                            setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Determines whether [is image exceed source] [the specified image width].
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageHeight">Height of the image.</param>
        /// <param name="monoImage"></param>
        /// <returns>
        ///     <c>true</c> if [is image exceed source] [the specified image width]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsImageExceedSource(uint imageWidth, uint imageHeight, Image monoImage)
        {
            return monoImage.ImageHeight > imageHeight && monoImage.ImageWidth > imageWidth;
        }

        /// <summary>
        ///     Determines whether [is image secret message] [the specified source pixels].
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        /// <param name="sourcePixels">The source pixels.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageHeight">Height of the image.</param>
        /// <returns>
        ///     <c>true</c> if [is image secret message] [the specified source pixels]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsImageSecretMessage(byte[] sourcePixels, uint imageWidth, uint imageHeight)
        {
            var pixelColor = getPixelBgra8(sourcePixels, 0, 0, imageWidth, imageHeight);
            return pixelColor.B == 212 && pixelColor.R == 212 && pixelColor.G == 212;
        }

        /// <summary>
        ///     Extracts the image message from embedded image.
        ///     Precondition: none
        ///     Postcondition: secret image is extracted
        /// </summary>
        /// <param name="sourcePixels"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        public static void ExtractSecretImage(byte[] sourcePixels, uint imageWidth, uint imageHeight)
        {
            for (var i = 0; i < imageHeight; i++)
            {
                for (var j = 0; j < imageWidth; j++)
                {
                    var pixelColor = getPixelBgra8(sourcePixels, i, j, imageWidth, imageHeight);

                    if (pixelColor.B % 2 == 0)
                    {
                        pixelColor.B = 0;
                        pixelColor.R = 0;
                        pixelColor.G = 0;
                    }
                    else
                    {
                        pixelColor.B = 255;
                        pixelColor.R = 255;
                        pixelColor.G = 255;
                    }

                    setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth);
                }
            }
        }

        #endregion
    }
}