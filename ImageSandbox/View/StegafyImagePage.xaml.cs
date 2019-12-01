using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using GroupKStegafy.Controller;
using GroupKStegafy.DataTier;
using GroupKStegafy.ViewModel;
using Image = GroupKStegafy.Model.Image;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GroupKStegafy.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StegafyImagePage : Page
    {
        private double dpiX;
        private double dpiY;
        private WriteableBitmap modifiedImage;
        private WriteableBitmap secretImage;
        private MainPageViewModel viewModel;
        private readonly FileReader reader;
        private Image sourceImage;
        private Image monoImage;
        private Image hiddenImage;
        private readonly ImageManager imageManager;
        private readonly SaveFileWriter writer;

        public StegafyImagePage()
        {
            this.InitializeComponent();

            this.viewModel = new MainPageViewModel();
            this.reader = new FileReader();
            this.sourceImage = new Image();
            this.monoImage = new Image();
            this.hiddenImage = new Image();
            this.imageManager = new ImageManager();
            this.writer = new SaveFileWriter();
            this.modifiedImage = null;
            this.secretImage = null;
            this.dpiX = 0;
            this.dpiY = 0;
        }

        private async void openButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await this.reader.SelectSourceImageFile();

            if (result != null)
            {
                var bitImage = await this.reader.MakeACopyOfTheFileToWorkOn(result);
                var sourceImage = await this.reader.CreateImage(result, bitImage);

                this.sourceImage = sourceImage;
                this.sourceImageDisplay.Source = sourceImage.BitImage;
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            this.writer.SaveWritableBitmap(this.modifiedImage);
        }

        private async void LoadMonoImageButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await this.reader.SelectSourceImageFile();

            if (result != null)
            {
                var bitImage = await this.reader.MakeACopyOfTheFileToWorkOn(result);
                var monoImage = await this.reader.CreateImage(result, bitImage);

                this.monoImage = monoImage;
                this.imageManager.MonoImage = monoImage;
                this.monoImageDisplay.Source = monoImage.BitImage;
            }       
        }

        private async void LoadHiddenImageButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await this.reader.SelectSourceImageFile();

            if (result != null)
            {
                var bitImage = await this.reader.MakeACopyOfTheFileToWorkOn(result);
                var hiddenImage = await this.reader.CreateImage(result, bitImage);

                this.hiddenImage = hiddenImage;
                this.hiddenImageDisplay.Source = hiddenImage.BitImage;
            }        
        }

        private async void EmbedButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.imageManager.IsImageExceedSource(Convert.ToUInt32(this.sourceImage.ImageWidth),
                Convert.ToUInt32(this.sourceImage.ImageHeight)))
            {
                this.tbImageError.Text = string.Empty;

                this.imageManager.EmbedImage(this.sourceImage.Pixels, Convert.ToUInt32(this.sourceImage.ImageWidth),
                    Convert.ToUInt32(this.sourceImage.ImageHeight));

                this.modifiedImage = new WriteableBitmap(this.sourceImage.ImageWidth, this.sourceImage.ImageHeight);

                using (var writeStream = this.modifiedImage.PixelBuffer.AsStream())
                {
                    await writeStream.WriteAsync(this.sourceImage.Pixels, 0, this.sourceImage.Pixels.Length);
                }
            }
            else
            {
                this.tbImageError.Text = "Secret Image exceeds the Source Image size";
            }
        }

        private async void ExtractButton_Click(object sender, RoutedEventArgs e)
        {
            this.imageManager.ExtractSecretImage(this.hiddenImage.Pixels, Convert.ToUInt32(this.hiddenImage.ImageWidth),
                Convert.ToUInt32(this.hiddenImage.ImageHeight));

            this.secretImage = new WriteableBitmap(this.hiddenImage.ImageWidth, this.hiddenImage.ImageHeight);

            using (var writeStream = this.secretImage.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(this.hiddenImage.Pixels, 0, this.hiddenImage.Pixels.Length);
                this.secretImageDisplay.Source = this.secretImage;
            }
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            var navigationFrame = Window.Current.Content as Frame;
            navigationFrame?.Navigate(typeof(MainPage));
        }
    }
}
