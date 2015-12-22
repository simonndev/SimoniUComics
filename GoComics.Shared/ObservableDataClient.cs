using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GoComics.Shared
{
    public static class ObservableDataClient
    {
        public static IObservable<byte[]> GetBytesAsObservable(string url,
            IDictionary<string, string> parameters = null,
            IProgress<Tuple<long, long>> progress = null,
            bool observeOnDispatcher = true)
        {
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            var uri = GetUri(url, parameters);

            var get =
                from response in Observable.FromAsync(() => client.GetAsync(uri))
                from bytes in Observable.FromAsync(() => ReadResponseBytesAsync(response, progress))
                select bytes;

            return observeOnDispatcher ? get.ObserveOn(SynchronizationContext.Current) : get;
        }

        public static IObservable<Stream> GetStreamAsObservable(string url,
            IDictionary<string, string> parameters = null,
            IProgress<Tuple<long, long>> progress = null,
            bool observeOnDispatcher = true)
        {
            var client = new HttpClient {Timeout = TimeSpan.FromSeconds(10)};
            var uri = GetUri(url, parameters);

            var get =
                from response in Observable.FromAsync(() => client.GetAsync(uri))
                from stream in Observable.FromAsync(() => ReadResponseToMemoryAsync(response, progress))
                select stream;

            return observeOnDispatcher ? get.ObserveOn(SynchronizationContext.Current) : get;
        }

        public static IObservable<string> GetStringAsObservable(string url,
            IDictionary<string, string> parameters = null,
            IProgress<Tuple<long, long>> progress = null,
            bool observeOnDispatcher = true)
        {
            var client = new HttpClient {Timeout = TimeSpan.FromSeconds(10)};
            var uri = GetUri(url, parameters);

            var get =
                from response in Observable.FromAsync(() => client.GetAsync(uri))
                from s in Observable.FromAsync(() => ReadResponseStringAsync(response, progress))
                select s;

            return observeOnDispatcher ? get.ObserveOn(SynchronizationContext.Current) : get;
        }

        public static IObservable<T> GetJsonItemAsObservable<T>(string url,
            string tokenName,
            IDictionary<string, string> parameters = null,
            IProgress<Tuple<long, long>> progress = null,
            bool observeOnDispatcher = true)
        {
            var client = new HttpClient {Timeout = TimeSpan.FromSeconds(10)};
            var uri = GetUri(url, parameters);

            var get =
                from response in Observable.FromAsync(() => client.GetAsync(uri))
                from s in Observable.FromAsync(() => ReadResponseStringAsync(response, progress))
                from item in GetJsonItems<T>(s, tokenName).ToObservable()
                select item;

            return observeOnDispatcher ? get.ObserveOn(SynchronizationContext.Current) : get;
        }

        private static async Task<MemoryStream> ReadResponseToMemoryAsync(HttpResponseMessage response, IProgress<Tuple<long, long>> progress = null)
        {
            long contentLength = response.Content.Headers.ContentLength ?? -1L;
            
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var buffer = new byte[contentLength];
                int offset = 0, read = 0;

                do
                {
                    read = await stream.ReadAsync(buffer, offset, buffer.Length - offset);
                    offset += read;

                    progress?.Report(Tuple.Create((long) offset, contentLength));
                } while (read > 0);

                return new MemoryStream(buffer);
            }
        }

        private static async Task<byte[]> ReadResponseBytesAsync(HttpResponseMessage response, IProgress<Tuple<long, long>> progress = null)
        {
            using (var memory = await ReadResponseToMemoryAsync(response, progress))
            {
                return memory.ToArray();
            }

            //long contentLength = response.Content.Headers.ContentLength ?? -1L;
            //var buffer = new byte[4096]; // 4KBs
            //using (var stream = await response.Content.ReadAsStreamAsync())
            //using (var memory = new MemoryStream())
            //{
            //    int read;
            //    while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            //    {
            //        await memory.WriteAsync(buffer, 0, read);
            //        progress?.Report(Tuple.Create(memory.Length, contentLength));
            //    }

            //    return memory.ToArray();
            //}
        }

        /// <summary>
        /// Reads UTF8 Encoding string from response bytes.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        private static async Task<string> ReadResponseStringAsync(HttpResponseMessage response,
            IProgress<Tuple<long, long>> progress = null)
        {
            var responseBytes = await ReadResponseBytesAsync(response, progress);
            return Encoding.UTF8.GetString(responseBytes);
        }

        private static IEnumerable<T> GetJsonItems<T>(string json, string tokenName)
        {
            JToken token = JToken.Parse(json);

            if (token.Type == JTokenType.Array)
            {
                foreach (var item in token)
                {
                    yield return item.ToObject<T>();
                }
            }

            if (token.Type == JTokenType.Object)
            {
                foreach (var item in token[tokenName])
                {
                    yield return item.ToObject<T>();
                }
            }

            yield return default(T);
        }

        /// <summary>
        /// Get data-source URI for GET request.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static Uri GetUri(string url, IDictionary<string, string> parameters = null)
        {
            Uri uri = new Uri(url);

            if (null != parameters && 0 < parameters.Count)
            {
                StringBuilder sb = new StringBuilder();
                int count = parameters.Count;
                foreach (var pair in parameters)
                {
                    string format = --count == 0 ? "{0}={1}" : "{0}={1}&";
                    sb.AppendFormat(format, pair.Key, Uri.EscapeDataString(pair.Value));
                }

                UriBuilder uriBuilder = new UriBuilder(uri)
                {
                    Query = sb.ToString()
                };

                return uriBuilder.Uri;
            }

            return uri;
        }
    }
}