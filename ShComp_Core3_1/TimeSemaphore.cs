using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShComp
{
    class TimeSemaphore
    {
        private SemaphoreSlim _sem;

        public TimeSpan Period { get; }

        public int CurrentCount => _sem.CurrentCount;

        public TimeSemaphore(int count, TimeSpan period)
        {
            _sem = new SemaphoreSlim(count);
            Period = period;
        }

        public async Task WaitAsync()
        {
            await _sem.WaitAsync();
            _ = Task.Delay(Period).ContinueWith(_ => _sem.Release());
        }
    }
}
