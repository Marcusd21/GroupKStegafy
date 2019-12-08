using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GroupKStegafy.DataTier;
using GroupKStegafy.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GroupKStegafy.View
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StegafyImagePage : Page
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="StegafyImagePage" /> class.
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        public StegafyImagePage()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        private async void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await FileReader.SelectSourceImageFile();

            if (result == null)
            {
                return;
            }

            var bitImageCopy = await FileReader.MakeACopyOfTheFileToWorkOn(result);
            var imageSource = await FileReader.CreateImage(result, bitImageCopy);

            var imagePageViewModel = (ImageViewModel) DataContext;

            if (imagePageViewModel.SetSourceImageCommand.CanExecute(imageSource))
            {
                imagePageViewModel.SetSourceImageCommand.Execute(imageSource);
            }
        }

        private async void LoadMonoImageButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await FileReader.SelectSourceImageFile();

            if (result == null)
            {
                return;
            }

            var bitImageCopy = await FileReader.MakeACopyOfTheFileToWorkOn(result);
            var imageMono = await FileReader.CreateImage(result, bitImageCopy);

            var imagePageViewModel = (ImageViewModel) DataContext;

            if (imagePageViewModel.SetMonoImageCommand.CanExecute(imageMono))
            {
                imagePageViewModel.SetMonoImageCommand.Execute(imageMono);
            }
        }

        private async void LoadEmbeddedImageButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await FileReader.SelectSourceImageFile();

            if (result == null)
            {
                return;
            }

            var bitImageCopy = await FileReader.MakeACopyOfTheFileToWorkOn(result);
            var imageHidden = await FileReader.CreateImage(result, bitImageCopy);

            var imagePageViewModel = (ImageViewModel) DataContext;

            if (imagePageViewModel.SetEmbeddedImageCommand.CanExecute(imageHidden))
            {
                imagePageViewModel.SetEmbeddedImageCommand.Execute(imageHidden);
            }
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            var navigationFrame = Window.Current.Content as Frame;
            navigationFrame?.Navigate(typeof(MainPage));
        }

        #endregion
    }
}