using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupKStegafy.Model
{
    /// <summary>
    /// Creates an instance of an Image
    /// </summary>
    public class Image
    {
        #region Data members

        /// <summary>
        /// Represents red bytes
        /// </summary>
        public List<double> RedBytes = new List<double>();
        /// <summary>
        /// Represents blue bytes
        /// </summary>
        public List<double> BlueBytes = new List<double>();
        /// <summary>
        /// Represents green bytes
        /// </summary>
        public List<double> GreenBytes = new List<double>();

        /// <summary>
        /// 
        /// </summary>
        public double dpiX;
        /// <summary>
        /// 
        /// </summary>
        public double dpiY;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the pixels.
        /// </summary>
        /// <value>
        /// The pixels.
        /// </value>
        public byte[] Pixels { get; set; }
        /// <summary>
        /// Gets or sets the bit image.
        /// </summary>
        /// <value>
        /// The bit image.
        /// </value>
        public WriteableBitmap BitImage { get; set; }

        /// <summary>
        /// Gets or sets the decoder.
        /// </summary>
        /// <value>
        /// The decoder.
        /// </value>
        public BitmapDecoder Decoder { get; set; }
        /// <summary>
        /// Gets or sets the height of the image.
        /// </summary>
        /// <value>
        /// The height of the image.
        /// </value>
        public int ImageHeight { get; set; }
        /// <summary>
        /// Gets or sets the width of the image.
        /// </summary>
        /// <value>
        /// The width of the image.
        /// </value>
        public int ImageWidth { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Image"/> class.
        /// </summary>
        public Image()
        {
            this.BitImage = new WriteableBitmap(600, 600);
        }

        #endregion

        #region Methods


        /// <summary>Sets the source image.</summary>
        /// <param name="stream">The stream.</param>
        /// <param name="storageFile">The storage file.</param>
        /// <param name="bitImage">The bit image.</param>
        public async Task SetSourceImage( StorageFile storageFile, BitmapImage bitImage)
        {
            IRandomAccessStream inputStream = await storageFile.OpenReadAsync();
            this.Decoder = await BitmapDecoder.CreateAsync(inputStream);
            var transform = new BitmapTransform
            {
                ScaledWidth = Convert.ToUInt32(bitImage.PixelWidth),
                ScaledHeight = Convert.ToUInt32(bitImage.PixelHeight)
            };

            var pixelData = await this.Decoder.GetPixelDataAsync(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Straight,
                transform,
                ExifOrientationMode.IgnoreExifOrientation,
                ColorManagementMode.DoNotColorManage
            );

            this.dpiX = this.Decoder.DpiX;
            this.dpiY = this.Decoder.DpiY;
            this.Pixels = pixelData.DetachPixelData();

            this.ImageHeight = (int)this.Decoder.PixelHeight;
            this.ImageWidth = (int)this.Decoder.PixelWidth;

            this.BitImage = new WriteableBitmap((int)this.Decoder.PixelWidth, (int)this.Decoder.PixelHeight);
            this.BitImage.SetSource(inputStream);
            
        }

        #endregion
    }
}

