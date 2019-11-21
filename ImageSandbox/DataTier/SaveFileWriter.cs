using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using GroupKStegafy.Model;

namespace GroupKStegafy.DataTier
{
    /// <summary>Instance of saving to a file</summary>
    public class SaveFileWriter
    {
        private WriteableBitmap modifiedImage;

        private Image saveImage;


        /// <summary>Initializes a new instance of the <see cref="SaveFileWriter"/> class.</summary>
        public SaveFileWriter()
        {
            this.saveImage = new Image();
        }

        /// <summary>
        ///     Saves the writable bitmap.
        /// </summary>
        /// <param name="image">The modified image.</param>
        public async void SaveWritableBitmap(WriteableBitmap image)
        {
            this.modifiedImage = image;
            this.saveImage.BitImage = image;
            var fileSavePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                SuggestedFileName = "image"
            };
            fileSavePicker.FileTypeChoices.Add("BMP", new List<string> { ".bmp" });
            fileSavePicker.FileTypeChoices.Add("PNG", new List<string> { ".png" });

            var savefile = await fileSavePicker.PickSaveFileAsync();

            if (savefile != null)
            {
                var stream = await savefile.OpenAsync(FileAccessMode.ReadWrite);
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

                var pixelStream = this.modifiedImage.PixelBuffer.AsStream();
                var pixels = new byte[pixelStream.Length];
                await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                    (uint)this.modifiedImage.PixelWidth,
                    (uint)this.modifiedImage.PixelHeight, this.saveImage.dpiX, this.saveImage.dpiY, pixels);
                await encoder.FlushAsync();

                stream.Dispose();
            }
        }
    }
}
