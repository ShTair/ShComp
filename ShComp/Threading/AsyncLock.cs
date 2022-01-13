namespace ShComp.Threading;

public readonly struct AsyncLock
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public readonly void Lock(Action func)
    {
        _semaphore.Wait();
        try { func(); }
        finally { _semaphore.Release(); }
    }

    public readonly T Lock<T>(Func<T> func)
    {
        _semaphore.Wait();
        try { return func(); }
        finally { _semaphore.Release(); }
    }

    public readonly async Task LockAsync(Action func)
    {
        await _semaphore.WaitAsync();
        try { func(); }
        finally { _semaphore.Release(); }
    }

    public readonly async Task LockAsync(Func<Task> func)
    {
        await _semaphore.WaitAsync();
        try { await func(); }
        finally { _semaphore.Release(); }
    }

    public readonly async Task<T> LockAsync<T>(Func<T> func)
    {
        await _semaphore.WaitAsync();
        try { return func(); }
        finally { _semaphore.Release(); }
    }

    public readonly async Task<T> LockAsync<T>(Func<Task<T>> func)
    {
        await _semaphore.WaitAsync();
        try { return await func(); }
        finally { _semaphore.Release(); }
    }

    public readonly async Task<Releaser> WaitAsync()
    {
        await _semaphore.WaitAsync();
        return new Releaser(_semaphore);
    }

    /// <summary>
    /// Disposeを呼び出すことでロックを開放します。Disposeは絶対に1回しか呼び出さないでください。
    /// </summary>
    public readonly struct Releaser : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;

        public Releaser(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
        }

        public readonly void Dispose()
        {
            _semaphore.Release();
        }
    }
}
