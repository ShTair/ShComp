namespace ShComp.Threading;

public class ChatteringBlocker<T>
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly TimeSpan _delay;
    private readonly IEqualityComparer<T> _equalityComparer;

    private DateTime _next;
    private CancellationTokenSource? _cts;

    public T Value { get; private set; }

    public event Action<T>? ValueChanged;

    public ChatteringBlocker(TimeSpan delay, T initialValue, IEqualityComparer<T> equalityComparer)
    {
        _delay = delay;
        Value = initialValue;
        _equalityComparer = equalityComparer;
    }

    public async Task<bool> ChangeValueAsync(T value)
    {
        CancellationTokenSource cts;
        TimeSpan delay;

        await _semaphore.WaitAsync();
        try
        {
            _cts?.Cancel();

            if (_equalityComparer.Equals(Value, value)) return false;

            delay = _next - DateTime.Now;
            if (delay <= TimeSpan.Zero)
            {
                Value = value;
                try { ValueChanged?.Invoke(value); }
                finally { _next = DateTime.Now + _delay; }
                return true;
            }

            cts = new CancellationTokenSource();
            _cts = cts;
        }
        finally { _semaphore.Release(); }

        var task = Task.Delay(delay, cts.Token);
        await task;

        if (!task.IsCanceled)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!cts.IsCancellationRequested)
                {
                    Value = value;
                    try { ValueChanged?.Invoke(value); }
                    finally { _next = DateTime.Now + _delay; }
                    return true;
                }
            }
            finally { _semaphore.Release(); }
        }

        return false;
    }
}
