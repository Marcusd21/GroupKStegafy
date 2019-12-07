﻿using GroupKStegafy.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupKStegafy.DataTier
{
    /// <summary>Creates instance of a file reader</summary>
    public static class FileReader
    {

        /// <summary>
        ///     Selects the source image file.
        /// </summary>
        /// <returns></returns>
        public static async Task<StorageFile> SelectSourceImageFile()
        {
            var openPicker = new FileOpenPicker {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                FileTypeFilter = {".png", ".bmp"}
            };

            var file = await openPicker.PickSingleFileAsync();

            return file;
        }

        /// <summary>Selects the source text file.</summary>
        /// <returns></returns>
        public static async Task<StorageFile> SelectSourceTextFile()
        {
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                FileTypeFilter = { ".txt"}
            };

            var file = await openPicker.PickSingleFileAsync();

            return file;
        }

        /// <summary>
        ///     Makes a copy of the file to work on.
        /// </summary>
        /// <param name="imageFile">The image file.</param>
        /// <returns></returns>
        public static async Task<BitmapImage> MakeACopyOfTheFileToWorkOn(StorageFile imageFile)
        {
            IRandomAccessStream inputStream = await imageFile.OpenReadAsync();
            var newImage = new BitmapImage();
            newImage.SetSource(inputStream);
            return newImage;
        }

        /// <summary>Creates the image.</summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static async Task<Image> CreateImage(StorageFile file, BitmapImage bitImage)
        {
            var image = new Image();

            using (var fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                await image.SetSourceImage(file, bitImage);
            }

            return image;     
        }

        /// <summary>Creates the text string.</summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static async Task<string> CreateTextString(StorageFile file)
        {
            var sb = new StringBuilder();
            
            using (var fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                var lines = await FileIO.ReadLinesAsync(file);

                foreach (var line in lines)
                {
                    var result = Regex.Match(line, "[a-zA-Z0-9]*");

                    while (result.Success)
                    {
                        sb.Append(result.Value);
                        result = result.NextMatch();
                    }
                }
            }

            return Convert.ToString(sb);           
        }
    }
}