namespace ShComp.Updater;

internal class ChangeDetector : IDisposable
{
    private readonly TaskCompletionSource _tcs;
    private readonly DelayWatcher _watcher;

    public ChangeDetector(string path, TimeSpan delay)
    {
        _tcs = new TaskCompletionSource();
        _watcher = new DelayWatcher(path, delay);
        _watcher.Changed += OnChanged;
    }

    public void Dispose()
    {
        _watcher.Dispose();
        _tcs.TrySetCanceled();
    }

    private void OnChanged()
    {
        _tcs.TrySetResult();
    }

    public Task WaitAsync()
    {
        return _tcs.Task;
    }
}
