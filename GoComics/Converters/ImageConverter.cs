using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace GoComics.Converters
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string)
            {
                return value;
            }

            if (value is Uri)
            {
                return value;
            }

            return parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private BitmapImage DownloadImage(Uri imageUri)
        {
            BitmapImage imageSource = new BitmapImage(imageUri);
            imageSource.DownloadProgress += (sender, progressArgs) =>
            {

            };

            imageSource.ImageOpened += (sender, routed) =>
            {

            };

            imageSource.ImageFailed += (sender, error) =>
            {

            };

            return imageSource;
        }
    }
}
