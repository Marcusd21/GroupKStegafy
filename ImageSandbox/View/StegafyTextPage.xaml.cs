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
using Windows.UI.Xaml.Navigation;
using GroupKStegafy.DataTier;
using GroupKStegafy.Model;
using System.Drawing;
using Image = GroupKStegafy.Model.Image;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GroupKStegafy.Controller;
using GroupKStegafy.Utility;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GroupKStegafy.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StegafyTextPage : Page
    {
        private Image sourceImage;
        private Image hiddenImage;
        private WriteableBitmap modifiedImage;
        private BitmapImage textImage;
        private string textFromFile;
        private readonly SaveFileWriter writer;
        private readonly FileReader reader;
        private readonly TextManager textManager;
        private readonly ImageManager imageManager;

        /// <summary>Initializes a new instance of the <see cref="StegafyTextPage"/> class.</summary>
        public StegafyTextPage()
        {
            this.InitializeComponent();
            this.writer = new SaveFileWriter();
            this.reader = new FileReader();
            this.sourceImage = new Image();
            this.hiddenImage = new Image();
            this.modifiedImage = null;
            this.textImage = new BitmapImage();
            this.textManager = new TextManager();
            this.imageManager = new ImageManager();
            this.fillComboBox();
        }

        private void fillComboBox()
        {

            for (int i = 1; i <= 8; i++)
            {
                var c = this.cbBpcc.Items;
                if (c != null)
                {
                    c.Add(i);
                }
            }

            this.cbBpcc.SelectedItem = 1;
        }

        private async void OpenSourceImageButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await this.reader.SelectSourceImageFile();

            if (result != null)
            {
                var bitImage = await this.reader.MakeACopyOfTheFileToWorkOn(result);
                var sourceImage = await this.reader.CreateImage(result, bitImage);

                this.sourceImage = sourceImage;
                this.sourceImageDisplay.Source = this.sourceImage.BitImage;
            }
        }

        private async void OpenTextFileButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await this.reader.SelectSourceTextFile();

            if (result != null)
            {
                this.textFromFile = await this.reader.CreateTextString(result);
                this.sourceText.Text = this.textFromFile;
            }          
        }

        private async void EmbedAndSave_Button_Click(object sender, RoutedEventArgs e)
        {
            this.keywordErrorLbl.Visibility = Visibility.Collapsed;

            if (this.keywordTxt.Text == string.Empty)
            {
                this.keywordErrorLbl.Visibility = Visibility.Visible;
                return;
            }

            var encryptText = CipherTextManager.VigenereEncrypt(this.textFromFile, this.keywordTxt.Text);
            this.encryptedText.Text = encryptText;

            this.textManager.EmbedText(this.sourceImage.Pixels, Convert.ToUInt32(this.sourceImage.ImageWidth), Convert.ToUInt32(this.sourceImage.ImageHeight), encryptText, 1);

            this.modifiedImage = new WriteableBitmap(this.sourceImage.ImageWidth,this.sourceImage.ImageHeight);

            this.writer.SaveWritableBitmap(this.modifiedImage);
            using (var writeStream = this.modifiedImage.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(this.sourceImage.Pixels, 0, this.sourceImage.Pixels.Length);
            }
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            var navigationFrame = Window.Current.Content as Frame;
            navigationFrame?.Navigate(typeof(MainPage));
        }

        private async void HiddenImageButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await this.reader.SelectSourceImageFile();

            if (result != null)
            {
                var bitImage = await this.reader.MakeACopyOfTheFileToWorkOn(result);
                var hiddenImage = await this.reader.CreateImage(result, bitImage);

                this.hiddenImage = hiddenImage;
                this.hiddenImageDisplay.Source = this.hiddenImage.BitImage;
            }
            if (this.imageManager.IsImageSecretMessage(this.hiddenImage.Pixels, Convert.ToUInt32(this.hiddenImage.ImageWidth),
                Convert.ToUInt32(this.hiddenImage.ImageHeight)))
            {
                this.textManager.ExtractSecretText(this.hiddenImage.Pixels, Convert.ToUInt32(this.hiddenImage.ImageWidth), Convert.ToUInt32(this.hiddenImage.ImageHeight));
            }

        }

       




    }

   
}
