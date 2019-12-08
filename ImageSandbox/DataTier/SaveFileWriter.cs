using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using GroupKStegafy.Model;

namespace GroupKStegafy.DataTier
{
    /// <summary>Instance of saving to a file</summary>
    public static class SaveFileWriter
    {
        #region Methods

        /// <summary>
        ///     Saves the writable bitmap.
        ///     Precondition: none
        ///     Postcondition: bitmap saved as an image file.
        /// </summary>
        /// <param name="image">The modified image.</param>
        /// <returns> true or false, if file was successfully saved.</returns>
        public static async Task<bool> SaveWritableBitmap(WriteableBitmap image)
        {
            var saveImage = new Image {BitImage = image};

            var fileSavePicker = new FileSavePicker {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                SuggestedFileName = "image"
            };
            fileSavePicker.FileTypeChoices.Add("BMP", new List<string> {".bmp"});
            fileSavePicker.FileTypeChoices.Add("PNG", new List<string> {".png"});

            var saveFile = await fileSavePicker.PickSaveFileAsync();

            if (saveFile == null)
            {
                return false;
            }

            var stream = await saveFile.OpenAsync(FileAccessMode.ReadWrite);
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

            var pixelStream = image.PixelBuffer.AsStream();
            var pixels = new byte[pixelStream.Length];
            await pixelStream.ReadAsync(pixels, 0, pixels.Length);

            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                (uint) image.PixelWidth,
                (uint) image.PixelHeight, saveImage.DpiX, saveImage.DpiY, pixels);
            await encoder.FlushAsync();

            stream.Dispose();

            return true;
        }

        #endregion
    }
}