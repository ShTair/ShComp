using ShComp.Threading;

namespace ShComp.Collections;

public class SemaphoreQueue<T>
{
    private readonly AsyncLock _lock;
    private readonly SemaphoreSlim _semaphore;
    private readonly Queue<T> _queue;

    public SemaphoreQueue()
    {
        _lock = new AsyncLock();
        _semaphore = new SemaphoreSlim(0);
        _queue = new Queue<T>();
    }

    public SemaphoreQueue(int capacity)
    {
        _semaphore = new SemaphoreSlim(0, capacity);
        _queue = new Queue<T>(capacity);
    }

    public void Enqueue(T item)
    {
        _lock.Lock(() =>
        {
            _queue.Enqueue(item);
            _semaphore.Release();
        });
    }

    public Task<T> DequeueAsync() => _lock.LockAsync(async () =>
    {
        await _semaphore.WaitAsync();
        return _queue.Dequeue();
    });

    public Task<T> DequeueAsync(CancellationToken cancellationToken) => _lock.LockAsync(async () =>
    {
        await _semaphore.WaitAsync(cancellationToken);
        return _queue.Dequeue();
    });

    public T Dequeue() => _lock.Lock(() =>
    {
        _semaphore.Wait();
        return _queue.Dequeue();
    });

    public T Dequeue(CancellationToken cancellationToken) => _lock.Lock(() =>
    {
        _semaphore.Wait(cancellationToken);
        return _queue.Dequeue();
    });
}
