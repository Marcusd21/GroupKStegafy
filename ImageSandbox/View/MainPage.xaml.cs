using Windows.UI.Xaml;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GroupKStegafy.View
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainPage" /> class.
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        private void ImageStegafyButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StegafyImagePage));
        }

        private void TextStegafyButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StegafyTextPage));
        }

        #endregion
    }
}