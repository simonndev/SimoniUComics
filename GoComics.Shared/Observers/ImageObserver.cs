using System;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using GoComics.Shared.Services;

namespace GoComics.Shared.Observers
{
    public class DownloadImageEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Determine if the the image is ready to open.
        /// </summary>
        public bool IsImageReady { get; set; }

        /// <summary>
        /// The physical path where the image data was saved to.
        /// </summary>
        public string FilePath { get; set; }

        public BitmapImage ImageSource { get; set; }
    }

    public class DownloadImageFailedEventArgs : RoutedEventArgs
    {
        public string ImageUrl { get; set; }
        public Exception Error { get; set; }
    }

    public class ImageObserver : IObserver<Stream>
    {
        public event EventHandler<DownloadImageEventArgs> DownloadImageCompleted;
        public event EventHandler<DownloadImageFailedEventArgs> DownloadImageFailed;
        public event Action<Stream> Completed; 

        private readonly IImageStorageService _imageStorage;
        private readonly string _imageUrl;
        private string _imageFilePath;
        private BitmapImage _imageSource;

        public ImageObserver(IImageStorageService imageStorage, string imageUrl)
        {
            this._imageStorage = imageStorage;
            this._imageUrl = imageUrl;
        }

       

        /// <summary>
        /// The folder where the image was saved to.
        /// </summary>
        public string FolderName { get; set; }

        public async void OnCompleted()
        {
            BitmapImage image = await this._imageStorage.Open(string.Empty);

            // TODO: Notify that image is ready.
            var args = new DownloadImageEventArgs
            {
                IsImageReady = true,
                FilePath = this._imageFilePath,
                ImageSource = image
            };

            this.OnDownloadImageCompleted(args);
        }

        public void OnError(Exception error)
        {
            var args = new DownloadImageFailedEventArgs
            {
                ImageUrl = this._imageUrl,
                Error = error
            };

            this.OnDownloadImageFailed(args);
        }

        public async void OnNext(Stream value)
        {
            // TODO: Save image for caching.
            this._imageFilePath = await this._imageStorage.Save(value, this.FolderName);
        }

        private void OnDownloadImageCompleted(DownloadImageEventArgs e)
        {
            this.DownloadImageCompleted?.Invoke(this, e);
        }

        private void OnDownloadImageFailed(DownloadImageFailedEventArgs e)
        {
            this.DownloadImageFailed?.Invoke(this, e);
        }
    }
}