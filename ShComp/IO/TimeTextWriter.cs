using System.Text;

namespace ShComp.IO;

public class TimeTextWriter : TextWriter
{
    private bool _isBeginningOfLine;

    public TextWriter BaseWriter { get; }

    public override Encoding Encoding => BaseWriter.Encoding;

    public TimeTextWriter(TextWriter baseWriter)
    {
        _isBeginningOfLine = true;
        BaseWriter = baseWriter;
    }

    public override void Write(char value)
    {
        if (value is '\r' or '\n')
        {
            _isBeginningOfLine = true;
        }
        else if (_isBeginningOfLine)
        {
            _isBeginningOfLine = false;
            BaseWriter.Write($"{DateTime.Now:HH:mm:ss.f} ");
        }

        BaseWriter.Write(value);
    }

    public static void SetOut()
    {
        var writer = new TimeTextWriter(Console.Out);
        Console.SetOut(writer);
    }
}
