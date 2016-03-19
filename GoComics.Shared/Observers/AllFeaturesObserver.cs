using GoComics.Shared.Models;
using GoComics.Shared.Models.UI;
using GoComics.Shared.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace GoComics.Shared.Observers
{
    public class AllFeaturesObserver : IObserver<Feature>
    {
        private readonly List<FeatureModel> _allFeatureModels = new List<FeatureModel>();
        private readonly IGoComicsService _service;
        private readonly IImageStorageService _imageStorage;

        public event Action<FeatureModel> GetNext;

        public event Action<List<FeatureModel>> Completed;

        public AllFeaturesObserver(IGoComicsService service, IImageStorageService imageStorage)
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

        public void OnNext(Feature value)
        {
            if (value == null)
            {
                return;
            }

            Debug.WriteLine("ID: {0} - {1}", value.Id, value.Title);
            string iconUrl = value.Icons.Medium ?? value.IconUrl;

            FeatureModel feature = new FeatureModel
            {
                Id = value.Id,
                Title = value.Title,
                Author = value.Author,
                IconUrl = iconUrl,
                IsPoliticalSlant = value.IsPoliticalSlant
            };

            ApiResultObserverBase<Stream> observer = new ApiResultObserverBase<Stream>();
            observer.Completed += async (imageStream) =>
            {
                // we only need to save the icon here.
                feature.IconFilePath = await this._imageStorage.Save(imageStream,
                    feature.Id.ToString(), true, "Features");
            };
            observer.Error += error =>
            {
                // TODO: show the reload icon
                Debug.WriteLine("Download Image Error - Image {0}. {1}", feature.IconUrl, error.Message);
            };

            if (!string.IsNullOrWhiteSpace(iconUrl))
            {
                this._service.DownloadImage(iconUrl, observer);
            }
            else
            {
                Debug.WriteLine("ID: {0} - {1}", value.Id, value.Title);
            }

            GetNext?.Invoke(feature);

            this._allFeatureModels.Add(feature);
        }
    }
}