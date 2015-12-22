using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;

namespace GoComics.Shared.Extensions.Reactive
{
    public static class WebResponseExtensions
    {
        public static IObservable<byte[]> GetBytesAsObservable(this WebResponse response)
        {
            return Observable.Defer(() => response.GetResponseStream().ReadBytesAsyncAsObservable())
#if WINDOWS_UWP
                .Finally(response.Dispose)
#else
                .Finally(response.Close())
#endif
                .Aggregate(new List<byte>(), (list, bytes) =>
                {
                    list.AddRange(bytes);

                    return list;
                })
                .Select(list => list.ToArray());
        }

        public static IObservable<string> GetStringAsObservable(this WebResponse response)
        {
            return GetStringAsObservable(response, Encoding.UTF8);
        }

        public static IObservable<string> GetStringAsObservable(this WebResponse response, Encoding encoding)
        {
            return response.GetBytesAsObservable().Select(bytes => encoding.GetString(bytes, 0, bytes.Length));
        }
    }
}