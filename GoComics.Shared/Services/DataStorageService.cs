using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace GoComics.Shared.Services
{
    public interface IDataStorageService
    {
        Task<string> SaveAsync<T>(string fileName, T data, bool replaceIfExists = true);

        Task<T> LoadAsync<T>(string fileName);
    }

    public class DataStorageService : IDataStorageService
    {
        private async Task<bool> FileExistsAsync(StorageFolder folder, string name)
        {
            var item = await folder.TryGetItemAsync(name);
            return item != null;
        }

        public async Task<string> SaveAsync<T>(string fileName, T data, bool replaceIfExists = true)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync(fileName,
                replaceIfExists
                    ? CreationCollisionOption.ReplaceExisting
                    : CreationCollisionOption.FailIfExists);

            if (!file.IsAvailable)
            {
                return string.Empty;
            }

            using (Stream stream = await file.OpenStreamForWriteAsync())
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(stream, data);
            }

            return file.Path;
        }

        public async Task<T> LoadAsync<T>(string fileName)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            if (!await FileExistsAsync(folder, fileName))
            {
                return default(T);
            }

            StorageFile file = await folder.GetFileAsync(fileName);
            using (Stream inputStream = await file.OpenStreamForReadAsync())
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(inputStream);
            }
        }
    }
}