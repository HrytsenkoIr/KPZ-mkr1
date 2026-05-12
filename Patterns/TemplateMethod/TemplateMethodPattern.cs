namespace Patterns.TemplateMethod;

public abstract class TemplateNode
{
    protected TemplateNode()
    {
        OnCreated();
    }

    public string Render()
    {
        BeforeRender();
        string html = RenderCore();
        AfterRender(html);
        return html;
    }

    protected abstract string RenderCore();

    protected virtual void OnCreated()
    {
    }

    protected virtual void BeforeRender()
    {
    }

    protected virtual void AfterRender(string html)
    {
    }
}

public sealed class TemplateTextNode : TemplateNode
{
    public string Text { get; }

    public TemplateTextNode(string text)
    {
        Text = text;
    }

    protected override string RenderCore()
    {
        return Text;
    }
}

public class TemplateElementNode : TemplateNode
{
    public string TagName { get; }
    public List<TemplateNode> Children { get; } = new();

    public TemplateElementNode(string tagName)
    {
        TagName = tagName;
    }

    public void AddChild(TemplateNode child)
    {
        ArgumentNullException.ThrowIfNull(child);
        Children.Add(child);
        OnChildAdded(child);
    }

    protected override string RenderCore()
    {
        return $"<{TagName}>{string.Join("", Children.Select(child => child.Render()))}</{TagName}>";
    }

    protected virtual void OnChildAdded(TemplateNode child)
    {
    }
}

public sealed class LoggingTemplateElementNode : TemplateElementNode
{
    public List<string> LifecycleLog { get; } = new();

    public LoggingTemplateElementNode(string tagName)
        : base(tagName)
    {
    }

    protected override void OnCreated()
    {
        LifecycleLog.Add($"{TagName}: created");
    }

    protected override void BeforeRender()
    {
        LifecycleLog.Add($"{TagName}: before render");
    }

    protected override void AfterRender(string html)
    {
        LifecycleLog.Add($"{TagName}: after render ({html.Length} chars)");
    }

    protected override void OnChildAdded(TemplateNode child)
    {
        LifecycleLog.Add($"{TagName}: child added -> {child.GetType().Name}");
    }
}

public static class TemplateMethodDemo
{
    public static IReadOnlyList<string> Run()
    {
        LoggingTemplateElementNode list = new("ul");

        TemplateElementNode firstItem = new("li");
        firstItem.AddChild(new TemplateTextNode("Item 1"));

        TemplateElementNode secondItem = new("li");
        secondItem.AddChild(new TemplateTextNode("Item 2"));

        list.AddChild(firstItem);
        list.AddChild(secondItem);
        _ = list.Render();

        return list.LifecycleLog;
    }
}
