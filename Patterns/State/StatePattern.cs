namespace Patterns.State;

public enum LightNodeStateKind
{
    Normal,
    Hidden,
    ReadOnly
}

public interface ILightNodeState
{
    LightNodeStateKind Kind { get; }
    string Render(StateNode node, Func<string> defaultRenderer);
    void EnsureCanMutate(StateNode node);
}

public abstract class StateNode
{
    private ILightNodeState _state = NormalLightNodeState.Instance;

    public LightNodeStateKind StateKind => _state.Kind;

    public void SetState(ILightNodeState state)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));
    }

    public string Render()
    {
        return _state.Render(this, RenderCore);
    }

    protected void EnsureCanMutate()
    {
        _state.EnsureCanMutate(this);
    }

    protected abstract string RenderCore();
}

public sealed class StateElementNode : StateNode
{
    public string TagName { get; }
    public List<StateNode> Children { get; } = new();

    public StateElementNode(string tagName)
    {
        TagName = tagName;
    }

    public void AddChild(StateNode child)
    {
        ArgumentNullException.ThrowIfNull(child);
        EnsureCanMutate();
        Children.Add(child);
    }

    protected override string RenderCore()
    {
        return $"<{TagName}>{string.Join("", Children.Select(child => child.Render()))}</{TagName}>";
    }
}

public sealed class StateTextNode : StateNode
{
    public string Text { get; private set; }

    public StateTextNode(string text)
    {
        Text = text;
    }

    public void SetText(string text)
    {
        EnsureCanMutate();
        Text = text;
    }

    protected override string RenderCore()
    {
        return Text;
    }
}

public sealed class NormalLightNodeState : ILightNodeState
{
    public static NormalLightNodeState Instance { get; } = new();

    public LightNodeStateKind Kind => LightNodeStateKind.Normal;

    private NormalLightNodeState()
    {
    }

    public string Render(StateNode node, Func<string> defaultRenderer)
    {
        return defaultRenderer();
    }

    public void EnsureCanMutate(StateNode node)
    {
    }
}

public sealed class HiddenLightNodeState : ILightNodeState
{
    public static HiddenLightNodeState Instance { get; } = new();

    public LightNodeStateKind Kind => LightNodeStateKind.Hidden;

    private HiddenLightNodeState()
    {
    }

    public string Render(StateNode node, Func<string> defaultRenderer)
    {
        return string.Empty;
    }

    public void EnsureCanMutate(StateNode node)
    {
    }
}

public sealed class ReadOnlyLightNodeState : ILightNodeState
{
    public static ReadOnlyLightNodeState Instance { get; } = new();

    public LightNodeStateKind Kind => LightNodeStateKind.ReadOnly;

    private ReadOnlyLightNodeState()
    {
    }

    public string Render(StateNode node, Func<string> defaultRenderer)
    {
        return defaultRenderer();
    }

    public void EnsureCanMutate(StateNode node)
    {
        throw new InvalidOperationException($"{node.GetType().Name} is read-only.");
    }
}

public static class StateDemo
{
    public static (string HiddenHtml, string ErrorMessage) Run()
    {
        StateElementNode list = new("ul");

        StateElementNode firstItem = new("li");
        firstItem.AddChild(new StateTextNode("Item 1"));

        StateElementNode secondItem = new("li");
        StateTextNode protectedText = new("Item 2");
        secondItem.AddChild(protectedText);

        list.AddChild(firstItem);
        list.AddChild(secondItem);

        secondItem.SetState(HiddenLightNodeState.Instance);
        protectedText.SetState(ReadOnlyLightNodeState.Instance);

        string hiddenHtml = list.Render();

        try
        {
            protectedText.SetText("Changed");
            return (hiddenHtml, string.Empty);
        }
        catch (InvalidOperationException exception)
        {
            return (hiddenHtml, exception.Message);
        }
    }
}
