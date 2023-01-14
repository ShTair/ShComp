namespace ShComp.Updater;

internal class Watcher : IDisposable
{
    private readonly FileSystemWatcher _watcher;

    public event Action? Changed;

    public Watcher(string path)
    {
        _watcher = new FileSystemWatcher(path);

        _watcher.Changed += OnChanged;
        _watcher.Deleted += OnChanged;
        _watcher.Created += OnChanged;
        _watcher.Renamed += OnChanged;

        _watcher.IncludeSubdirectories = true;
        _watcher.EnableRaisingEvents = true;
    }

    public void Dispose()
    {
        _watcher.Dispose();
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        Changed?.Invoke();
    }
}
