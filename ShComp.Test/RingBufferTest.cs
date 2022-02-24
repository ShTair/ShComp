using ShComp.Collections;
using System.Linq;
using Xunit;

namespace ShComp.Test;

public class RingBufferTest
{
    [Fact]
    public void CountTest()
    {
        var buffer = new RingBuffer<int?>(3);
        buffer.Add(1);
        Assert.Single(buffer);

        buffer.Add(null);
        buffer.Add(2);
        Assert.Equal(3, buffer.Count);

        buffer.Add(null);
        buffer.Add(3);
        Assert.Equal(3, buffer.Count);
    }

    [Fact]
    public void EnumerableTest()
    {
        var buffer = new RingBuffer<int>(3) { 1, 2, 3, 4, 5 };
        var items = buffer.ToArray();
        Assert.Equal(4, items[1]);
    }

    [Fact]
    public void RemoveTest()
    {
        var buffer = new RingBuffer<int>(3) { 1, 2, 3, 4, 5 };
        buffer.Remove();
        Assert.Equal(2, buffer.Count);
        Assert.Equal(4, buffer[3]);
        Assert.Equal(4, buffer.ToArray()[0]);

        buffer.Remove();
        Assert.True(buffer.Remove());
        Assert.Empty(buffer);

        Assert.False(buffer.Remove());
    }

    [Fact]
    public void ForeachTest()
    {
        var buffer = new RingBuffer<int>(3) { 1 };
        foreach (var item in buffer)
        {
            Assert.Equal(1, item);
        }
    }
}
