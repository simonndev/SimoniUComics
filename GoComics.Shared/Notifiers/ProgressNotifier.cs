using System;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;

namespace GoComics.Shared.Notifiers
{
    public class ProgressNotifier<T> : IObservable<T>, IProgress<T>
    {
        private readonly IScheduler _scheduler;
        private readonly Subject<T> _progressTrigger = new Subject<T>();

        /// <summary>
        /// Use scheduler is Scheduler.Immediate.
        /// </summary>
        public ProgressNotifier()
        {
            this._scheduler = Scheduler.Immediate;
        }

        /// <summary>
        /// Use scheduler is argument's scheduler.
        /// </summary>
        public ProgressNotifier(IScheduler scheduler)
        {
            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            this._scheduler = scheduler;
        }

        /// <summary>
        /// Push value to subscribers on setuped scheduler.
        /// </summary>
        public void Report(T value)
        {
            _scheduler.Schedule(() => _progressTrigger.OnNext(value));
        }

        /// <summary>
        /// Push value to subscribers on setuped scheduler.
        /// </summary>
        public IDisposable Report(T value, TimeSpan dueTime)
        {
            return _scheduler.Schedule(dueTime, () => _progressTrigger.OnNext(value));
        }

        /// <summary>
        /// Push value to subscribers on setuped scheduler.
        /// </summary>
        public IDisposable Report(T value, DateTimeOffset dueTime)
        {
            return _scheduler.Schedule(dueTime, () => _progressTrigger.OnNext(value));
        }

        /// <summary>
        /// Subscribe observer.
        /// </summary>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            return _progressTrigger.Subscribe(observer);
        }
    }
}