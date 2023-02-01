using System.Text;

namespace ShComp.IO;

public class TimeTextWriter : TextWriter
{
    private bool _isBeginningOfLine;

    private readonly string _format;

    public TextWriter BaseWriter { get; }

    public override Encoding Encoding => BaseWriter.Encoding;

    public TimeTextWriter(TextWriter baseWriter) : this(baseWriter, "HH:mm:ss.f ") { }

    public TimeTextWriter(TextWriter baseWriter, string format)
    {
        _isBeginningOfLine = true;
        BaseWriter = baseWriter;
        _format = format;
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
            BaseWriter.Write(DateTime.Now.ToString(_format));
        }

        BaseWriter.Write(value);
    }

    public static void SetOut(string format = "HH:mm:ss.f ")
    {
        var writer = new TimeTextWriter(Console.Out, format);
        Console.SetOut(writer);
    }
}
