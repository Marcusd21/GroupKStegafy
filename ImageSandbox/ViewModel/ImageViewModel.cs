using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media.Imaging;
using GroupKStegafy.Annotations;
using GroupKStegafy.DataTier;
using GroupKStegafy.Model;
using GroupKStegafy.Utility;

namespace GroupKStegafy.ViewModel
{
    /// <summary>Instance of the view model</summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class ImageViewModel : INotifyPropertyChanged
    {
        #region Data members

        private Image sourceImage;

        private Image monochromeImage;

        private Image embeddedImage;

        private WriteableBitmap secretImage;

        private string errorMonoSizeText;

        private string errorEncryptText;

        private string encryptedText;

        private string secretMessage;

        private string embedSuccessText;

        private int currentBpcc;

        private int selectedBpcc;

        #endregion

        #region Properties

        /// <summary>
        ///     Command for setting up the source Image.
        /// </summary>
        public RelayCommand SetSourceImageCommand { get; }

        /// <summary>
        ///     Command for setting up the mono Image.
        /// </summary>
        public RelayCommand SetMonoImageCommand { get; }

        /// <summary>
        ///     Command for setting up the embedded Image.
        /// </summary>
        public RelayCommand SetEmbeddedImageCommand { get; }

        /// <summary>
        ///     Command for embedding and saving image within image.
        /// </summary>
        public RelayCommand EmbedAndSaveImageCommand { get; }

        /// <summary>
        ///     Command for embedding and saving text within image.
        /// </summary>
        public RelayCommand EmbedAndSaveTextImageCommand { get; }

        /// <summary>
        ///     Command for encrypting text.
        /// </summary>
        public RelayCommand EncryptTextCommand { get; }

        /// <summary>
        ///     Gets or sets the original text.
        /// </summary>
        /// <value>
        ///     The original text.
        /// </value>
        public string OriginalText { get; set; }

        /// <summary>
        ///     Gets or sets the keyword text.
        /// </summary>
        /// <value>
        ///     The keyword text.
        /// </value>
        public string KeywordText { get; set; }

        /// <summary>
        ///     Gets or sets the selected BPCC.
        /// </summary>
        /// <value>
        ///     The selected BPCC.
        /// </value>
        public int SelectedBpcc
        {
            get => this.selectedBpcc;
            set
            {
                this.selectedBpcc = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the source image.
        /// </summary>
        /// <value>
        ///     The source image.
        /// </value>
        public Image SourceImage
        {
            get => this.sourceImage;
            set
            {
                this.sourceImage = value;
                this.OnPropertyChanged();
                this.EmbedAndSaveImageCommand.UpdateCanExecute();
                this.EmbedAndSaveTextImageCommand.UpdateCanExecute();
                this.EmbedSuccessText = string.Empty;
            }
        }

        /// <summary>
        ///     Gets or sets the monochrome image.
        /// </summary>
        /// <value>
        ///     The monochrome image.
        /// </value>
        public Image MonochromeImage
        {
            get => this.monochromeImage;
            set
            {
                this.monochromeImage = value;
                this.OnPropertyChanged();
                this.EmbedAndSaveImageCommand.UpdateCanExecute();
            }
        }

        /// <summary>
        ///     Gets or sets the embedded image.
        /// </summary>
        /// <value>
        ///     The embedded image.
        /// </value>
        public Image EmbeddedImage
        {
            get => this.embeddedImage;
            set
            {
                this.embeddedImage = value;
                this.OnPropertyChanged();
                this.embeddedImageExtraction();
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
        ///     Gets or sets the error text.
        /// </summary>
        /// <value>
        ///     The error text.
        /// </value>
        public string ErrorText
        {
            get => this.errorMonoSizeText;
            set
            {
                this.errorMonoSizeText = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the error encrypt text.
        /// </summary>
        /// <value>
        ///     The error encrypt text.
        /// </value>
        public string ErrorEncryptText
        {
            get => this.errorEncryptText;
            set
            {
                this.errorEncryptText = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the encrypted text.
        /// </summary>
        /// <value>
        ///     The encrypted text.
        /// </value>
        public string EncryptedText
        {
            get => this.encryptedText;
            set
            {
                this.encryptedText = value;
                this.OnPropertyChanged();
                this.EmbedAndSaveTextImageCommand.UpdateCanExecute();
            }
        }

        /// <summary>
        ///     Gets or sets the secret message.
        /// </summary>
        /// <value>
        ///     The secret message.
        /// </value>
        public string SecretMessage
        {
            get => this.secretMessage;
            set
            {
                this.secretMessage = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the embed success text.
        /// </summary>
        /// <value>
        ///     The embed success text.
        /// </value>
        public string EmbedSuccessText
        {
            get => this.embedSuccessText;
            set
            {
                this.embedSuccessText = value;
                this.OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImageViewModel" /> class.
        ///     Precondition: none
        ///     Postcondition: initializes the view model.
        /// </summary>
        public ImageViewModel()
        {
            this.sourceImage = new Image();
            this.monochromeImage = new Image();
            this.embeddedImage = new Image();
            this.selectedBpcc = 1;

            this.SetSourceImageCommand = new RelayCommand(this.setImage, canSetImage);
            this.SetMonoImageCommand = new RelayCommand(this.setMonoImage, canSetMonoImage);
            this.SetEmbeddedImageCommand = new RelayCommand(this.setEmbeddedImage, canSetEmbeddedImage);
            this.EmbedAndSaveImageCommand = new RelayCommand(this.embedAndSaveImage, this.canEmbedAndSaveImage);
            this.EmbedAndSaveTextImageCommand =
                new RelayCommand(this.embedAndSaveTextImage, this.canEmbedAndSaveTextImage);
            this.EncryptTextCommand = new RelayCommand(this.encrpytText, canEncrpytText);
        }

        #endregion

        #region Methods

        /// <summary>Occurs when a property value changes.</summary>
        /// <returns>event handler for property changed</returns>
        public event PropertyChangedEventHandler PropertyChanged;

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

        private static bool canSetEmbeddedImage(object obj)
        {
            return obj != null;
        }

        private void setEmbeddedImage(object obj)
        {
            var hiddenImage = obj as Image;
            this.EmbeddedImage = hiddenImage;
        }

        private bool canEmbedAndSaveImage(object obj)
        {
            return this.SourceImage.Pixels != null && this.MonochromeImage.Pixels != null;
        }

        private async void embedAndSaveImage(object obj)
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

                var result = await SaveFileWriter.SaveWritableBitmap(modifiedImage);

                this.EmbedSuccessText = result ? "Image embedding successful!" : "Error: Image embedding unsuccessful!";
            }
            else
            {
                this.ErrorText = "**Mono Image exceeds the Source Image size**";
            }
        }

        private bool canEmbedAndSaveTextImage(object obj)
        {
            return this.SourceImage.Pixels != null && !string.IsNullOrEmpty(this.EncryptedText);
        }

        private async void embedAndSaveTextImage(object obj)
        {
            TextManager.EmbedText(this.sourceImage.Pixels, Convert.ToUInt32(this.sourceImage.ImageWidth),
                Convert.ToUInt32(this.sourceImage.ImageHeight), this.encryptedText, Convert.ToInt32(this.SelectedBpcc));

            var modifiedImage = new WriteableBitmap(this.sourceImage.ImageWidth, this.sourceImage.ImageHeight);

            using (var writeStream = modifiedImage.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(this.sourceImage.Pixels, 0, this.sourceImage.Pixels.Length);
            }

            var result = await SaveFileWriter.SaveWritableBitmap(modifiedImage);

            this.EmbedSuccessText = result ? "Image embedding successful!" : "Error: Image embedding unsuccessful!";
            this.currentBpcc = this.SelectedBpcc;
        }

        private static bool canEncrpytText(object obj)
        {
            return true;
        }

        private void encrpytText(object obj)
        {
            this.ErrorEncryptText = string.Empty;

            if (string.IsNullOrEmpty(this.KeywordText) || this.OriginalText == null)
            {
                this.ErrorEncryptText = "**Please provide a keyword and/or text file!**";
                return;
            }

            this.EncryptedText = CipherTextManager.VigenereEncrypt(this.OriginalText, this.KeywordText);
        }

        private void embeddedImageExtraction()
        {
            if (ImageManager.IsImageSecretMessage(this.embeddedImage.Pixels,
                Convert.ToUInt32(this.embeddedImage.ImageWidth),
                Convert.ToUInt32(this.embeddedImage.ImageHeight)))
            {
                this.extractSecretMessage();
            }
            else
            {
                this.extractSecretImage();
            }
        }

        private void extractSecretMessage()
        {
            if (this.embeddedImage.Pixels == null)
            {
                return;
            }

            var item = TextManager.ExtractSecretText(this.embeddedImage.Pixels,
                Convert.ToUInt32(this.embeddedImage.ImageWidth), Convert.ToUInt32(this.embeddedImage.ImageHeight),
                Convert.ToInt32(this.currentBpcc));

            var curr = item.Aggregate("", (current, c) => current + c);

            var decryptedMessage = CipherTextManager.VigenereDecrypt(curr);
            this.SecretMessage = decryptedMessage;
        }

        private async void extractSecretImage()
        {
            if (this.embeddedImage.Pixels == null)
            {
                return;
            }

            ImageManager.ExtractSecretImage(this.embeddedImage.Pixels, Convert.ToUInt32(this.embeddedImage.ImageWidth),
                Convert.ToUInt32(this.embeddedImage.ImageHeight));

            var imageSecret = new WriteableBitmap(this.embeddedImage.ImageWidth, this.embeddedImage.ImageHeight);

            using (var writeStream = imageSecret.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(this.embeddedImage.Pixels, 0, this.embeddedImage.Pixels.Length);
                this.SecretImage = imageSecret;
            }
        }

        /// <summary>
        ///     Updates property.
        ///     Precondition: none
        ///     Postcondition: none
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