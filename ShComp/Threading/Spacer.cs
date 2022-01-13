namespace ShComp.Threading;

public class Spacer
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly TimeSpan _delay;

    private DateTime _next;
    private CancellationTokenSource? _cts;

    public Spacer(TimeSpan delay)
    {
        _delay = delay;
    }

    public async Task<bool> InvokeAsync(Func<Task> func)
    {
        CancellationTokenSource cts;
        TimeSpan delay;

        await _semaphore.WaitAsync();
        try
        {
            _cts?.Cancel();

            delay = _next - DateTime.Now;
            if (delay <= TimeSpan.Zero)
            {
                try { await func(); }
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
                    try { await func(); }
                    finally { _next = DateTime.Now + _delay; }
                    return true;
                }
            }
            finally { _semaphore.Release(); }
        }

        return false;
    }
}
