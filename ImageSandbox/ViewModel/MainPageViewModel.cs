using System.ComponentModel;
using System.Runtime.CompilerServices;
using GroupKStegafy.Annotations;
using GroupKStegafy.Model;

namespace GroupKStegafy.ViewModel
{
    /// <summary>Instance of the view model</summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class MainPageViewModel : INotifyPropertyChanged
    {
        #region Data members

        private Image sourceImage;

        private Image monochromeImage;

        private Image hiddenImage;

        private Image secretImage;

        #endregion

        #region Properties

        /// <summary>Gets or sets the scores.</summary>
        /// <value>The scores.</value>
        public Image SourceImage
        {
            get => this.sourceImage;
            set
            {
                this.sourceImage = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>Gets or sets the monochrome image.</summary>
        /// <value>The monochrome image.</value>
        public Image MonochromeImage
        {
            get => this.monochromeImage;
            set
            {
                this.monochromeImage = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>Gets or sets the hidden image.</summary>
        /// <value>The hidden image.</value>
        public Image HiddenImage
        {
            get => this.hiddenImage;
            set
            {
                this.hiddenImage = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>Gets or sets the message image.</summary>
        /// <value>The message image.</value>
        public Image SecretImage
        {
            get => this.secretImage;
            set
            {
                this.secretImage = value;
                this.OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="MainPageViewModel" /> class.</summary>
        public MainPageViewModel()
        {
            this.sourceImage = new Image();
            this.monochromeImage = new Image();
            this.hiddenImage = new Image();
            this.secretImage = new Image();
        }

        #endregion

        #region Methods

        /// <summary>Occurs when a property value changes.</summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Updates property
        /// </summary>
        /// <param name="propertyName"></param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}