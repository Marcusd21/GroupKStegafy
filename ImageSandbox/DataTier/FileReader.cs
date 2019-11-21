using GroupKStegafy.Model;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupKStegafy.DataTier
{
    /// <summary>Creates instance of a file reader</summary>
    public class FileReader
    {
        #region Methods

        /// <summary>
        ///     Selects the source image file.
        /// </summary>
        /// <returns></returns>
        public async Task<StorageFile> SelectSourceImageFile()
        {
            var openPicker = new FileOpenPicker {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                FileTypeFilter = {".png", ".bmp", ".jpg"}
            };

            var file = await openPicker.PickSingleFileAsync();

            return file;
        }

        /// <summary>
        ///     Makes a copy of the file to work on.
        /// </summary>
        /// <param name="imageFile">The image file.</param>
        /// <returns></returns>
        public async Task<BitmapImage> MakeACopyOfTheFileToWorkOn(StorageFile imageFile)
        {
            IRandomAccessStream inputStream = await imageFile.OpenReadAsync();
            var newImage = new BitmapImage();
            newImage.SetSource(inputStream);
            return newImage;
        }

        /// <summary>Creates the image.</summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public async Task<Image> CreateImage(StorageFile file, BitmapImage bitImage)
        {
            var image = new Image();

            using (var fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                await image.SetSourceImage(file, bitImage);
            }

            return image;

            #endregion
        }
    }
}