using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace GoComics.Shared.Services
{
    public interface ILocalStorageService
    {
        Task Write<T>(string fileName, T data, bool replaceIfExists = false);

        Task<T> Read<T>(string fileName);
    }

    public class JsonDataSourceService : ILocalStorageService
    {
        /// <summary>
        /// Reads JSON content from a file and returns an object from the JSON.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<T> Read<T>(string fileName)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            if (!await FileExistsAsync(folder, fileName))
            {
                return default(T);
            }

            StorageFile file = await folder.GetFileAsync(fileName);
            using (Stream inputStream = await file.OpenStreamForReadAsync())
            using (StreamReader streamReader = new StreamReader(inputStream))
            {
                JsonSerializer serializer = new JsonSerializer();
                return (T) serializer.Deserialize(streamReader, typeof (T));
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

        /// <summary>
        /// Writes an object in JSON format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task Write<T>(string fileName, T data, bool replaceIfExists = false)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync(fileName,
                replaceIfExists ? CreationCollisionOption.ReplaceExisting : CreationCollisionOption.OpenIfExists);

            using (Stream stream = await file.OpenStreamForWriteAsync())
            using (StreamWriter writer = new StreamWriter(stream))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, data);
            }
        }
    }
}