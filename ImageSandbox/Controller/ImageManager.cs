using System;
using Windows.UI;
using GroupKStegafy.Model;

namespace GroupKStegafy.Controller
{
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

        public void embedImage(byte[] sourcePixels, uint imageWidth, uint imageHeight)
        {
            for (var i = 0; i < imageHeight; i++)
            {
                for (var j = 0; j < imageWidth; j++)
                {

                    var pixelColor = this.getPixelBgra8(sourcePixels, i, j, imageWidth, imageHeight);
                    var monoColor = this.getPixelBgra8(this.MonoImage.Pixels, i, j,
                        Convert.ToUInt32(this.MonoImage.ImageWidth), Convert.ToUInt32(this.MonoImage.ImageHeight));

                    if (j == 0 && i == 0)
                    {
                        //set header to 212 values for R,G,B
                    }
                    else if (j == 1 && i == 0)
                    {
                        pixelColor.R &= 0xfe;
                    } else if (monoColor.R == 0)
                    {
                        pixelColor.B &= 0xfe;
                    }
                    else
                    {
                        pixelColor.B |= 1;
                    }

                    this.setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth, imageHeight);
                }
            }
        }

        public void extractSecretImage(byte[] sourcePixels, uint imageWidth, uint imageHeight)
        {
            for (var i = 0; i < imageHeight; i++)
            {
                for (var j = 0; j < imageWidth; j++)
                {
                    var pixelColor = this.getPixelBgra8(sourcePixels, i, j, imageWidth, imageHeight);

                    if (j == 0 && i == 0)
                    {
                        continue;
                    }

                    if (j == 1 && i == 0)
                    {
                        continue;
                    }

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

        private void giveImageRedTint(byte[] sourcePixels, uint imageWidth, uint imageHeight)
        {
            for (var i = 0; i < imageHeight; i++)
            {
                for (var j = 0; j < imageWidth; j++)
                {
                    var pixelColor = this.getPixelBgra8(sourcePixels, i, j, imageWidth, imageHeight);

                    pixelColor.R = 255;

                    this.setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth, imageHeight);
                }
            }
        }

        #endregion
    }
}