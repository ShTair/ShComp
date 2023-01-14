namespace ShComp.Updater;

internal class DelayWatcher : IDisposable
{
    private bool _isDisposed;

    private readonly object _lock = new();
    private CancellationTokenSource? _cts;

    private readonly Watcher _watcher;
    private readonly TimeSpan _delay;

    public event Action? Changed;

    public DelayWatcher(string path, TimeSpan delay)
    {
        _watcher = new Watcher(path);
        _watcher.Changed += OnChanged;

        _delay = delay;
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        _watcher.Dispose();

        lock (_lock)
        {
            _cts?.Cancel();
        }
    }

    private void OnChanged()
    {
        CancellationTokenSource cts;
        lock (_lock)
        {
            if (_isDisposed) return;

            _cts?.Cancel();
            _cts = cts = new CancellationTokenSource();
        }

        Task.Delay(_delay, _cts.Token).ContinueWith(t =>
        {
            if (t.IsCompletedSuccessfully) Changed?.Invoke();
        });
    }
}
