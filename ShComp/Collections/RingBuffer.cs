using System.Collections;

namespace ShComp.Collections;

public class RingBuffer<T> : IList<T>
{
    private readonly T?[] _buffer;

    private int _nextIndex;
    private int _count;

    public int HeadIndex => _nextIndex - _count;

    public RingBuffer(int capacity)
    {
        _buffer = new T[capacity];
    }

    public T this[int index]
    {
        get
        {
            if (index < _nextIndex - _count || index >= _nextIndex) throw new ArgumentOutOfRangeException(nameof(index));
            return _buffer[index % _buffer.Length]!;
        }
        set => throw new NotImplementedException();
    }

    public int Count => _count;

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        _buffer[_nextIndex % _buffer.Length] = item;
        _nextIndex++;
        if (_count < _buffer.Length) _count++;
    }

    public void Clear()
    {
        Array.Clear(_buffer, 0, _buffer.Length);
        _nextIndex = 0;
        _count = 0;
    }

    public bool Contains(T item)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        for (var i = 0; i < _count && i < array.Length - arrayIndex; i++)
        {
            array[arrayIndex + i] = this[HeadIndex + i];
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < _count; i++)
        {
            yield return _buffer[(HeadIndex + i) % _buffer.Length]!;
        }
    }

    public int IndexOf(T item)
    {
        throw new NotImplementedException();
    }

    public void Insert(int index, T item)
    {
        throw new NotImplementedException();
    }

    public bool Remove()
    {
        if (_count <= 0) return false;
        _buffer[(_nextIndex - _count) % _buffer.Length] = default;
        _count--;
        return true;
    }

    public bool Remove(T item)
    {
        throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
