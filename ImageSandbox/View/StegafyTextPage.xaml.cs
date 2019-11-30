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
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GroupKStegafy.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StegafyTextPage : Page
    {
        private Image sourceImage;
        private BitmapImage textImage;
        private string textFromFile;
        private readonly FileReader reader;

        public StegafyTextPage()
        {
            this.InitializeComponent();
            this.reader = new FileReader();
            this.sourceImage = new Image();
            this.textImage = new BitmapImage();

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
            var bitImage = await this.reader.MakeACopyOfTheFileToWorkOn(result);
            var sourceImage = await this.reader.CreateImage(result, bitImage);

            this.sourceImage = sourceImage;
            this.sourceImageDisplay.Source = this.sourceImage.BitImage;
        }

        private async void OpenSecretButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await this.reader.SelectSourceTextFile();
            this.textFromFile = this.reader.CreateTextString(result);

            var bmp = this.DrawText();
            this.textImage = await this.ConverBitmapToBitmapImageAsync(bmp);

            this.sourceSecretTextDisplay.Source = this.textImage;
        }

        private Bitmap DrawText()
        {
            //first, create a dummy bitmap just to get a graphics object
            var img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            var font = new Font(System.Drawing.FontFamily.GenericSansSerif, 12);

            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(this.textFromFile, font);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            img = new Bitmap((int)textSize.Width, (int)textSize.Height);

            drawing = Graphics.FromImage(img);

            //paint the background
            drawing.Clear(Color.AntiqueWhite);

            //create a brush for the text
            var textBrush = new SolidBrush(Color.Black);

            drawing.DrawString(this.textFromFile, font, textBrush, 0, 0);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            return img;
        }

        private async Task<BitmapImage> ConverBitmapToBitmapImageAsync(Bitmap bmp)
        {
            MemoryStream stream = new MemoryStream();
            bmp.Save(stream, ImageFormat.Png);

            BitmapImage bitmapImage = new BitmapImage();
            await bitmapImage.SetSourceAsync(stream.AsRandomAccessStream());

            return bitmapImage;
        }

        private void EmbedAndSave_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            var navigationFrame = Window.Current.Content as Frame;
            navigationFrame?.Navigate(typeof(MainPage));
        }
    }

   
}
