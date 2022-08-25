namespace ShComp.Collections.Tree;

public class NestedSet
{
    public static IList<T>? ConstructTree<T>(IEnumerable<T> nodes) where T : class, INestedSetTreeNode<T>
    {
        var roots = new List<T>();
        T? prev = null;

        foreach (var node in nodes.OrderBy(t => t.Left))
        {
            node.Children?.Clear();

            while (node.Left > (prev?.Right ?? int.MaxValue)) prev = prev!.Parent;
            if (node.Left == (prev?.Right ?? int.MaxValue)) throw new InvalidOperationException();

            node.Parent = prev;
            if (prev is null) roots.Add(node);
            else (prev.Children ??= new List<T>()).Add(node);

            prev = node;
        }

        return roots;
    }

    public static void CalculateFromTree<T>(IEnumerable<T> roots) where T : class, INestedSetTreeNode<T>
    {
        var location = 0;
        foreach (var root in roots)
        {
            CalculateFromTree(root, ref location);
        }
    }

    public static void CalculateFromTree<T>(T root) where T : class, INestedSetTreeNode<T>
    {
        var location = 0;
        CalculateFromTree(root, ref location);
    }

    private static void CalculateFromTree<T>(T node, ref int location) where T : class, INestedSetTreeNode<T>
    {
        node.Left = ++location;
        if (node.Children is not null)
        {
            foreach (var child in node.Children)
            {
                CalculateFromTree(child, ref location);
            }
        }
        node.Right = ++location;
    }
}

public interface INestedSetItem
{
    int Left { get; set; }

    int Right { get; set; }
}

public interface INestedSetTreeNode<T> : INestedSetItem where T : INestedSetTreeNode<T>
{
    T? Parent { get; set; }

    IList<T>? Children { get; set; }
}
