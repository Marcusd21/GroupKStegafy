using System;
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
            var textBytesValue = this.convertTextToBytes(text);
            var current = 0;
            var byteAmount = Convert.ToByte(bpcc);
            var bitChecked = 0;
            var bit = "";

            var amount = "";
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

                else if (current < textBytesValue.Length && bitChecked % 8 == 0)
                {
                    var result = "";
                    var pixel = "";
                    bitChecked = 0;
                    bit = Convert.ToString(textBytesValue[current], 2).PadLeft(8, '0');
                    var pixelBit = Convert.ToString(pixelColor.B, 2).PadLeft(8, '0');

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
                        if (k > pixelBit.Length - bitChecked - bpcc && k < pixelBit.Length - bitChecked + 1)
                        {
                            pixel += bit[k];
                            amount = bit[k] + amount;
                        }
                        else
                        {
                            pixel += pixelBit[k];
                        }
                    }

                    if (j >= 232)
                    {
                        var a = 0;
                    }

                    var value = Convert.ToInt32(pixel, 2);
                    var bite = Convert.ToByte(value);
                    if (bite != 246)
                    {
                        pixelColor.B |= bite;
                    }
                    else
                    {
                        pixelColor.B &= bite;
                    }

                    this.setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth, imageHeight);
                    current++;
                }
                else if (current <= textBytesValue.Length && bitChecked % 8 != 0)
                {
                    var result = "";
                    var pixel = "";
                    var pixelBit = Convert.ToString(pixelColor.B, 2).PadLeft(8, '0');
                    for (var k = bit.Length - 1; k > bit.Length - bpcc - 1; k--)
                    {
                        if (bitChecked < 8 && bitChecked < bitChecked + bpcc)
                        {
                            result += bit[k];
                        }
                    }

                    if (j >= 232)
                    {
                        var a = 0;
                    }

                    bitChecked++;

                    for (var k = 0; k < pixelBit.Length; k++)
                    {
                        if (k > pixelBit.Length - bitChecked - bpcc && k < pixelBit.Length - bitChecked + 1)
                        {
                            pixel += bit[k];
                            amount = bit[k] + amount;
                        }
                        else
                        {
                            pixel += pixelBit[k];
                        }
                    }

                    if (amount.Equals(bit))
                    {
                        amount = "";
                    }

                    var value = Convert.ToInt32(pixel, 2);
                    var bite = Convert.ToByte(value);
                    if (bite >= 250 && bite <= 255)
                    {
                        pixelColor.B |= bite;
                    }
                    else
                    {
                        pixelColor.B &= bite;
                    }

                    this.setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth, imageHeight);
                }
            }
        }

        /// <summary>Extracts the secret text.</summary>
        /// <param name="sourcePixels">The source pixels.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageHeight">Height of the image.</param>
        public List<char> ExtractSecretText(byte[] sourcePixels, uint imageWidth, uint imageHeight, int bpcc)
        {
            var message = "";
            var bitChecked = 0;
            var channel = 0;
            var answer = new List<byte>();

            for (var i = 0; i < imageHeight; i++)
            {
                for (var j = 2; j < imageWidth; j++)
                {
                    var pixelColor = this.getPixelBgra8(sourcePixels, i, j, imageWidth, imageHeight);
                    var bitR = Convert.ToString(pixelColor.R, 2).PadLeft(8, '0');
                    var bitG = Convert.ToString(pixelColor.G, 2).PadLeft(8, '0');
                    var bitB = Convert.ToString(pixelColor.B, 2).PadLeft(8, '0');
                    var count = 0;
                    while (channel != 3)
                    {
                        if (channel == 0)
                        {
                            var amount = bpcc + bitChecked;
                            var a = bitChecked;
                            for (var k = 0; k < bitR.Length; k++)
                            {
                                if (k > bitR.Length - bpcc - 1 && k < bitR.Length)
                                {
                                    message += bitR[k];
                                    bitChecked++;

                                    if (bitChecked == 8)
                                    {
                                        count++;
                                        var value = Convert.ToInt32(message, 2);
                                        var bite = Convert.ToByte(value);
                                        answer.Add(bite);
                                        var c = Convert.ToChar(bite);
                                        message = "";
                                        bitChecked = 0;
                                        if (count == bpcc)
                                        {
                                            count = 0;
                                            break;
                                        }
                                    }
                                }
                            }

                            channel++;
                        }
                        else if (channel == 1)
                        {
                            var amount = bpcc + bitChecked;
                            var a = bitChecked;
                            for (var k = 0; k < bitG.Length; k++)
                            {
                                if (k > bitG.Length - bpcc - 1 && k < bitG.Length)
                                {
                                    message += bitG[k];
                                    bitChecked++;

                                    if (bitChecked == 8)
                                    {
                                        count++;
                                        var value = Convert.ToInt32(message, 2);
                                        var bite = Convert.ToByte(value);
                                        answer.Add(bite);
                                        var c = Convert.ToChar(bite);
                                        message = "";
                                        bitChecked = 0;
                                        if (count == bpcc)
                                        {
                                            count = 0;
                                            break;
                                        }
                                    }
                                }
                            }

                            channel++;
                        }
                        else
                        {
                            var amount = bpcc + bitChecked;
                            var a = bitChecked;
                            for (var k = 0; k < bitB.Length; k++)
                            {
                                if (k > bitG.Length - bpcc - 1 && k < bitG.Length)
                                {
                                    message += bitB[k];
                                    bitChecked++;

                                    if (bitChecked == 8)
                                    {
                                        count++;
                                        var value = Convert.ToInt32(message, 2);
                                        var bite = Convert.ToByte(value);
                                        answer.Add(bite);
                                        var c = Convert.ToChar(bite);
                                        message = "";
                                        bitChecked = 0;
                                        if (count == bpcc)
                                        {
                                            count = 0;
                                            break;
                                        }
                                    }
                                }
                            }

                            channel++;
                        }
                    }

                    channel = 0;
                }
            }

            var current = this.convertBytesToText(answer);
            return current;
        }

        private byte[] convertTextToBytes(string text)
        {
            var array = Encoding.ASCII.GetBytes(text);

            return array;
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