using System;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;

namespace GoComics.Shared.Extensions.Reactive
{
    public static class WebRequestExtensions
    {
        public static IObservable<WebResponse> GetResponseAsyncAsObservable(this WebRequest request)
        {
            return Observable.FromAsync(request.GetResponseAsync);
        }

        public static IObservable<byte[]> GetBytesAsObservable(this WebRequest request)
        {
            return Observable.Defer(() => request.GetResponseAsyncAsObservable()).SelectMany(response => response.GetBytesAsObservable());
        }

        public static IObservable<string> GetStringAsObservable(this WebRequest request)
        {
            return GetStringAsObservable(request, Encoding.UTF8);
        }

        public static IObservable<string> GetStringAsObservable(this WebRequest request, Encoding encoding)
        {
            return Observable.Defer(() => request.GetResponseAsyncAsObservable()).SelectMany(response => response.GetStringAsObservable());
        }
    }
}