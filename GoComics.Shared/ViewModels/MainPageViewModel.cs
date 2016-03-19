using GoComics.Shared.Models;
using GoComics.Shared.Observers;
using GoComics.Shared.Services;
using Newtonsoft.Json;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using GoComics.Shared.Models.UI;

namespace GoComics.Shared.ViewModels
{
    public partial class MainPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IGoComicsService _service;
        private readonly IDataStorageService _dataStorageService;
        private readonly IImageStorageService _imageStorageService;

        private double _percentComplete;
        private IReadOnlyCollection<FeatureModel> _allTimeFeatureCollection;

        public MainPageViewModel(INavigationService navigationService, IGoComicsService service, IDataStorageService dataStorageService, IImageStorageService imageStorageService)
        {
            this._navigationService = navigationService;
            this._service = service;
            this._dataStorageService = dataStorageService;
            this._imageStorageService = imageStorageService;

            //ShowDetailCommand = new DelegateCommand<ItemClickEventArgs>((args) =>
            //{
            //    News selectedNews = args.ClickedItem as News;
            //    _navigationService.Navigate("Detail", selectedNews);
            //});
        }

        public double PercentComplete
        {
            get { return this._percentComplete; }
            set { base.SetProperty(ref this._percentComplete, value); }
        }

        public IReadOnlyCollection<FeatureModel> AllTimeFeatureCollection
        {
            get { return _allTimeFeatureCollection; }
            private set { SetProperty(ref _allTimeFeatureCollection, value); }
        }

        public override async void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);

            List<FeatureModel> features = await this._dataStorageService.LoadAsync<List<FeatureModel>>("features");
            if (null == features)
            {
                var progressIndicator = new Progress<Tuple<long, long>>((progress) =>
                {
                    Debug.WriteLine("Current: {0} Total: {1}", progress.Item1, progress.Item2);
                });

                var allFeaturesObserver = new AllFeaturesObserver(this._service, this._imageStorageService);
                allFeaturesObserver.GetNext += (feature) =>
                {
                    // TODO: try open the image
                };

                allFeaturesObserver.Completed += async (featureList) =>
                {
                    // TODO: save to local storage
                    await this._dataStorageService.SaveAsync("features", featureList);
                };

                this._service.GetAllFeatures(allFeaturesObserver, progressIndicator);
            }

            var progressIndicator1 = new Progress<Tuple<long, long>>((progress) =>
            {
                Debug.WriteLine("Current: {0} Total: {1}", progress.Item1, progress.Item2);
            });

            var allTimeFeaturesObserver = new AllTimeFeaturesObserver(this._service, this._imageStorageService);
            allTimeFeaturesObserver.Completed += (featureList) =>
            {
                this.AllTimeFeatureCollection = new ReadOnlyCollection<FeatureModel>(featureList);
            };

            this._service.GetAllTimeFeatures(allTimeFeaturesObserver, progressIndicator1);
        }

        private void ObserverError(Exception obj)
        {
            throw obj;
        }

        private void Observer_Completed(string allFeaturesJsonString)
        {
            Debug.WriteLine(allFeaturesJsonString);

            Features features = JsonConvert.DeserializeObject<Features>(allFeaturesJsonString);
        }
    }
}