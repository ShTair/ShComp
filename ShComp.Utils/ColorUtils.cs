namespace ShComp;

public class ColorUtils
{
    public static (double R, double G, double B) HsvToRgb(double hue, double saturation, double value)
    {
        var h = hue / 60;
        //var c = saturation * value;
        var c = saturation;
        var x = c * (1 - Math.Abs(h % 2 - 1));
        var d = value - saturation;

        (double R, double G, double B) v = h switch
        {
            < 1 => (c, x, 0),
            < 2 => (x, c, 0),
            < 3 => (0, c, x),
            < 4 => (0, x, c),
            < 5 => (x, 0, c),
            <= 6 => (c, 0, x),
            _ => (0, 0, 0),
        };

        return (v.R + d, v.G + d, v.B + d);
    }

    public static (double Hue, double Saturation, double Value) RgbToHsv(int r, int g, int b)
    {
        throw new NotImplementedException();
    }
}
