using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using GroupKStegafy.Controller;
using GroupKStegafy.DataTier;
using GroupKStegafy.Model;
using GroupKStegafy.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GroupKStegafy.View
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="MainPage" /> class.</summary>
        public MainPage()
        {
            this.InitializeComponent();
        }

        #endregion

        private void ImageStegafyButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StegafyImagePage));
        }

        private void TextStegafyButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StegafyTextPage));
        }
    }
}