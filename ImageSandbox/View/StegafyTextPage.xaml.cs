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
using GroupKStegafy.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GroupKStegafy.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StegafyTextPage : Page
    {
        /// <summary>Initializes a new instance of the <see cref="StegafyTextPage"/> class.</summary>
        public StegafyTextPage()
        {
            this.InitializeComponent();
            this.fillComboBox();
        }

        private void fillComboBox()
        {
            for (var i = 1; i <= 8; i++)
            {
                var c = this.cbBpcc.Items;
                c?.Add(i);
            }
        }

        private async void LoadSourceImageButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await FileReader.SelectSourceImageFile();

            if (result == null) return;

            var bitImage = await FileReader.MakeACopyOfTheFileToWorkOn(result);
            var imageSource = await FileReader.CreateImage(result, bitImage);

            var imagePageViewModel = (ImageViewModel) this.DataContext;

            if (imagePageViewModel.SetSourceImageCommand.CanExecute(imageSource))
            {
                imagePageViewModel.SetSourceImageCommand.Execute(imageSource);
            }
        }

        private async void LoadEmbeddedImageButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await FileReader.SelectSourceImageFile();

            if (result != null)
            {
                var bitImage = await FileReader.MakeACopyOfTheFileToWorkOn(result);
                var imageHidden = await FileReader.CreateImage(result, bitImage);

                var imagePageViewModel = (ImageViewModel)this.DataContext;

                if (imagePageViewModel.SetEmbeddedImageCommand.CanExecute(imageHidden))
                {
                    imagePageViewModel.SetEmbeddedImageCommand.Execute(imageHidden);
                }
            }
        }

        private async void LoadTextFileButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await FileReader.SelectSourceTextFile();

            if (result == null) return;

            var textFromFile = await FileReader.CreateTextString(result);
            this.sourceText.Text = textFromFile;
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            var navigationFrame = Window.Current.Content as Frame;
            navigationFrame?.Navigate(typeof(MainPage));
        }
    }

   
}
