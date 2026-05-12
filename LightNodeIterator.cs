using System.Collections;
using System.Collections.Generic;

public sealed class DepthFirstLightNodeIterator : IEnumerable<LightNode>
{
    // ЦЕ ПАТТЕРН: ITERATOR
    private readonly LightNode _root;

    public DepthFirstLightNodeIterator(LightNode root)
    {
        _root = root;
    }

    public IEnumerator<LightNode> GetEnumerator()
    {
        Stack<LightNode> stack = new();
        stack.Push(_root);

        while (stack.Count > 0)
        {
            LightNode current = stack.Pop();
            yield return current;

            if (current is not LightElementNode element)
            {
                continue;
            }

            for (int i = element.Children.Count - 1; i >= 0; i--)
            {
                stack.Push(element.Children[i]);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public sealed class BreadthFirstLightNodeIterator : IEnumerable<LightNode>
{
    // ЦЕ ПАТТЕРН: ITERATOR
    private readonly LightNode _root;

    public BreadthFirstLightNodeIterator(LightNode root)
    {
        _root = root;
    }

    public IEnumerator<LightNode> GetEnumerator()
    {
        Queue<LightNode> queue = new();
        queue.Enqueue(_root);

        while (queue.Count > 0)
        {
            // Бере вузол з початку черги і повертає його назовні
            LightNode current = queue.Dequeue();
            yield return current;

            if (current is not LightElementNode element)
            {
                continue;
            }

            foreach (LightNode child in element.Children)
            {
                // додає дітей в кінець черги
                queue.Enqueue(child);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
