using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace GoComics.Shared.Services
{
    public interface IImageStorageService
    {
        Task<string> Save(Stream imageSource, string fileName, bool replaceIfExist = false, params string[] folders);

        Task<BitmapImage> Open(string fileName, params string[] folders);
    }

    public class ImageStorageService : IImageStorageService
    {
        public async Task<string> Save(Stream imageSource, string fileName, bool replaceIfExist = false, params string[] folders)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            //if (folders != null && folders.Any())
            //{
            //    foreach (string folderName in folders)
            //    {
            //        folder = await folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            //    }
            //}

            StorageFile imageFile = await folder.CreateFileAsync(fileName,
                replaceIfExist ? CreationCollisionOption.ReplaceExisting : CreationCollisionOption.OpenIfExists);
            
            using (Stream imageOutputStream = await imageFile.OpenStreamForWriteAsync())
            {
                await imageSource.CopyToAsync(imageOutputStream);
            }

            return imageFile.Path;
        }

        public async Task<BitmapImage> Open(string fileName, params string[] folders)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            //if (folders != null && folders.Any())
            //{
            //    foreach (string folderName in folders)
            //    {
            //        if (await ItemExistsAsync(folder, folderName))
            //        {
            //            folder = await folder.GetFolderAsync(folderName);
            //        }
            //    }
            //}

            if (!await FileExistsAsync(folder, fileName))
            {
                return new BitmapImage(new Uri("ms-appx:///Assets/Reload.png"));
            }

            StorageFile file = await folder.GetFileAsync(fileName);
            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
            {
                BitmapImage image = new BitmapImage();
                await image.SetSourceAsync(stream);

                return image;
            }
        }

        private async Task<bool> FileExistsAsync(StorageFolder folder, string name)
        {
            var item = await folder.TryGetItemAsync(name);
            return item != null;
        }

        private async Task<bool> ItemExistsAsync(StorageFolder folder, string name, bool isFile = false)
        {
            var item = await folder.TryGetItemAsync(name);
            if (item == null)
            {
                return false;
            }

            if (isFile && item.IsOfType(StorageItemTypes.File))
            {
                return true;
            }

            return !isFile && item.IsOfType(StorageItemTypes.Folder);
        }
    }
}