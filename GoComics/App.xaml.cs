using GoComics.Shared.Models;
using GoComics.Shared.Services;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Mvvm;
using Prism.Unity.Windows;
using Prism.Windows.AppModel;
using Prism.Windows.Navigation;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.UI.Notifications;
using Windows.UI.Xaml;

namespace GoComics
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : PrismUnityApplication
    {
        // Bootstrap: App singleton service declarations
        private TileUpdater _tileUpdater;

        public IEventAggregator EventAggregator { get; set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
        }

        protected override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            EventAggregator = new EventAggregator();

            // Register MvvmAppBase services with the container so that view models can take dependencies on them
            Container.RegisterInstance<ISessionStateService>(SessionStateService);
            Container.RegisterInstance<INavigationService>(NavigationService);
            Container.RegisterInstance<IEventAggregator>(EventAggregator);
            //Container.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));

            // Register any app specific types with the container
            Container.RegisterType<IGoComicsService, GoComicsService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDataStorageService, DataStorageService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IImageStorageService, ImageStorageService>(new ContainerControlledLifetimeManager());

            // Set a factory for the ViewModelLocator to use the container to construct view models so their
            // dependencies get injected by the container
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewModelTypeName = string.Format( "GoComics.Shared.ViewModels.{0}ViewModel, GoComics.Shared", viewType.Name);
                var viewModelType = Type.GetType(viewModelTypeName);
                if (viewModelType == null)
                {
                    viewModelTypeName = string.Format(CultureInfo.InvariantCulture, "GoComics.Shared.ViewModels.{0}ViewModel, GoComics.Shared.Windows, Version=1.0.0.0, Culture=neutral", viewType.Name);
                    viewModelType = Type.GetType(viewModelTypeName);
                }

                return viewModelType;
            });

            _tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            //_tileUpdater.StartPeriodicUpdate(new Uri(Constants.ServerAddress + "/api/TileNotification"), PeriodicUpdateRecurrence.HalfHour);

            return base.OnInitializeAsync(args);
        }

        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            if (args != null && !string.IsNullOrEmpty(args.Arguments))
            {
                // The app was launched from a Secondary Tile
                // Navigate to the item's page
                NavigationService.Navigate("ComicViewer", args.Arguments);
            }
            else
            {
                // Navigate to the initial page
                NavigationService.Navigate("Main", null);
            }

            Window.Current.Activate();
            return Task.FromResult<object>(null);
        }

        protected override void OnRegisterKnownTypesForSerialization()
        {
            SessionStateService.RegisterKnownType(typeof(ComicIcons));
            SessionStateService.RegisterKnownType(typeof(Feature));
            SessionStateService.RegisterKnownType(typeof(Featured));
            SessionStateService.RegisterKnownType(typeof(Features));
            SessionStateService.RegisterKnownType(typeof(FeatureItem));
        }
    }
}