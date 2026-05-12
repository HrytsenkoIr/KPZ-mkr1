using System.Collections;

namespace Patterns.Iterator;

public abstract class IteratorNode
{
}

public sealed class IteratorElementNode : IteratorNode
{
    public string TagName { get; }
    public List<IteratorNode> Children { get; } = new();

    public IteratorElementNode(string tagName)
    {
        TagName = tagName;
    }

    public void AddChild(IteratorNode child)
    {
        ArgumentNullException.ThrowIfNull(child);
        Children.Add(child);
    }
}

public sealed class IteratorTextNode : IteratorNode
{
    public string Text { get; }

    public IteratorTextNode(string text)
    {
        Text = text;
    }
}

public sealed class DepthFirstLightNodeIterator : IEnumerable<IteratorNode>
{
    private readonly IteratorNode _root;

    public DepthFirstLightNodeIterator(IteratorNode root)
    {
        _root = root;
    }

    public IEnumerator<IteratorNode> GetEnumerator()
    {
        Stack<IteratorNode> stack = new();
        stack.Push(_root);

        while (stack.Count > 0)
        {
            IteratorNode current = stack.Pop();
            yield return current;

            if (current is IteratorElementNode element)
            {
                for (int i = element.Children.Count - 1; i >= 0; i--)
                {
                    stack.Push(element.Children[i]);
                }
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public sealed class BreadthFirstLightNodeIterator : IEnumerable<IteratorNode>
{
    private readonly IteratorNode _root;

    public BreadthFirstLightNodeIterator(IteratorNode root)
    {
        _root = root;
    }

    public IEnumerator<IteratorNode> GetEnumerator()
    {
        Queue<IteratorNode> queue = new();
        queue.Enqueue(_root);

        while (queue.Count > 0)
        {
            IteratorNode current = queue.Dequeue();
            yield return current;

            if (current is IteratorElementNode element)
            {
                foreach (IteratorNode child in element.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public static class IteratorDemo
{
    public static IReadOnlyList<string> EnumerateDepthFirst()
    {
        IteratorElementNode root = BuildTree();
        return Describe(new DepthFirstLightNodeIterator(root));
    }

    public static IReadOnlyList<string> EnumerateBreadthFirst()
    {
        IteratorElementNode root = BuildTree();
        return Describe(new BreadthFirstLightNodeIterator(root));
    }

    private static IteratorElementNode BuildTree()
    {
        IteratorElementNode root = new("ul");

        IteratorElementNode firstItem = new("li");
        firstItem.AddChild(new IteratorTextNode("Item 1"));

        IteratorElementNode secondItem = new("li");
        secondItem.AddChild(new IteratorTextNode("Item 2"));

        root.AddChild(firstItem);
        root.AddChild(secondItem);

        return root;
    }

    private static IReadOnlyList<string> Describe(IEnumerable<IteratorNode> traversal)
    {
        List<string> result = new();

        foreach (IteratorNode node in traversal)
        {
            result.Add(node switch
            {
                IteratorElementNode element => $"<{element.TagName}>",
                IteratorTextNode text => text.Text,
                _ => node.GetType().Name
            });
        }

        return result;
    }
}
