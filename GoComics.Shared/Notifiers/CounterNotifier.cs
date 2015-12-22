using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace GoComics.Shared.Notifiers
{
    public enum CounterStatus
    {
        Empty,
        Increment,
        Decrement,
        Max
    }

    public class CounterNotifier : IObservable<CounterStatus>, INotifyPropertyChanged
    {
        private readonly object syncRoot = new object();
        private readonly Subject<CounterStatus> CounterTrigger = new Subject<CounterStatus>();
        private int count;

        /// <summary>
        /// Setup max count of signal.
        /// </summary>
        public CounterNotifier(int max = int.MaxValue)
        {
            if (max <= 0)
            {
                throw new ArgumentException(nameof(max));
            }

            Max = max;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Max { get; private set; }

        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Increment count and notify status.
        /// </summary>
        public IDisposable Increase(int incrementCount = 1)
        {
            if (incrementCount < 0)
            {
                throw new ArgumentException(nameof(incrementCount));
            }

            lock (syncRoot)
            {
                if (Count == Max)
                {
                    return Disposable.Empty;
                }
                else if (incrementCount + Count > Max)
                {
                    Count = Max;
                }
                else
                {
                    Count += incrementCount;
                }

                CounterTrigger.OnNext(CounterStatus.Increment);
                if (Count == Max)
                {
                    CounterTrigger.OnNext(CounterStatus.Max);
                }

                return Disposable.Create(() => this.Decrease(incrementCount));
            }
        }

        /// <summary>
        /// Decrement count and notify status.
        /// </summary>
        public void Decrease(int decrementCount = 1)
        {
            if (decrementCount < 0)
            {
                throw new ArgumentException(nameof(decrementCount));
            }

            lock (syncRoot)
            {
                if (Count == 0)
                {
                    return;
                }
                else if (Count - decrementCount < 0)
                {
                    Count = 0;
                }
                else
                {
                    Count -= decrementCount;
                }

                CounterTrigger.OnNext(CounterStatus.Decrement);
                if (Count == 0)
                {
                    CounterTrigger.OnNext(CounterStatus.Empty);
                }
            }
        }

        public IDisposable Subscribe(IObserver<CounterStatus> observer)
        {
            return CounterTrigger.Subscribe(observer);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}