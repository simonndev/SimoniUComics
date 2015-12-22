using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace GoComics.Shared
{
    /// <summary>
    /// http://grogansoft.com/blog/?p=1160
    /// </summary>
    public static class LiveTileBuilder
    {
        public static async Task MakeTiles(FrameworkElement large, FrameworkElement medium)
        {
            string largeFileName = "large.png", mediumFileName = "medium.png";
            await MakeTileImageFile(largeFileName, large);
            await MakeTileImageFile(mediumFileName, medium);

            SetLiveTileToSingleImage(largeFileName, mediumFileName);
        }

        private static async Task MakeTileImageFile(string fileName, FrameworkElement control)
        {
            StorageFile tileImageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName,
                CreationCollisionOption.ReplaceExisting);

            CachedFileManager.DeferUpdates(tileImageFile);
            using (IRandomAccessStream fileStream = await tileImageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                await CaptureTileImage(fileStream, control);
            }

            await CachedFileManager.CompleteUpdatesAsync(tileImageFile);
        }

        private static async Task<RenderTargetBitmap> CaptureTileImage(IRandomAccessStream fileStream,
            FrameworkElement control)
        {
            RenderTargetBitmap tileBitmap = new RenderTargetBitmap();
            await tileBitmap.RenderAsync(control);

            IBuffer pixels = await tileBitmap.GetPixelsAsync();
            double dpi = DisplayInformation.GetForCurrentView().LogicalDpi;
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Straight,
                (uint) (tileBitmap.PixelHeight),
                (uint) (tileBitmap.PixelWidth),
                dpi,
                dpi,
                pixels.ToArray());

            await encoder.FlushAsync();

            return tileBitmap;
        }

        static void SetLiveTileToSingleImage(string wideImageFileName, string mediumImageFileName)
        {
            // Construct the tile content as a string
            string content = $@"
                                <tile>
                                    <visual> 
                                        <binding template='TileSquareImage'>
                                           <image id='1' src='ms-appdata:///local/{mediumImageFileName}' />
                                        </binding> 
                                         <binding template='TileWideImage' branding='none'>
                                           <image id='1' src='ms-appdata:///local/{wideImageFileName}' />
                                        </binding>
  
                                    </visual>
                                </tile>";

            // Load the string into an XmlDocument
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);

            // Then create the tile notification
            var notification = new TileNotification(doc);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }
    }
}
