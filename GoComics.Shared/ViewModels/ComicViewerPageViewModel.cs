using GoComics.Shared.Models;
using GoComics.Shared.Observers;
using GoComics.Shared.Services;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace GoComics.Shared.ViewModels
{
    public class ComicViewerPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IGoComicsService _service;
        private FeatureItem _featureItem;
        private string _comicPageLink;
        private bool _canLoadPreviousPage = false;
        private bool _canLoadNextPage = false;

        public ComicViewerPageViewModel(INavigationService navigationService, IGoComicsService service)
        {
            this._navigationService = navigationService;
            this._service = service;

            LoadPreviousPageCommand = new DelegateCommand(this.DoLoadPreviousPage);
            LoadNextPageCommand = new DelegateCommand(this.DoLoadNextPage);
        }

        public ICommand LoadPreviousPageCommand { get; private set; }
        public ICommand LoadNextPageCommand { get; private set; }

        public FeatureItem Comic
        {
            get { return this._featureItem; }
            private set
            {
                SetProperty(ref _featureItem, value);
            }
        }

        public string ComicPageLink
        {
            get { return this._comicPageLink; }
            private set
            {
                SetProperty(ref _comicPageLink, value);
            }
        }

        public bool CanLoadPreviousPage
        {
            get { return this._canLoadPreviousPage; }
            private set
            {
                SetProperty(ref _canLoadPreviousPage, value);
            }
        }

        public bool CanLoadNextPage
        {
            get { return this._canLoadNextPage; }
            private set
            {
                SetProperty(ref _canLoadNextPage, value);
            }
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);

            var featureId = Convert.ToInt32(e.Parameter);
            ApiStringObserver observer = new ApiStringObserver();
            observer.Completed += this.GetComicPageCompleted;

            this._service.GetRecentFeaturePage(featureId, observer);
        }

        private void DoLoadPreviousPage()
        {
            if (this.CanLoadPreviousPage)
            {
                var url = this.Comic.PreviousLink + ".json";
                ApiStringObserver observer = new ApiStringObserver();
                observer.Completed += this.GetComicPageCompleted;

                this._service.GetComicPage(url, observer);
            }
        }

        private void DoLoadNextPage()
        {
            if (this.CanLoadNextPage)
            {
                var url = this.Comic.NextLink + ".json";
                ApiStringObserver observer = new ApiStringObserver();
                observer.Completed += this.GetComicPageCompleted;

                this._service.GetComicPage(url, observer);
            }
        }

        private void GetComicPageCompleted(string json)
        {
            this.Comic = JsonConvert.DeserializeObject<FeatureItem>(json);
            this.ComicPageLink = this.Comic != null ? this.Comic.ImageLink : string.Empty;
            this.CanLoadPreviousPage = Comic != null && !string.IsNullOrEmpty(Comic.PreviousLink);
            this.CanLoadNextPage = Comic != null && !string.IsNullOrEmpty(Comic.NextLink); ;
        }
    }
}