using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using GroupKStegafy.Annotations;
using GroupKStegafy.Controller;
using GroupKStegafy.DataTier;
using GroupKStegafy.Model;
using GroupKStegafy.Utility;

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

        private WriteableBitmap secretImage;

        private string errorText;

        public RelayCommand SetSourceImageCommand { get; }

        public RelayCommand SetMonoImageCommand { get; }

        public RelayCommand SetHiddenImageCommand { get; }

        public RelayCommand EmbedAndSaveCommand { get; }

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
                this.EmbedAndSaveCommand.UpdateCanExecute();
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
                this.extractSecretImage();
            }
        }

        /// <summary>Gets or sets the message image.</summary>
        /// <value>The message image.</value>
        public WriteableBitmap SecretImage
        {
            get => this.secretImage;
            set
            {
                this.secretImage = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the error message when trying to do an action.
        /// </summary>
        public string ErrorText
        {
            get => this.errorText;
            set
            {
                this.errorText = value;
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
            this.secretImage = null;

            this.SetSourceImageCommand = new RelayCommand(this.setImage, canSetImage);
            this.SetMonoImageCommand = new RelayCommand(this.setMonoImage, canSetMonoImage);
            this.SetHiddenImageCommand = new RelayCommand(this.setHiddenImage, canSetHiddenImage);
            this.EmbedAndSaveCommand = new RelayCommand(this.embedAndSave, canEmbedAndSave);
        }

        #endregion

        #region Methods

        private static bool canSetImage(object obj)
        {
            return obj != null;
        }

        private void setImage(object obj)
        {
            var image = obj as Image;
            this.SourceImage = image;
        }

        private static bool canSetMonoImage(object obj)
        {
            return obj != null;
        }

        private void setMonoImage(object obj)
        {
            var monoImage = obj as Image;
            this.MonochromeImage = monoImage;
        }

        private static bool canSetHiddenImage(object obj)
        {
            return obj != null;
        }

        private void setHiddenImage(object obj)
        {
            var hiddenImage = obj as Image;
            this.HiddenImage = hiddenImage;
        }

        private bool canEmbedAndSave(object obj)
        {
            return this.SourceImage.Pixels != null && this.MonochromeImage.Pixels != null;
        }

        private async void embedAndSave(object obj)
        {
            if (!ImageManager.IsImageExceedSource(Convert.ToUInt32(this.sourceImage.ImageWidth),
                Convert.ToUInt32(this.sourceImage.ImageHeight), this.monochromeImage))
            {
                this.ErrorText = string.Empty;

                ImageManager.EmbedImage(this.sourceImage.Pixels, Convert.ToUInt32(this.sourceImage.ImageWidth),
                    Convert.ToUInt32(this.sourceImage.ImageHeight), this.monochromeImage);

                var modifiedImage = new WriteableBitmap(this.sourceImage.ImageWidth, this.sourceImage.ImageHeight);

                using (var writeStream = modifiedImage.PixelBuffer.AsStream())
                {
                    await writeStream.WriteAsync(this.sourceImage.Pixels, 0, this.sourceImage.Pixels.Length);
                }

                SaveFileWriter.SaveWritableBitmap(modifiedImage);
            }
            else
            {
                this.ErrorText = "Secret Image exceeds the Source Image size";
            }
        }

        private async void extractSecretImage()
        {
            if (this.hiddenImage.Pixels == null) return;

            ImageManager.ExtractSecretImage(this.hiddenImage.Pixels, Convert.ToUInt32(this.hiddenImage.ImageWidth),
                Convert.ToUInt32(this.hiddenImage.ImageHeight));

            var imageSecret = new WriteableBitmap(this.hiddenImage.ImageWidth, this.hiddenImage.ImageHeight);

            using (var writeStream = imageSecret.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(this.hiddenImage.Pixels, 0, this.hiddenImage.Pixels.Length);
                this.SecretImage = imageSecret;
            }
        }

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