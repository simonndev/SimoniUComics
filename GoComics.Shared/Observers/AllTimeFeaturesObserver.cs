using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using GoComics.Shared.Models;
using GoComics.Shared.Models.UI;
using GoComics.Shared.Services;

namespace GoComics.Shared.Observers
{
    public class AllTimeFeaturesObserver : IObserver<Featured>
    {
        public event Action<List<FeatureModel>> Completed;

        private readonly List<FeatureModel> _allFeatureModels = new List<FeatureModel>();
        private readonly IGoComicsService _service;
        private readonly IImageStorageService _imageStorage;
        
        public AllTimeFeaturesObserver(IGoComicsService service, IImageStorageService imageStorage)
        {
            this._service = service;
            this._imageStorage = imageStorage;
        }

        public void OnCompleted()
        {
            Completed?.Invoke(this._allFeatureModels);
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public async void OnNext(Featured value)
        {
            if (value == null)
            {
                // there is a case where the observer returns null value when parsing JArray
                return;
            }

            FeatureModel feature = new FeatureModel
            {
                Id = value.FeatureId,
                Title = value.Feature.Title,
                Author = value.Feature.Author,
                Icon = new BitmapImage(new Uri("ms-appx:///Assets/blank_face.gif")),
                IconUrl = value.Feature.IconUrl,
                IsPoliticalSlant = value.Feature.IsPoliticalSlant
            };

            feature.Icon = await this._imageStorage.Open(feature.Id.ToString(), "Features");

            //this._service.DownloadImage(feature.IconUrl, observer);
            this._allFeatureModels.Add(feature);
        }
    }
}
