using ShComp.Collections.Tree;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ShComp.Test;

public class NestedSetTest
{
    [Fact]
    public void ConstructTreeTest()
    {
        var items = new List<TestItem>
        {
            new TestItem("Clothing", 1, 22),
            new TestItem("Men's", 2, 9),
            new TestItem("Women's", 10, 21),
            new TestItem("Suits", 3, 8),
            new TestItem("Slacks", 4, 5),
            new TestItem("Jackets", 6, 7),
            new TestItem("Dresses", 11, 16),
            new TestItem("Skirts", 17, 18),
            new TestItem("Blouses", 19, 20),
            new TestItem("Evening Gowns", 12, 13),
            new TestItem("Sun Dresses", 14, 15),
        };

        var roots = NestedSet.ConstructTree(items);

        Assert.Equal(11, roots?.Sum(t => t.GetAll().Count()));
        Assert.Equal("Clothing", roots![0].Name);
        Assert.Equal("Sun Dresses", roots![0].Children![1].Children![0].Children![1].Name);
    }

    [Fact]
    public void CalculateFromTreeTest()
    {
        var root = new TestItem("Clothing",
            new TestItem("Men's",
                new TestItem("Suits",
                    new TestItem("Slacks"),
                    new TestItem("Jackets"))),
            new TestItem("Women's",
                new TestItem("Dresses",
                    new TestItem("Evening Gowns"),
                    new TestItem("Sun Dresses")),
                new TestItem("Skirts"),
                new TestItem("Blouses")));

        NestedSet.CalculateFromTree(new[] { root });

        var items = root.GetAll().ToDictionary(t => t.Name);
        Assert.Equal(13, items["Evening Gowns"].Right);
        Assert.Equal(17, items["Skirts"].Left);
    }

    private class TestItem : INestedSetTreeNode<TestItem>
    {
        public string Name { get; }

        public int Left { get; set; }

        public int Right { get; set; }

        public TestItem? Parent { get; set; }

        public IList<TestItem>? Children { get; set; }

        public TestItem(string name, int left, int right)
        {
            Name = name;
            Left = left;
            Right = right;
        }

        public TestItem(string name, params TestItem[] children)
        {
            Name = name;
            Children = children;
        }

        public override string ToString() => $"{Name} {Left} {Right}";

        public IEnumerable<TestItem> GetAll()
        {
            yield return this;
            if (Children is null) yield break;
            foreach (var child in Children)
            {
                foreach (var item in child.GetAll())
                {
                    yield return item;
                }
            }
        }
    }
}
