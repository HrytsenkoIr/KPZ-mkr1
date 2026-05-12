using System.Collections.Generic;

namespace Patterns.Visitor;

public interface IVisitableNode
{
    void Accept(ILightNodeVisitor visitor);
}

public interface ILightNodeVisitor
{
    void VisitElement(VisitorElementNode element);
    void VisitText(VisitorTextNode text);
}

public abstract class VisitorNode : IVisitableNode
{
    public abstract void Accept(ILightNodeVisitor visitor);
}

public sealed class VisitorElementNode : VisitorNode
{
    public string TagName { get; }
    public List<VisitorNode> Children { get; } = new();

    public VisitorElementNode(string tagName)
    {
        TagName = tagName;
    }

    public void AddChild(VisitorNode child)
    {
        ArgumentNullException.ThrowIfNull(child);
        Children.Add(child);
    }

    public override void Accept(ILightNodeVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        visitor.VisitElement(this);
    }
}

public sealed class VisitorTextNode : VisitorNode
{
    public string Text { get; }

    public VisitorTextNode(string text)
    {
        Text = text;
    }

    public override void Accept(ILightNodeVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        visitor.VisitText(this);
    }
}

public sealed class HtmlStatisticsVisitor : ILightNodeVisitor
{
    public int ElementCount { get; private set; }
    public int TextNodeCount { get; private set; }
    public int TotalTextLength { get; private set; }

    public void VisitElement(VisitorElementNode element)
    {
        ElementCount++;
    }

    public void VisitText(VisitorTextNode text)
    {
        TextNodeCount++;
        TotalTextLength += text.Text.Length;
    }
}

public static class VisitorTraversal
{
    public static void VisitDepthFirst(VisitorNode root, ILightNodeVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(visitor);

        Stack<VisitorNode> stack = new();
        stack.Push(root);

        while (stack.Count > 0)
        {
            VisitorNode current = stack.Pop();
            current.Accept(visitor);

            if (current is VisitorElementNode element)
            {
                for (int i = element.Children.Count - 1; i >= 0; i--)
                {
                    stack.Push(element.Children[i]);
                }
            }
        }
    }
}

public static class VisitorDemo
{
    public static HtmlStatisticsVisitor CollectStatistics()
    {
        VisitorElementNode root = new("ul");

        VisitorElementNode firstItem = new("li");
        firstItem.AddChild(new VisitorTextNode("Item 1"));

        VisitorElementNode secondItem = new("li");
        secondItem.AddChild(new VisitorTextNode("Item 2"));

        root.AddChild(firstItem);
        root.AddChild(secondItem);

        HtmlStatisticsVisitor visitor = new();
        VisitorTraversal.VisitDepthFirst(root, visitor);
        return visitor;
    }
}
