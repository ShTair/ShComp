namespace ShComp;

public static class EnumerableExtensions
{
    /// <summary>
    /// 指定した配列の中身を、指定した個数だけランダムな順序で返します。<br />
    /// 指定した配列そのものの順序も変更されます。
    /// </summary>
    public static IEnumerable<T> Shuffle<T>(this IList<T> items, int count)
    {
        var random = new Random();
        for (int i = 0; i < count; i++)
        {
            var r = random.Next(i, items.Count);
            if (i != r) (items[i], items[r]) = (items[r], items[i]);
            yield return items[i];
        }
    }

    /// <summary>
    /// 指定した配列の中身をランダムな順序で返します。<br />
    /// 指定した配列そのものの順序も変更されます。
    /// </summary>
    public static IEnumerable<T> Shuffle<T>(this IList<T> items)
    {
        return Shuffle(items, items.Count);
    }

    /// <summary>
    /// 指定したコレクションの中身を、指定した個数だけランダムな順序で返します。<br />
    /// 内部的に配列を生成しています。
    /// </summary>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items, int count)
    {
        return Shuffle(items.ToArray(), count);
    }

    /// <summary>
    /// 指定したコレクションの中身をランダムな順序で返します。<br />
    /// 内部的に配列を生成しています。
    /// </summary>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items)
    {
        return Shuffle(items.ToArray());
    }
}
