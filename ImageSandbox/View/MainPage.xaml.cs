using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using GroupKStegafy.DataTier;
using GroupKStegafy.ViewModel;
using GroupKStegafy.Model;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GroupKStegafy.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        #region Data members

        private double dpiX;
        private double dpiY;
        private WriteableBitmap modifiedImage;
        private MainPageViewModel viewModel;
        private FileReader reader;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="MainPage"/> class.</summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.viewModel = new MainPageViewModel();
            this.reader = new FileReader();
            this.modifiedImage = null;
            this.dpiX = 0;
            this.dpiY = 0;
        }

        #endregion

        #region Methods

        private async void openButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await this.reader.SelectSourceImageFile();
            var bitImage = await this.reader.MakeACopyOfTheFileToWorkOn(result);
            var sourceImage = await this.reader.CreateImage(result, bitImage);

            this.sourceImageDisplay.Source = sourceImage.BitImage;
        }

        private void giveImageRedTint(byte[] sourcePixels, uint imageWidth, uint imageHeight)
        {
            for (var i = 0; i < imageHeight; i++)
            {
                for (var j = 0; j < imageWidth; j++)
                {
                    var pixelColor = this.GetPixelBgra8(sourcePixels, i, j, imageWidth, imageHeight);

                    pixelColor.R = 255;

                    this.SetPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth, imageHeight);
                }
            }
        }

        private async Task<StorageFile> selectSourceImageFile()
        {
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");

            var file = await openPicker.PickSingleFileAsync();

            return file;
        }

        private async Task<BitmapImage> MakeACopyOfTheFileToWorkOn(StorageFile imageFile)
        {
            IRandomAccessStream inputstream = await imageFile.OpenReadAsync();
            var newImage = new BitmapImage();
            newImage.SetSource(inputstream);
            return newImage;
        }

        public Color GetPixelBgra8(byte[] pixels, int x, int y, uint width, uint height)
        {
            var offset = (x * (int)width + y) * 4;
            var r = pixels[offset + 2];
            var g = pixels[offset + 1];
            var b = pixels[offset + 0];
            return Color.FromArgb(0, r, g, b);
        }

        public void SetPixelBgra8(byte[] pixels, int x, int y, Color color, uint width, uint height)
        {
            var offset = (x * (int)width + y) * 4;
            pixels[offset + 2] = color.R;
            pixels[offset + 1] = color.G;
            pixels[offset + 0] = color.B;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            this.saveWritableBitmap();
        }

        private async void saveWritableBitmap()
        {
            var fileSavePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                SuggestedFileName = "image"
            };
            fileSavePicker.FileTypeChoices.Add("PNG files", new List<string> { ".png" });
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
                    (uint)this.modifiedImage.PixelHeight, this.dpiX, this.dpiY, pixels);
                await encoder.FlushAsync();

                stream.Dispose();
            }
        }

        #endregion

        private async void LoadMonoImageButton_Click(object sender, RoutedEventArgs e)
        {
          var result = await  this.reader.SelectSourceImageFile();
          var bitImage = await this.reader.MakeACopyOfTheFileToWorkOn(result);
          var monoImage = await this.reader.CreateImage(result,bitImage );

          this.monoImageDisplay.Source = monoImage.BitImage;
        }

        private async void LoadHiddenImageButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await this.reader.SelectSourceImageFile();
            var bitImage = await this.reader.MakeACopyOfTheFileToWorkOn(result);
            var hiddenImage = await this.reader.CreateImage(result, bitImage);

            this.hiddenImageDisplay.Source = hiddenImage.BitImage;
        }
    }
}
