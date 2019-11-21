﻿using System;
using Windows.UI;
using GroupKStegafy.Model;

namespace GroupKStegafy.Controller
{
    /// <summary>
    /// 
    /// </summary>
    public class ImageManager
    {
        #region Data members

        /// <summary>The mono image</summary>
        public Image MonoImage;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="ImageManager" /> class.</summary>
        public ImageManager()
        {
            this.MonoImage = new Image();
        }

        #endregion

        #region Methods

        /// <summary>Gets the pixel bgra8.</summary>
        /// <param name="pixels">The pixels.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        private Color getPixelBgra8(byte[] pixels, int x, int y, uint width, uint height)
        {
            var offset = (x * (int) width + y) * 4;

            if (offset >= (width * height * 4))
            {
                return Color.FromArgb(0, 255, 255, 255);
            }

            var r = pixels[offset + 2];
            var g = pixels[offset + 1];
            var b = pixels[offset + 0];
            return Color.FromArgb(0, r, g, b);
        }

        /// <summary>Sets the pixel bgra8.</summary>
        /// <param name="pixels">The pixels.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        private void setPixelBgra8(byte[] pixels, int x, int y, Color color, uint width, uint height)
        {
            var offset = (x * (int) width + y) * 4;
            pixels[offset + 2] = color.R;
            pixels[offset + 1] = color.G;
            pixels[offset + 0] = color.B;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcePixels"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        public void EmbedImage(byte[] sourcePixels, uint imageWidth, uint imageHeight)
        {
            for (var i = 0; i < imageHeight; i++)
            {
                for (var j = 0; j < imageWidth; j++)
                {
                    if (i < this.MonoImage.ImageHeight && j < this.MonoImage.ImageWidth)
                    {
                        var pixelColor = this.getPixelBgra8(sourcePixels, i, j, imageWidth, imageHeight);
                        var monoColor = this.getPixelBgra8(this.MonoImage.Pixels, i, j,
                            Convert.ToUInt32(this.MonoImage.ImageWidth), Convert.ToUInt32(this.MonoImage.ImageHeight));
                        if (monoColor.R == 0)
                        {
                            pixelColor.B &= 0xfe;

                            this.setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth, imageHeight);
                        }
                        else
                        {
                            pixelColor.B |= 1;

                            this.setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth, imageHeight);
                        }

                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcePixels"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        public void ExtractSecretImage(byte[] sourcePixels, uint imageWidth, uint imageHeight)
        {
            for (var i = 0; i < imageHeight; i++)
            {
                for (var j = 0; j < imageWidth; j++)
                {
                    var pixelColor = this.getPixelBgra8(sourcePixels, i, j, imageWidth, imageHeight);

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

                    this.setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth, imageHeight);
                }
            }
        }

       

        #endregion
    }
}