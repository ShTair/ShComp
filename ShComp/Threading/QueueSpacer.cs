namespace ShComp.Threading;

/// <summary>
/// 指定した間隔をあけて順番に処理を実行するメソッドを提供します。
/// </summary>
public class QueueSpacer
{
    private readonly AsyncLock _lock;
    private readonly TimeSpan _delay;
    private DateTime _next;

    public QueueSpacer(TimeSpan delay)
    {
        _lock = new AsyncLock();
        _delay = delay;
    }

    public Task InvokeAsync(Func<Task> func) => _lock.LockAsync(async () =>
    {
        var delay = _next - DateTime.Now;
        if (delay > TimeSpan.Zero) await Task.Delay(delay);
        try { await func(); }
        finally { _next = DateTime.Now + _delay; }
    });

    public Task<T> InvokeAsync<T>(Func<Task<T>> func) => _lock.LockAsync(async () =>
    {
        var delay = _next - DateTime.Now;
        if (delay > TimeSpan.Zero) await Task.Delay(delay);
        try { return await func(); }
        finally { _next = DateTime.Now + _delay; }
    });
}
