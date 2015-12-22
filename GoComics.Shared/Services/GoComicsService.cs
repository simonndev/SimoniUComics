using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using GoComics.Shared.Models;

namespace GoComics.Shared.Services
{
    public interface IGoComicsService
    {
        void GetAllFeatures(IObserver<Feature> observer, IProgress<Tuple<long, long>> progress = null);

        void GetAllTimeFeatures(IObserver<Featured> observer, IProgress<Tuple<long, long>> progress = null);

        void GetPopularFeatures(IObserver<string> observer, IProgress<Tuple<long, long>> progress = null);

        void GetNewestFeatures(IObserver<string> observer, IProgress<Tuple<long, long>> progress = null);

        void GetRecentFeaturePage(int featureId, IObserver<string> observer,
            IProgress<Tuple<long, long>> progress = null);

        /// <summary>
        /// Get comic-page JSON data.
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <param name="observer"></param>
        /// <param name="progress"></param>
        void GetComicPage(string imageUrl, IObserver<byte[]> observer, IProgress<Tuple<long, long>> progress = null);

        /// <summary>
        /// Gets comic-page image bytes.
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <param name="observer"></param>
        /// <param name="progress"></param>
        void GetComicPage(string imageUrl, IObserver<string> observer, IProgress<Tuple<long, long>> progress = null);

        void DownloadImage(string imageUrl, IObserver<Stream> observer, IProgress<Tuple<long, long>> progress = null);
    }

    public class GoComicsService : IGoComicsService
    {
        private const string ApiKey = "FHE576CHD922"; // KAJS6R5FJAS3
        private const string AllFeatures = "http://www.gocomics.com/api/features.json";
        private const string AllTimeFeatures = "http://www.gocomics.com/api/featured.json";
        private const string PopularFeatures = "http://www.gocomics.com/api/featured_comics.json";
        private const string NewestFeatures = "http://www.gocomics.com/api/newest.json";
        private const string RecentFeaturePage = "http://www.gocomics.com/api/feature/{0}/recent.json";

        private const int TIMEOUT = 10; // TODO: should be read from configuration file.
        private static readonly object SyncRoot = new object();
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(TIMEOUT);

        private static readonly IDictionary<Guid, IDisposable> CurrentSubscriptions =
            new Dictionary<Guid, IDisposable>();

        public void GetAllFeatures(IObserver<Feature> observer, IProgress<Tuple<long, long>> progress = null)
        {
            //merge just two dictionaries without duplicate key checks
            //dicA.Concat(dicB).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)

            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"client_code", ApiKey}
            };

            SubscribeJsonItem(observer, AllFeatures, "features", parameters, progress);
        }

        public void GetAllTimeFeatures(IObserver<Featured> observer, IProgress<Tuple<long, long>> progress = null)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"client_code", ApiKey}
            };

            SubscribeJsonItem(observer, AllTimeFeatures, string.Empty, parameters, progress);
        }

        public void GetPopularFeatures(IObserver<string> observer, IProgress<Tuple<long, long>> progress = null)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"client_code", "KAJS6R5FJAS3"}
            };

            SubscribeString(observer, PopularFeatures, parameters, progress);
        }

        public void GetNewestFeatures(IObserver<string> observer, IProgress<Tuple<long, long>> progress = null)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"client_code", "KAJS6R5FJAS3"}
            };

            SubscribeString(observer, NewestFeatures, parameters, progress);
        }

        public void GetRecentFeaturePage(int featureId, IObserver<string> observer,
            IProgress<Tuple<long, long>> progress = null)
        {
            var url = string.Format(RecentFeaturePage, featureId);

            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"client_code", "KAJS6R5FJAS3"}
            };

            SubscribeString(observer, url, parameters, progress);
        }

        public void GetComicPage(string imageUrl, IObserver<string> observer,
            IProgress<Tuple<long, long>> progress = null)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"client_code", "KAJS6R5FJAS3"}
            };

            SubscribeString(observer, imageUrl, parameters, progress);
        }

        public void GetComicPage(string imageUrl, IObserver<byte[]> observer,
            IProgress<Tuple<long, long>> progress = null)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"client_code", "KAJS6R5FJAS3"}
            };

            SubscribeBytes(observer, imageUrl, parameters, progress);
        }

        public void DownloadImage(string imageUrl, IObserver<Stream> observer,
            IProgress<Tuple<long, long>> progress = null)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"client_code", "KAJS6R5FJAS3"}
            };

            SubscribeImage(observer, imageUrl, parameters, progress);
        }

        private static void SubscribeBytes(IObserver<byte[]> observer,
            string url,
            IDictionary<string, string> parameters,
            IProgress<Tuple<long, long>> progress = null)
        {
            var subscriptionKey = Guid.NewGuid();

            var observable = ObservableDataClient.GetBytesAsObservable(url, parameters, progress);
            var disposal = observable.Finally(() => CleanupSubscription(subscriptionKey)).Subscribe(observer);

            lock (SyncRoot)
            {
                CurrentSubscriptions.Add(subscriptionKey, disposal);
            }
        }

        private static void SubscribeImage(IObserver<Stream> observer,
            string url,
            IDictionary<string, string> parameters,
            IProgress<Tuple<long, long>> progress = null)
        {
            var subscriptionKey = Guid.NewGuid();

            var observable = ObservableDataClient.GetStreamAsObservable(url, parameters, progress);
            var disposal = observable.Finally(() => CleanupSubscription(subscriptionKey)).Subscribe(observer);

            lock (SyncRoot)
            {
                CurrentSubscriptions.Add(subscriptionKey, disposal);
            }
        }

        private static void SubscribeString(IObserver<string> observer,
            string url,
            IDictionary<string, string> parameters,
            IProgress<Tuple<long, long>> progress = null)
        {
            var subscriptionKey = Guid.NewGuid();

            var observable = ObservableDataClient.GetStringAsObservable(url, parameters, progress);
            var disposal = observable.Finally(() => CleanupSubscription(subscriptionKey)).Subscribe(observer);

            lock (SyncRoot)
            {
                CurrentSubscriptions.Add(subscriptionKey, disposal);
            }
        }

        private static void SubscribeJsonItem<T>(IObserver<T> observer,
            string url,
            string tokenName,
            IDictionary<string, string> parameters,
            IProgress<Tuple<long, long>> progress = null)
        {
            var subscriptionKey = Guid.NewGuid();

            var observable = ObservableDataClient.GetJsonItemAsObservable<T>(url, tokenName, parameters, progress);
            var disposal = observable.Finally(() => CleanupSubscription(subscriptionKey)).Subscribe(observer);

            lock (SyncRoot)
            {
                CurrentSubscriptions.Add(subscriptionKey, disposal);
            }
        }

        private static void CleanupSubscription(Guid key)
        {
            lock (SyncRoot)
            {
                if (!CurrentSubscriptions.ContainsKey(key))
                {
                    Debug.WriteLine(
                        string.Format("GoComicsService::Cleanup - CurrentSubscriptions does not contain key {0}", key));
                    return;
                }

                CurrentSubscriptions[key].Dispose();
                CurrentSubscriptions.Remove(key);
            }
        }
    }
}