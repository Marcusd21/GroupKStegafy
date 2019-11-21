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
using GroupKStegafy.Controller;
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
        private WriteableBitmap secretImage;
        private MainPageViewModel viewModel;
        private FileReader reader;
        private Image sourceImage;
        private Image monoImage;
        private Image hiddenImage;
        private ImageManager imageManager;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="MainPage"/> class.</summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.viewModel = new MainPageViewModel();
            this.reader = new FileReader();
            this.sourceImage = new Image();
            this.monoImage = new Image();
            this.hiddenImage = new Image();
            this.imageManager = new ImageManager();
            this.modifiedImage = null;
            this.secretImage = null;
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

            this.sourceImage = sourceImage;
            this.sourceImageDisplay.Source = sourceImage.BitImage;
            
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

          this.monoImage = monoImage;
          this.imageManager.MonoImage = monoImage;
          this.monoImageDisplay.Source = monoImage.BitImage;
        }

        private async void LoadHiddenImageButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await this.reader.SelectSourceImageFile();
            var bitImage = await this.reader.MakeACopyOfTheFileToWorkOn(result);
            var hiddenImage = await this.reader.CreateImage(result, bitImage);

            this.hiddenImage = hiddenImage;
            this.hiddenImageDisplay.Source = hiddenImage.BitImage;
        }

        private async void EmbedButton_Click(object sender, RoutedEventArgs e)
        {
            this.imageManager.embedImage(this.sourceImage.Pixels, Convert.ToUInt32(this.sourceImage.ImageWidth), Convert.ToUInt32(this.sourceImage.ImageHeight));

            this.modifiedImage = new WriteableBitmap(this.sourceImage.ImageWidth, this.sourceImage.ImageHeight);

            using (var writeStream = this.modifiedImage.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(this.sourceImage.Pixels, 0, this.sourceImage.Pixels.Length);
            }
        }

        private async void ExtractButton_Click(object sender, RoutedEventArgs e)
        {
            this.imageManager.extractSecretImage(this.hiddenImage.Pixels, Convert.ToUInt32(this.hiddenImage.ImageWidth), Convert.ToUInt32(this.hiddenImage.ImageHeight));

            this.secretImage = new WriteableBitmap(this.hiddenImage.ImageWidth, this.hiddenImage.ImageHeight);

            using (var writeStream = this.secretImage.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(this.hiddenImage.Pixels, 0, this.hiddenImage.Pixels.Length);
                this.secretImageDisplay.Source = this.secretImage;
            }
        }
    }
}
