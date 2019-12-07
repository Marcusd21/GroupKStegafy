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
        public StegafyImagePage()
        {
            this.InitializeComponent();
        }

        private async void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await FileReader.SelectSourceImageFile();

            if (result == null) return;

            var bitImageCopy = await FileReader.MakeACopyOfTheFileToWorkOn(result);
            var imageSource = await FileReader.CreateImage(result, bitImageCopy);

            var imagePageViewModel = (MainPageViewModel) this.DataContext;

            if (imagePageViewModel.SetSourceImageCommand.CanExecute(imageSource))
            {
                imagePageViewModel.SetSourceImageCommand.Execute(imageSource);
            }
        }

        private async void LoadMonoImageButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await FileReader.SelectSourceImageFile();

            if (result == null) return;

            var bitImageCopy = await FileReader.MakeACopyOfTheFileToWorkOn(result);
            var imageMono = await FileReader.CreateImage(result, bitImageCopy);

            var imagePageViewModel = (MainPageViewModel) this.DataContext;

            if (imagePageViewModel.SetMonoImageCommand.CanExecute(imageMono))
            {
                imagePageViewModel.SetMonoImageCommand.Execute(imageMono);
            }
        }

        private async void LoadHiddenImageButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await FileReader.SelectSourceImageFile();

            if (result == null) return;

            var bitImageCopy = await FileReader.MakeACopyOfTheFileToWorkOn(result);
            var imageHidden = await FileReader.CreateImage(result, bitImageCopy);

            var imagePageViewModel = (MainPageViewModel) this.DataContext;

            if (imagePageViewModel.SetHiddenImageCommand.CanExecute(imageHidden))
            {
                imagePageViewModel.SetHiddenImageCommand.Execute(imageHidden);
            }
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            var navigationFrame = Window.Current.Content as Frame;
            navigationFrame?.Navigate(typeof(MainPage));
        }
    }
}
