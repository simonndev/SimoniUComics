using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoComics.Shared.Extensions.Reactive
{
    public static class HttpResponseMessageExtensions
    {
//        public static IObservable<Stream> ReadResponseStreamAsyncAsObservable(this HttpResponseMessage response)
//        {
//            return Observable.FromAsync(() => response.Content.ReadAsStreamAsync());
//        }

//        public static IObservable<byte[]> GetBytesAsObservable(this HttpResponseMessage response)
//        {
//            return Observable.Defer(() => response.ReadResponseStreamAsyncAsObservable()).ReadBytesAsyncAsObservable())
//#if WINDOWS_UWP
//                .Finally(() => response.Dispose())
//#else
//                .Finally(() => response.Close())
//#endif
//                .Aggregate(new List<byte>(), (list, bytes) =>
//                {
//                    list.AddRange(bytes);

//                    return list;
//                })
//                .Select(list => list.ToArray());
//        }

//        public static IObservable<string> GetStringAsObservable(this HttpResponseMessage response)
//        {
//            return GetStringAsObservable(response, Encoding.UTF8);
//        }

//        public static IObservable<string> GetStringAsObservable(this HttpResponseMessage response, Encoding encoding)
//        {
//            return response.GetBytesAsObservable().Select(bytes => encoding.GetString(bytes, 0, bytes.Length));
//        }
    }
}
