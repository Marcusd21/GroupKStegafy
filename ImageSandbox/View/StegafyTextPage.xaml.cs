﻿using System;
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
using Image = GroupKStegafy.Model.Image;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GroupKStegafy.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StegafyTextPage : Page
    {
        private Image sourceImage;
        private readonly FileReader reader;

        public StegafyTextPage()
        {
            this.InitializeComponent();
            this.reader = new FileReader();
            this.sourceImage = new Image();

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
            this.sourceImageDisplay.Source = sourceImage.BitImage;
        }

        private void OpenSecretButton_Click(object sender, RoutedEventArgs e)
        {

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
