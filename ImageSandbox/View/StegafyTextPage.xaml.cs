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
        private TextManager textManager;
        private BitmapImage textImage;

        /// <summary>Initializes a new instance of the <see cref="StegafyTextPage"/> class.</summary>
        public StegafyTextPage()
        {
            this.InitializeComponent();
            this.sourceImage = new Image();
            this.hiddenImage = new Image();
            this.modifiedImage = null;
            this.textImage = new BitmapImage();
            this.textManager = new TextManager();
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
            var result = await FileReader.SelectSourceImageFile();

            if (result != null)
            {
                var bitImage = await FileReader.MakeACopyOfTheFileToWorkOn(result);
                var imageSource = await FileReader.CreateImage(result, bitImage);

                this.sourceImage = imageSource;
                this.sourceImageDisplay.Source = this.sourceImage.BitImage;
            }
        }

        private async void OpenTextFileButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await FileReader.SelectSourceTextFile();

            if (result != null)
            {
                var textFromFile = await FileReader.CreateTextString(result);
                this.sourceText.Text = textFromFile;
            }          
        }

        private void EncryptTextButton_Click(object sender, RoutedEventArgs e)
        {
            this.keywordErrorLbl.Visibility = Visibility.Collapsed;

            if (this.keywordTxt.Text == string.Empty || this.sourceText.Text == string.Empty)
            {
                this.keywordErrorLbl.Visibility = Visibility.Visible;
                return;
            }

            this.encryptedText.Text = CipherTextManager.VigenereEncrypt(this.sourceText.Text, this.keywordTxt.Text);
        }

        private async void EmbedAndSave_Button_Click(object sender, RoutedEventArgs e)
        {
            this.embedErrorLbl.Visibility = Visibility.Collapsed;

            if (this.sourceImage.Pixels == null || this.encryptedText.Text == string.Empty)
            {
                this.embedErrorLbl.Visibility = Visibility.Visible;
                return;
            }

            this.textManager.EmbedText(this.sourceImage.Pixels, Convert.ToUInt32(this.sourceImage.ImageWidth), Convert.ToUInt32(this.sourceImage.ImageHeight), this.encryptedText.Text, Convert.ToInt32(this.cbBpcc.SelectedValue));

            this.modifiedImage = new WriteableBitmap(this.sourceImage.ImageWidth,this.sourceImage.ImageHeight);

            SaveFileWriter.SaveWritableBitmap(this.modifiedImage);

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
            var result = await FileReader.SelectSourceImageFile();

            if (result != null)
            {
                var bitImage = await FileReader.MakeACopyOfTheFileToWorkOn(result);
                var imageHidden = await FileReader.CreateImage(result, bitImage);

                this.hiddenImage = imageHidden;
                this.hiddenImageDisplay.Source = this.hiddenImage.BitImage;
            }
            if (ImageManager.IsImageSecretMessage(this.hiddenImage.Pixels, Convert.ToUInt32(this.hiddenImage.ImageWidth),
                Convert.ToUInt32(this.hiddenImage.ImageHeight)))
            {
               var item = this.textManager.ExtractSecretText(this.hiddenImage.Pixels, Convert.ToUInt32(this.hiddenImage.ImageWidth), Convert.ToUInt32(this.hiddenImage.ImageHeight), Convert.ToInt32(this.cbBpcc.SelectedValue));
               var curr = "";
               foreach (var c in item)
               {
                   curr += c;
               }

               var answer = CipherTextManager.VigenereDecrypt(curr);
               this.tbHiddenMessage.Text = answer;
            }

        }

       




    }

   
}
