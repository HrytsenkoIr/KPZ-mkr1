using System;
using System.Collections.Generic;

internal static class Program
{
    private static void Main()
    {
        LoggingLightElementNode ul = new("ul", DisplayType.Block, ClosingType.Normal);
        ul.AddClass("my-list");
        ul.SetStyle("margin", "8px");

        LightElementNode li1 = new("li", DisplayType.Block, ClosingType.Normal);
        LightTextNode text1 = new("Item 1");
        li1.AddChild(text1);

        LightElementNode li2 = new("li", DisplayType.Block, ClosingType.Normal);
        LightTextNode text2 = new("Item 2");
        li2.AddChild(text2);

        LightElementNode li3 = new("li", DisplayType.Block, ClosingType.Normal);
        li3.AddChild(new LightTextNode("Item 3"));

        DomCommandManager commandManager = new();
        commandManager.Execute(new AddChildCommand(ul, li1));
        commandManager.Execute(new AddChildCommand(ul, li2));
        commandManager.Execute(new AddChildCommand(ul, li3));
        commandManager.Execute(new AddClassCommand(li2, "selected"));
        commandManager.Execute(new SetTextCommand(text1, "Updated item 1"));

        ul.SetState(NormalLightNodeState.Instance);
        li3.SetState(HiddenLightNodeState.Instance);
        text2.SetState(ReadOnlyLightNodeState.Instance);

        Console.WriteLine("OuterHTML:");
        Console.WriteLine(ul.OuterHTML());

        Console.WriteLine();
        Console.WriteLine("Depth-first traversal:");
        PrintTraversal(new DepthFirstLightNodeIterator(ul));

        Console.WriteLine();
        Console.WriteLine("Breadth-first traversal:");
        PrintTraversal(new BreadthFirstLightNodeIterator(ul));

        HtmlStatisticsVisitor statistics = new();
        VisitTree(ul, statistics);

        Console.WriteLine();
        Console.WriteLine("Visitor statistics:");
        Console.WriteLine($"Elements: {statistics.ElementCount}");
        Console.WriteLine($"Text nodes: {statistics.TextNodeCount}");
        Console.WriteLine($"Total text length: {statistics.TotalTextLength}");

        Console.WriteLine();
        Console.WriteLine("Command undo/redo:");
        commandManager.TryUndo();
        Console.WriteLine(ul.OuterHTML());
        commandManager.TryRedo();
        Console.WriteLine(ul.OuterHTML());

        Console.WriteLine();
        Console.WriteLine("State protection:");
        try
        {
            text2.SetText("Should fail");
        }
        catch (InvalidOperationException exception)
        {
            Console.WriteLine(exception.Message);
        }

        Console.WriteLine();
        Console.WriteLine("Lifecycle log:");
        foreach (string entry in ul.LifecycleLog)
        {
            Console.WriteLine(entry);
        }
    }

    private static void VisitTree(LightNode root, ILightNodeVisitor visitor)
    {
        foreach (LightNode node in new DepthFirstLightNodeIterator(root))
        {
            node.Accept(visitor);
        }
    }

    private static void PrintTraversal(IEnumerable<LightNode> traversal)
    {
        foreach (LightNode node in traversal)
        {
            Console.WriteLine(DescribeNode(node));
        }
    }

    private static string DescribeNode(LightNode node)
    {
        return node switch
        {
            LightElementNode element => $"<{element.TagName}> [{element.StateKind}]",
            LightTextNode text => $"\"{text.Text}\" [{text.StateKind}]",
            _ => node.GetType().Name
        };
    }

    private sealed class LoggingLightElementNode : LightElementNode
    {
        public List<string> LifecycleLog { get; } = new();

        public LoggingLightElementNode(string tagName, DisplayType display, ClosingType closing)
            : base(tagName, display, closing)
        {
        }

        protected override void OnCreated()
        {
            LifecycleLog.Add($"{TagName}: created");
        }

        protected override void OnInserted(LightElementNode parent)
        {
            LifecycleLog.Add($"{TagName}: inserted into <{parent.TagName}>");
        }

        protected override void OnRemoved(LightElementNode parent)
        {
            LifecycleLog.Add($"{TagName}: removed from <{parent.TagName}>");
        }

        protected override void OnClassListApplied()
        {
            LifecycleLog.Add($"{TagName}: class list applied");
        }

        protected override void OnStylesApplied()
        {
            LifecycleLog.Add($"{TagName}: styles applied");
        }

        protected override void OnChildAdded(LightNode child)
        {
            LifecycleLog.Add($"{TagName}: child added -> {child.GetType().Name}");
        }

        protected override void OnChildRemoved(LightNode child)
        {
            LifecycleLog.Add($"{TagName}: child removed -> {child.GetType().Name}");
        }

        protected override void AfterRender(string html)
        {
            LifecycleLog.Add($"{TagName}: rendered ({html.Length} chars)");
        }
    }
}
