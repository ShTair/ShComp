namespace ShComp;

public static class StringExtensions
{
    public static int? AsInt(this string s) => int.TryParse(s, out var result) ? result : null;

    public static double? AsDouble(this string s) => double.TryParse(s, out var result) ? result : null;

    public static DateTime? AsDateTime(this string s) => DateTime.TryParse(s, out var result) ? result : null;

    public static DateOnly? AsDate(this string s) => DateTime.TryParse(s, out var result) ? DateOnly.FromDateTime(result) : null;

    public static DateOnly? AsDateOnly(this string s) => DateOnly.TryParse(s, out var result) ? result : null;
}
