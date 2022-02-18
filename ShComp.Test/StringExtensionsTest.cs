using System;
using Xunit;

namespace ShComp.Test;

public class StringExtensionsTest
{
    [Fact]
    public void AsIntTest()
    {
        Assert.Equal(1, "1".AsInt());
        Assert.Null("".AsInt());
    }

    [Fact]
    public void AsDateTest()
    {
        Assert.Equal(new DateOnly(2022, 2, 1), "2022/2/1 12:21".AsDate());
    }
}
