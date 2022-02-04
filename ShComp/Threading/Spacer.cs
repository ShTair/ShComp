namespace ShComp.Threading;

/// <summary>
/// 指定した時間間隔をあけて処理を実行します。
/// 次の実行時間までに複数の処理が登録された場合、最後に登録した処理のみが、次の実行時間まで待機され実行されます。
/// </summary>
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

    /// <summary>
    /// 処理を実行します。
    /// 前に処理を実行してから、まだ待機時間が経過していない場合、残りの時間を待機してから実行します。
    /// 実行が完了した場合trueを返します。
    /// 待機中にさらに次の処理が待機中になった場合、即座に待機が終了しfalseを返します。
    /// </summary>
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
        try { await task; } catch { }

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
