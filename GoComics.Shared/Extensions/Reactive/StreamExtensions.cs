using System;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace GoComics.Shared.Extensions.Reactive
{
    public static class StreamExtensions
    {
        private static IObservable<int> ReadAsyncAsObservable(this Stream stream, byte[] buffer, int offset, int count)
        {
            return Observable.FromAsync(() => stream.ReadAsync(buffer, offset, count));
        }

        public static IObservable<byte[]> ReadBytesAsyncAsObservable(this Stream stream, int chunkSize = 65536)
        {
            return Observable.Defer(() => Observable.Return(new byte[chunkSize], Scheduler.CurrentThread))
                .SelectMany(buffer => stream.ReadAsyncAsObservable(buffer, 0, chunkSize), (buffer, read) => new { buffer, read })
                .Repeat().TakeWhile(action => action.read != 0)
                .Select(action =>
                {
                    if (action.read == chunkSize)
                    {
                        return action.buffer;
                    }

                    var newBuffer = new byte[action.read];
                    Array.Copy(action.buffer, newBuffer, action.read);

                    return newBuffer;
                })
#if WINDOWS_UWP
                .Finally(stream.Dispose);
#else
                .Finally(stream.Close());
#endif
        }
    }
}