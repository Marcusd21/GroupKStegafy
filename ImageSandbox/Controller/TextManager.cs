using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;

namespace GroupKStegafy.Controller
{
    /// <summary>Instance to update text</summary>
    public static class TextManager
    {
        #region Data members

        //private static List<byte> TextBytes;

       // private static readonly int[] Item = {0X1, 0X2, 0X4, 0X8, 0X10, 0X20, 0X40, 0X80, 0X100};

        #endregion

        #region Methods

        /// <summary>Gets the pixel bgra8.</summary>
        /// <param name="pixels">The pixels.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
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

        /// <summary>Sets the pixel bgra8.</summary>
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

        /// <summary>Embeds the text.</summary>
        /// <param name="sourcePixels">The source pixels.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageHeight">Height of the image.</param>
        /// <param name="text">The text.</param>
        public static void EmbedText(byte[] sourcePixels, uint imageWidth, uint imageHeight, string text, int bpcc)
        {
            var textBytesValue = convertTextToBytes(text);
            var current = 0;
            var byteAmount = Convert.ToByte(bpcc);
            var bitChecked = 0;
            var bit = "";
            var channel = 0;
            var pixel = "";
            for (var i = 0; i < imageHeight; i++)
            {
                for (var j = 0; j < imageWidth; j++)
                {
                    var pixelColor = getPixelBgra8(sourcePixels, i, j, imageWidth, imageHeight);
                    var bitR = Convert.ToString(pixelColor.R, 2).PadLeft(8, '0');
                    var bitG = Convert.ToString(pixelColor.G, 2).PadLeft(8, '0');
                    var bitB = Convert.ToString(pixelColor.B, 2).PadLeft(8, '0');
                    var count = 0;
                    if (j == 0 && i == 0)
                    {
                        pixelColor.B = 212;
                        pixelColor.R = 212;
                        pixelColor.G = 212;
                        setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth);
                    }
                    else if (j == 1 && i == 0)
                    {
                        pixelColor.R = 1;
                        pixelColor.G = byteAmount;
                        pixelColor.B = 0;
                        setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth);
                    }
                    else
                    {
                        while (channel != 3 && current < textBytesValue.Length)
                        {
                            if (channel == 0)
                            {
                                bit = Convert.ToString(textBytesValue[current], 2).PadLeft(8, '0');

                                var result = "";
                                var amount = bitChecked;
                                for (var k = 0; k < bitR.Length - bpcc; k++)
                                {
                                    pixel += bitR[k];
                                }

                                for (var k = 0; k < bit.Length; k++)
                                {
                                    if (k > amount - 1 && k < amount + bpcc)
                                    {
                                        result += bit[k];
                                        count++;
                                        bitChecked++;

                                        if (bitChecked == 8)
                                        {
                                            current++;

                                            bitChecked = 0;
                                        }
                                    }
                                }
                                if (count != bpcc && current < textBytesValue.Length)
                                {
                                    bit = Convert.ToString(textBytesValue[current], 2).PadLeft(8, '0');
                                    
                                    var countLeft = bpcc - count;
                                    for (var k = 0; k < countLeft; k++)
                                    {
                                        result += bit[k];
                                        count++;
                                        bitChecked++;
                                    }
                                }

                                if (count == bpcc)
                                {
                                    var value = "";
                                    var curr = 0;
                                    for (var k = result.Length-1; k >=0; k--)
                                    {
                                        
                                        if (result[k] == '1' && bitR[bitR.Length - 1 - curr] == '0')
                                        {
                                            value = result[k]+value;
                                            
                                                curr++;
                                                var item = Convert.ToInt32(value, 2);
                                                var bite = Convert.ToByte(item);
                                                pixelColor.R |= bite;
                                                setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth);
                                            
                                        }
                                        else if (result[k] == '0' && bitR[bitR.Length - 1 - curr] == '1')
                                        {
                                            curr++;
                                            value = result[k] + value;

                                            var item = Convert.ToInt32(value, 2);
                                                var bite = Convert.ToByte(item);
                                                pixelColor.R &= bite;
                                                setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth);
                                            
                                        }
                                        else if (result[k] == '0' && bitR[bitR.Length - 1 - curr] == '0')
                                        {
                                            curr++;
                                            value = result[k] + value;

                                            var item = Convert.ToInt32(value, 2);
                                                var bite = Convert.ToByte(item);
                                                pixelColor.R &= bite;
                                                setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth);
                                            
                                        }
                                        else
                                        {
                                            curr++;
                                            value = result[k] + value;

                                            var item = Convert.ToInt32(value, 2);
                                                var bite = Convert.ToByte(item);
                                                pixelColor.R |= bite;
                                                setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth);
                                            
                                        }
                                    }

                                   

                                    pixel = "";
                                    count = 0;
                                }

