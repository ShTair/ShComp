namespace ShComp.Threading;

public class Retryer
{
    private readonly int _max;
    private readonly Func<int, TimeSpan> _delayFunc;

    public Retryer(int max, Func<int, TimeSpan> delayFunc)
    {
        _max = max;
        _delayFunc = delayFunc;
    }

    public Retryer(int max, TimeSpan delay) : this(max, _ => delay) { }

    public async Task InvokeAsync(Func<int, Task> func)
    {
        for (var i = 0; i < _max - 1; i++)
        {
            try { await func(i); return; }
            catch { }

            var delay = _delayFunc(i);
            await Task.Delay(delay);
        }

        await func(_max - 1);
    }

    public async Task<T> InvokeAsync<T>(Func<int, Task<T>> func)
    {
        for (var i = 0; i < _max - 1; i++)
        {
            try { return await func(i); }
            catch { }

            var delay = _delayFunc(i);
            await Task.Delay(delay);
        }

        return await func(_max - 1);
    }

    public async Task InvokeAsync(Func<int, CancellationToken, Task> func, CancellationToken cancellationToken)
    {
        for (var i = 0; i < _max - 1; i++)
        {
            try { await func(i, cancellationToken); return; }
            catch { }

            var delay = _delayFunc(i);
            await Task.Delay(delay, cancellationToken);
        }

        await func(_max - 1, cancellationToken);
    }

    public async Task<T> InvokeAsync<T>(Func<int, CancellationToken, Task<T>> func, CancellationToken cancellationToken)
    {
        for (var i = 0; i < _max - 1; i++)
        {
            try { return await func(i, cancellationToken); }
            catch { }

            var delay = _delayFunc(i);
            await Task.Delay(delay, cancellationToken);
        }

        return await func(_max - 1, cancellationToken);
    }
}
