namespace ShComp;

public static class StringExtensions
{
    public static short? AsShort(this string s) => short.TryParse(s, out var result) ? result : null;

    public static int? AsInt(this string s) => int.TryParse(s, out var result) ? result : null;

    public static double? AsDouble(this string s) => double.TryParse(s, out var result) ? result : null;

    public static DateTime? AsDateTime(this string s) => DateTime.TryParse(s, out var result) ? result : null;

    /// <summary>
    /// DateTimeとして読み込み、DateOnlyに変換して返します。
    /// </summary>
    public static DateOnly? AsDate(this string s) => DateTime.TryParse(s, out var result) ? DateOnly.FromDateTime(result) : null;

    /// <summary>
    /// 文字列に時間が含まれている場合、失敗します。
    /// </summary>
    public static DateOnly? AsDateOnly(this string s) => DateOnly.TryParse(s, out var result) ? result : null;
}