                                channel++;
                            }
                            else if (channel == 1)
                            {
                                bit = Convert.ToString(textBytesValue[current], 2).PadLeft(8, '0');
                                var result = "";

                                var amount = bitChecked;
                                for (var k = 0; k < bitG.Length - bpcc; k++)
                                {
                                    pixel += bitG[k];
                                }

                                for (var k = 0; k < bit.Length; k++)
                                {
                                    if (k > amount - 1 && k < amount + bpcc)
                                    {
                                        result += bit[k];
                                        count++;
                                        bitChecked++;

                                        if (bitChecked == 8)
                                        {
                                            current++;

                                            bitChecked = 0;
                                        }
                                    }
                                }

                                if (count != bpcc && current < textBytesValue.Length)
                                {
                                    bit = Convert.ToString(textBytesValue[current], 2).PadLeft(8, '0');
                                    
                                    var countLeft = bpcc - count;
                                    for (var k = 0; k < countLeft; k++)
                                    {
                                        result += bit[k];
                                        count++;
                                        bitChecked++;
                                    }
                                }

                                if (count == bpcc)
                                {
                                    var curr = 0;
                                    var value = "";
                                    for (var k = result.Length-1; k >=0; k--)
                                    {
                                        
                                        if (result[k] == '1' && bitG[bitG.Length - 1 - curr] == '0')
                                        {
                                            value = result[k] + value;
                                            curr++;
                                           
                                                var item = Convert.ToInt32(value, 2);
                                                var bite = Convert.ToByte(item);
                                                pixelColor.G |= bite;
                                                setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth);

                                        }
                                        else if (result[k] == '0' && bitG[bitG.Length - 1 - curr] == '1')
                                        {
                                            curr++;
                                            value = result[k] + value;

                                            var item = Convert.ToInt32(value, 2);
                                                var bite = Convert.ToByte(item);
                                                pixelColor.G &= bite;
                                                setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth);

                                        }
                                        else if (result[k] == '0' && bitG[bitG.Length - 1 - curr] == '0')
                                        {
                                            curr++;
                                            value = result[k] + value;

                                            var item = Convert.ToInt32(value, 2);
                                                var bite = Convert.ToByte(item);
                                                pixelColor.G &= bite;
                                                setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth);

                                        }
                                        else
                                        {
                                            curr++;
                                            value = result[k] + value;

                                            var item = Convert.ToInt32(value, 2);
                                                var bite = Convert.ToByte(item);
                                                pixelColor.G |= bite;
                                                setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth);

                                        }
                                    }


                                    pixel = "";
                                    count = 0;
                                }

                                channel++;
                            }

                            else
                            {
                                bit = Convert.ToString(textBytesValue[current], 2).PadLeft(8, '0');

                                var amount = bitChecked;
                                var result = "";
                                for (var k = 0; k < bitB.Length - bpcc; k++)
                                {
                                    pixel += bitB[k];
                                }

                                for (var k = 0; k < bit.Length; k++)
                                {
                                    if (k > amount - 1 && k < amount + bpcc)
                                    {
                                        result += bit[k];
                                        count++;
                                        bitChecked++;

                                        if (bitChecked == 8)
                                        {
                                            current++;
                                            bitChecked = 0;
                                        }
                                    }
                                }
                                if (count != bpcc && current < textBytesValue.Length)
                                {
                                    bit = Convert.ToString(textBytesValue[current], 2).PadLeft(8, '0');
                                    
                                    var countLeft = bpcc - count;
                                    for (var k = 0; k < countLeft; k++)
                                    {
                                        result += bit[k];
                                        count++;
                                        bitChecked++;
                                    }
                                }
                                if (count == bpcc)
                                {
                                    var value = "";
                                    var curr = 0;
                                    for (var k = result.Length-1; k >=0; k--)
                                    {
                                       
                                        if (result[k] == '1' && bitB[bitB.Length - 1 - curr] == '0')
                                        {
                                            curr++;
                                            value = result[k] + value;

                                            var item = Convert.ToInt32(value, 2);
                                                var bite = Convert.ToByte(item);
                                                pixelColor.B |= bite;
                                                setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth);

                                        }
                                        else if (result[k] == '0' && bitB[bitB.Length - 1 - curr] == '1')
                                        {
                                            curr++;
                                            value = result[k] + value;

                                            var item = Convert.ToInt32(value, 2);
                                                var bite = Convert.ToByte(item);
                                                pixelColor.B &= bite;
                                                setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth);

                                        }
                                        else if (result[k] == '0' && bitB[bitB.Length - 1 - curr] == '0')
                                        {
                                            curr++;
                                            value = result[k] + value;

                                            var item = Convert.ToInt32(value, 2);
                                                var bite = Convert.ToByte(item);
                                                pixelColor.B &= bite;
                                                setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth);

                                        }
                                        else
                                        {
                                            curr++;
                                            value = result[k] + value;

                                            var item = Convert.ToInt32(value, 2);
                                                var bite = Convert.ToByte(item);
                                                pixelColor.B|= bite;
                                                setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth);

                                        }
                                    }

                                

                                    pixel = "";
                                    count = 0;
                                }

                                channel++;
                            }
                        }
                    }

                    channel = 0;
                }
            }
        }

        /// <summary>Extracts the secret text.</summary>
        /// <param name="sourcePixels">The source pixels.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageHeight">Height of the image.</param>
        public static List<char> ExtractSecretText(byte[] sourcePixels, uint imageWidth, uint imageHeight, int bpcc)
        {
            var message = "";
            var bitChecked = 0;
            var channel = 0;
            var answer = new List<byte>();

            for (var i = 0; i < imageHeight; i++)
            {
                for (var j = 2; j < imageWidth; j++)
                {
                    var pixelColor = getPixelBgra8(sourcePixels, i, j, imageWidth, imageHeight);
                    var bitR = Convert.ToString(pixelColor.R, 2).PadLeft(8, '0');
                    var bitG = Convert.ToString(pixelColor.G, 2).PadLeft(8, '0');
                    var bitB = Convert.ToString(pixelColor.B, 2).PadLeft(8, '0');
                    var count = 0;
                    while (channel != 3)
                    {
                        if (channel == 0)
                        {
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

            var current = convertBytesToText(answer);
            return current;
        }

        private static byte[] convertTextToBytes(string text)
        {
            var array = Encoding.ASCII.GetBytes(text);

            return array;
        }

        private static List<char> convertBytesToText(List<byte> text)
        {
            var bytes = new List<char>();

            foreach (var item in text)
            {
                bytes.Add(Convert.ToChar(item));
            }

            return bytes;
        }

        private static string convertBitsToChar(string value)
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