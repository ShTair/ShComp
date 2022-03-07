namespace ShComp;

public static class EnumerableExtensions
{
    /// <summary>
    /// 指定した配列の中身をランダムな順序で返します。<br />
    /// 指定した配列そのものの順序も変更されます。
    /// </summary>
    public static IEnumerable<T> Shuffle<T>(this IList<T> items)
    {
        var random = new Random();
        for (int i = 0; i < items.Count; i++)
        {
            var r = random.Next(i, items.Count);
            if (i != r) (items[i], items[r]) = (items[r], items[i]);
            yield return items[i];
        }
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
