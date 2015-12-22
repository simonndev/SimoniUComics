using GoComics.Shared.Extensions.Reactive;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoComics.Shared.Services
{
    public static class GoComicsClient
    {
        //public static IObservable<T> ApiGet<T>(string url, IDictionary<string, string> parameters, TimeSpan? timeout, IProgress<double> progress, bool observeOnDispatcher = true)
        //{
        //    var uri = GetUri(url, parameters);

        //    var clientHandler = new HttpClientHandler
        //    {
        //        CookieContainer = new CookieContainer(),
        //        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
        //    };

        //    var client = new HttpClient(clientHandler, true)
        //    {
        //        Timeout = timeout ?? TimeSpan.FromSeconds(10)
        //    };
        //    var request = new HttpRequestMessage(HttpMethod.Get, uri);
        //    //var disposal = new CompositeDisposable(request);

        //    var get =
        //        (from response in Observable.FromAsync(cancellation => client.SendAsync(request, cancellation))
        //         from stream in response.StatusCode == HttpStatusCode.OK ?
        //            Observable.FromAsync(() => ReadResponseStreamToMemoryAsync(response, progress)) :
        //            Observable.Throw<Stream>(new Exception("Could not read response stream: " + response.ReasonPhrase))
        //         from item in ReadJsonModels<T>(stream).ToObservable()
        //         select item);
        //    //.Finally(disposal.Dispose);

        //    return observeOnDispatcher ? get.ObserveOn(SynchronizationContext.Current) : get;

        //}

        public static IObservable<string> ApiGetJsonString(string url, IDictionary<string, string> parameters, IProgress<Tuple<long, long>> progress = null, bool observeOnDispatcher = true)
        {
            var uri = GetUri(url, parameters);
            var request = WebRequest.CreateHttp(uri);
            var get = request.GetStringAsObservable();

            return observeOnDispatcher ? get.ObserveOn(SynchronizationContext.Current) : get;
        }

        public static IObservable<byte[]> ApiGetBytes(string url, IDictionary<string, string> parameters = null, IProgress<Tuple<long, long>> progress = null, bool observeOnDispatcher = true)
        {
            var uri = GetUri(url, parameters);
            var request = WebRequest.CreateHttp(uri);
            var get = request.GetBytesAsObservable();

            return observeOnDispatcher ? get.ObserveOn(SynchronizationContext.Current) : get;
        }

        public static IObservable<T> ApiRequest<T>(string url, IDictionary<string, string> parameters, TimeSpan? timeout, IProgress<double> progress, bool observeOnDispatcher = true)
        {
            var uri = GetUri(url, parameters);

            var request = WebRequest.CreateHttp(uri);

            var get =
                (from response in Observable.FromAsync(request.GetResponseAsync)
                 from stream in Observable.FromAsync(() => ReadResponseStreamToMemoryAsync(response, progress))
                 from item in ReadJsonModels<T>(stream).ToObservable()
                 select item);
            //.Finally(disposal.Dispose);

            return observeOnDispatcher ? get.ObserveOn(SynchronizationContext.Current) : get;
        }

        //public static IObservable<Stream> ApiGetStream(string url, TimeSpan? timeout, IProgress<double> progress, bool observeOnDispatcher = true)
        //{
        //    using (HttpClient client = new HttpClient { Timeout = timeout ?? TimeSpan.FromSeconds(10) })
        //    {
        //        var request = new HttpRequestMessage(HttpMethod.Get, url);

        //        var getImageStream =
        //            (from response in Observable.FromAsync(cancellation => client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellation))
        //             from stream in response.StatusCode == HttpStatusCode.OK ?
        //                Observable.FromAsync(() => ReadResponseStreamToMemoryAsync(response, progress)) :
        //                Observable.Throw<Stream>(new Exception("Could not read response stream: " + response.ReasonPhrase))
        //             select stream);

        //        return observeOnDispatcher ? getImageStream.ObserveOn(SynchronizationContext.Current) : getImageStream;
        //    }
        //}

        //public static IObservable<byte[]> GetBytes(string url, TimeSpan? timeout, IProgress<Tuple<long, long>> progress, bool observeOnDispatcher = true)
        //{
        //    using (HttpClient client = new HttpClient { Timeout = timeout ?? TimeSpan.FromSeconds(10) })
        //    {
        //        var request = new HttpRequestMessage(HttpMethod.Get, url);

        //        var getBytes =
        //            (from response in Observable.FromAsync(cancellation => client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellation))
        //             from bytes in response.StatusCode == HttpStatusCode.OK ?
        //                Observable.FromAsync(() => ReadResponseBytesToMemoryAsync(response, progress)) :
        //                Observable.Throw<IEnumerable<byte[]>>(new Exception("Could not read response stream: " + response.ReasonPhrase))
        //             from bit in bytes.ToObservable()
        //             select bit);

        //        return observeOnDispatcher ? getBytes.ObserveOn(SynchronizationContext.Current) : getBytes;
        //    }
        //}

        private static IObservable<T> ObserveModelFromStream<T>(Stream stream) where T : new()
        {
            var jsonSerializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            return Observable.Create<T>(
                (observer, cancellationToken) => Task.Run(() =>
                {
                    using (StreamReader reader = new StreamReader(stream))
                    using (JsonTextReader jsonReader = new JsonTextReader(reader))
                    {
                        T item = default(T);
                        while (jsonReader.Read())
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            item = jsonSerializer.Deserialize<T>(jsonReader);
                            observer.OnNext(item);
                        }

                        observer.OnCompleted();
                    }
                }));
        }

        private static IEnumerable<T> ReadJsonModels<T>(Stream stream)
        {
            var jsonSerializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            using (StreamReader reader = new StreamReader(stream))
            using (JsonTextReader jsonReader = new JsonTextReader(reader))
            {
                while (jsonReader.Read())
                {
                    yield return jsonSerializer.Deserialize<T>(jsonReader);
                }
            }
        }

        private static async Task<Stream> ReadResponseStreamToMemoryAsync(WebResponse response, IProgress<double> progress)
        {
            var contentLength = response.ContentLength;
            var buffer = new byte[4096]; // 4KBs
            var read = 0;

            using (var responseStream = response.GetResponseStream())
            using (MemoryStream memoryStream = new MemoryStream())
            {
                do
                {
                    read = await responseStream.ReadAsync(buffer, 0, buffer.Length);
                    await memoryStream.WriteAsync(buffer, 0, read);

                    progress.Report(100 * (double)memoryStream.Length / contentLength);

                    // For display progress on UI
                    await Task.Delay(100);
                }
                while (read != 0);

                return memoryStream;
            }
        }

        private static async Task<IEnumerable<byte[]>> ReadResponseBytesToMemoryAsync(HttpResponseMessage response, IProgress<Tuple<long, long>> progress)
        {
            var contentLength = response.Content.Headers.ContentLength ?? -1L;
            var buffer = new byte[4096]; // 4KBs
            var read = 0;

            using (var responseStream = await response.Content.ReadAsStreamAsync())
            using (MemoryStream memoryStream = new MemoryStream())
            {
                do
                {
                    read = await responseStream.ReadAsync(buffer, 0, buffer.Length);
                    await memoryStream.WriteAsync(buffer, 0, read);

                    progress.Report(Tuple.Create(memoryStream.Length, contentLength));

                    // For display progress on UI
                    await Task.Delay(100);
                }
                while (read != 0);

                return new[] { memoryStream.ToArray() };
            }
        }

        //private static T ParseJsonFromStream<T>(MemoryStream stream)
        //{
        //    var rawJsonContent = Encoding.UTF8.GetString(stream.ToArray(), 0, (int)stream.Length);
        //    JObject jsonContent = JObject.Parse(rawJsonContent);

        //    IList<JToken> tokens = jsonContent["features"].Children().ToList();
        //    foreach (var token in tokens)
        //    {
        //        SearchResult searchResult = JsonConvert.DeserializeObject<SearchResult>(result.ToString());
        //        searchResults.Add(searchResult);
        //    }
        //}

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T">Response object in JSON format.</typeparam>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static async Task<T> CallApi<T>(string url, IDictionary<string, string> parameters, IProgress<Tuple<long, long>> progress)
        {
            var client = new HttpClient();
            var uri = GetUri(url, parameters);
            var response = await client.GetAsync(uri);

            var contentLength = response.Content.Headers.ContentLength ?? -1L;
            var responseStream = await response.Content.ReadAsStreamAsync();
            var buffer = new byte[4096]; // 4KBs
            var received = 0;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                while ((received = await responseStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    await memoryStream.WriteAsync(buffer, 0, received);
                    progress.Report(Tuple.Create(memoryStream.Length, contentLength));

                    // Display progress
                    await Task.Delay(100);
                }

                return ReadJsonFromStream<T>(memoryStream);
            }
        }

        public static async Task<string> GetJsonString(string url, IDictionary<string, string> parameters, IProgress<Tuple<long, long>> progress)
        {
            var client = new HttpClient();
            var uri = GetUri(url, parameters);
            var response = await client.GetAsync(uri);

            var contentLength = response.Content.Headers.ContentLength ?? -1L;
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var buffer = new byte[4096]; // 4KBs
                var received = 0;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    while ((received = await responseStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        await memoryStream.WriteAsync(buffer, 0, received);
                        progress.Report(Tuple.Create(memoryStream.Length, contentLength));

                        // Display progress
                        await Task.Delay(100);
                    }

                    return Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
        }

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

        /// <summary>
        /// Reads whole JSON data into an <c>Object</c>.
        /// </summary>
        /// <param name="stream">JSON stream received from WebResponse.</param>
        /// <returns></returns>
        private static T ReadJsonFromStream<T>(Stream stream)
        {
            var jsonSerializer = new JsonSerializer();

            using (StreamReader reader = new StreamReader(stream))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(reader))
                {
                    return (T)jsonSerializer.Deserialize<T>(jsonReader);
                }
            }
        }
    }
}