﻿using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;

namespace GroupKStegafy.Controller
{
    /// <summary>Instance to update text</summary>
    public class TextManager
    {
        #region Data members

        private List<byte> textBytes;

        private readonly int[] item = {0X1, 0X2, 0X4, 0X8, 0X10, 0X20, 0X40, 0X80, 0X100};

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

            if (offset >= width * height * 4)
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

        /// <summary>Embeds the text.</summary>
        /// <param name="sourcePixels">The source pixels.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageHeight">Height of the image.</param>
        /// <param name="text">The text.</param>
        public void EmbedText(byte[] sourcePixels, uint imageWidth, uint imageHeight, string text, int bpcc)
        {
            this.textBytes = this.convertTextToBytes(text);
            var current = 0;
            var byteAmount = Convert.ToByte(bpcc);
            var bitChecked = 0;
            var bit = "";
            var pixelBit = "";
            for (var i = 0; i < imageHeight; i++)
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
                    pixelColor.G = byteAmount;
                    pixelColor.B = 0;
                    this.setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth, imageHeight);
                }
                else if (current < this.textBytes.Count && bitChecked % 8 == 0)
                {
                    bitChecked = 0;
                    var result = "";
                    var pixel = "";
                    bit = Convert.ToString(this.textBytes[current], 2).PadLeft(8, '0');
                    pixelBit = Convert.ToString(pixelColor.B, 2).PadLeft(8, '0');
                    for (var k = bit.Length - 1; k > bit.Length - bpcc - 1; k--)
                    {
                        if (bitChecked < 8 && bitChecked < bitChecked + bpcc)
                        {
                            result += bit[k];
                            bitChecked++;
                        }
                    }

                    for (var k = 0; k < pixelBit.Length; k++)
                    {
                        if (k < pixelBit.Length - bitChecked)
                        {
                            pixel += pixelBit[k];
                        }
                    }

                    var answer = pixel + result;

                    var value = Convert.ToInt32(answer, 2);
                    var bite = Convert.ToByte(value);

                    pixelColor.B |= bite;

                    this.setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth, imageHeight);
                    current++;
                }
                else if (current < this.textBytes.Count && bitChecked % 8 != 0)
                {
                    var result = "";
                    var pixel = "";

                    for (var k = bit.Length; k >= bit.Length - bitChecked; k--)
                    {
                        if (bitChecked < 8 && bitChecked <= bitChecked + bpcc)
                        {
                            result += bit[k - 1];
                        }
                    }

                    bitChecked++;

                    for (var k = 0; k < pixelBit.Length; k++)
                    {
                        if (k < pixelBit.Length - bitChecked)
                        {
                            pixel += pixelBit[k];
                        }
                    }

                    var answer = pixel + result;
                    var value = Convert.ToInt32(answer, 2);
                    var bite = Convert.ToByte(value);

                    pixelColor.B |= bite;

                    this.setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth, imageHeight);
                }
            }
        }

        /// <summary>Extracts the secret text.</summary>
        /// <param name="sourcePixels">The source pixels.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageHeight">Height of the image.</param>
        public void ExtractSecretText(byte[] sourcePixels, uint imageWidth, uint imageHeight)
        {
            var message = "";
            var bitChecked = 0;
            for (var i = 0; i < imageHeight; i++)
            for (var j = 2; j < imageWidth; j++)
            {
                var pixelColor = this.getPixelBgra8(sourcePixels, i, j, imageWidth, imageHeight);
                if (bitChecked % 8 == 0)
                {
                    bitChecked = 0;
                    var hex = pixelColor.B.ToString("X2");
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

                    bitChecked++;
                }
                else if (bitChecked % 8 != 0)
                {
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

                    bitChecked++;
                }
            }

            var item = this.convertBytesToText(this.textBytes);
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

        private List<char> convertBytesToText(List<byte> text)
        {
            var bytes = new List<char>();

            foreach (var item in text)
            {
                bytes.Add(Convert.ToChar(item));
            }

            return bytes;
        }

        private string convertBitsToChar(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var Sb = new StringBuilder();

            for (var i = 0; i < value.Length / 8; ++i)
            {
                Sb.Append(Convert.ToChar(value.Substring(8 * i, 8)));
            }

            return Sb.ToString();
        }

        #endregion
    }
}