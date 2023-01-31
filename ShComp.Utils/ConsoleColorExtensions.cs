namespace ShComp;

public static class ConsoleColorExtensions
{
    public static ConsoleColorRestorer SetForeground(this ConsoleColor color)
    {
        var restorer = new ConsoleColorRestorer();
        Console.ForegroundColor = color;
        return restorer;
    }

    public static ConsoleColorRestorer SetBackground(this ConsoleColor color)
    {
        var restorer = new ConsoleColorRestorer();
        Console.BackgroundColor = color;
        return restorer;
    }
}

public sealed class ConsoleColorRestorer : IDisposable
{
    private readonly (ConsoleColor, ConsoleColor) _defaults;

    public ConsoleColorRestorer()
    {
        _defaults = (Console.ForegroundColor, Console.BackgroundColor);
    }

    public void Dispose()
    {
        (Console.ForegroundColor, Console.BackgroundColor) = _defaults;
    }
}
