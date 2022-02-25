namespace ShComp.Security;

public class PasswordGenerator
{
    private readonly Random _r = new();
    private readonly List<Container> _containers = new();

    private int _count;

    public void AddParameter(int count, string? letters = null)
    {
        char[] array;
        if (letters == null) array = _containers.SelectMany(t => t.Letters).ToArray();
        else array = letters.ToArray();

        if (count > array.Length) throw new Exception();

        _containers.Add(new Container(count, array));
        _count += count;
    }

    public string Generate()
    {
        var pass = new char[_count];
        var index = 0;
        foreach (var container in _containers)
        {
            Shuffle(container.Letters, container.Count);
            for (int i = 0; i < container.Count; i++)
            {
                pass[index++] = container.Letters[i];
            }
        }

        Shuffle(pass, pass.Length);
        return new string(pass);
    }

    private void Shuffle(char[] array, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var r = _r.Next(i, array.Length);
            (array[i], array[r]) = (array[r], array[i]);
        }
    }

    private struct Container
    {
        public int Count { get; }

        public char[] Letters { get; }

        public Container(int count, char[] letters)
        {
            Count = count;
            Letters = letters;
        }
    }
}
