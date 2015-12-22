using System;
using System.Collections.Generic;

namespace GoComics.Shared.Observers
{
    public class ApiResultObserverBase<TResult> : IObserver<TResult>
    {
        public ApiResultObserverBase()
        {
            this.Result = default(TResult);
        }

        public event Action<TResult> Completed;

        public event Action<Exception> Error;

        public TResult Result { get; protected set; }

        public virtual void OnCompleted()
        {
            Completed?.Invoke(Result);
        }

        public virtual void OnError(Exception error)
        {
            Error?.Invoke(error);
        }

        public virtual void OnNext(TResult value)
        {
            this.Result = value;
        }
    }
}