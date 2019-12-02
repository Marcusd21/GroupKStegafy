using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.UI;

namespace GroupKStegafy.Controller
{
    /// <summary>Instance to update text</summary>
    public class TextManager
    {
        private List<byte> textBytes;

        /// <summary>Gets the pixel bgra8.</summary>
        /// <param name="pixels">The pixels.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        private Color getPixelBgra8(byte[] pixels, int x, int y, uint width, uint height)
        {
            var offset = (x * (int)width + y) * 4;

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
            var offset = (x * (int)width + y) * 4;
            pixels[offset + 2] = color.R;
            pixels[offset + 1] = color.G;
            pixels[offset + 0] = color.B;
        }

        /// <summary>Embeds the text.</summary>
        /// <param name="sourcePixels">The source pixels.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageHeight">Height of the image.</param>
        /// <param name="text">The text.</param>
        public void EmbedText(byte[] sourcePixels, uint imageWidth, uint imageHeight, string text, int bpcc)
        {
           this.textBytes =  this.convertTextToBytes(text);
           var current = 0;
           
            for (var i = 0; i < imageHeight; i++)
            {
                for (var j = 0; j < imageWidth; j++)
                {

                    var pixelColor = this.getPixelBgra8(sourcePixels, i, j, imageWidth, imageHeight);
                    if (j == 0 && i == 0)
                    {
                        pixelColor.B = 212;
                        pixelColor.R = 212;
                        pixelColor.G = 212;
                        this.setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth, imageHeight);
                    }
                    else if (j == 1 && i == 0)
                    {
                        pixelColor.R = 1;
                        pixelColor.G = 1;
                        pixelColor.B = 0;
                        this.setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth, imageHeight);
                    }

                   else if (this.textBytes[current] < this.textBytes.Count)
                    {
                        
                        pixelColor.B &= this.textBytes[current];
                        

                        this.setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth, imageHeight);
                        current++;
                    }
                   


                }
            }
        }

        /// <summary>Extracts the secret text.</summary>
        /// <param name="sourcePixels">The source pixels.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageHeight">Height of the image.</param>
        public void ExtractSecretText(byte[] sourcePixels, uint imageWidth, uint imageHeight)
        {
           var item = convertTextToBytes(this.textBytes);
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
        private List<byte> convertTextToBytes(string text)
        {
            var bytes = new List<byte>();

            foreach (var item in text)
            {
                bytes.Add(Convert.ToByte(item));
            }

            return bytes;
        }

        private List<char> convertTextToBytes(List<byte> text)
        {
            var bytes = new List<char>();

            foreach (var item in text)
            {
                bytes.Add(Convert.ToChar(item));
            }

            return bytes;
        }

    }
}
