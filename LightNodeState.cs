using System;

public enum LightNodeStateKind
{
    Normal,
    Hidden,
    ReadOnly
}

public interface ILightNodeState
{
    // ЦЕ ПАТТЕРН: STATE
    LightNodeStateKind Kind { get; }
    string Render(LightNode node, Func<string> defaultRenderer);
    void EnsureCanMutate(LightNode node);
}

public sealed class NormalLightNodeState : ILightNodeState
{
    // ЦЕ ПАТТЕРН: STATE
    public static NormalLightNodeState Instance { get; } = new();

    public LightNodeStateKind Kind => LightNodeStateKind.Normal;

    private NormalLightNodeState()
    {
    }

    public string Render(LightNode node, Func<string> defaultRenderer)
    {
        return defaultRenderer();
    }

    public void EnsureCanMutate(LightNode node)
    {
    }
}

public sealed class HiddenLightNodeState : ILightNodeState
{
    // ЦЕ ПАТТЕРН: STATE
    public static HiddenLightNodeState Instance { get; } = new();

    public LightNodeStateKind Kind => LightNodeStateKind.Hidden;

    private HiddenLightNodeState()
    {
    }

    public string Render(LightNode node, Func<string> defaultRenderer)
    {
        return string.Empty;
    }

    public void EnsureCanMutate(LightNode node)
    {
    }
}

public sealed class ReadOnlyLightNodeState : ILightNodeState
{
    // ЦЕ ПАТТЕРН: STATE
    public static ReadOnlyLightNodeState Instance { get; } = new();

    public LightNodeStateKind Kind => LightNodeStateKind.ReadOnly;

    private ReadOnlyLightNodeState()
    {
    }

    public string Render(LightNode node, Func<string> defaultRenderer)
    {
        return defaultRenderer();
    }

    public void EnsureCanMutate(LightNode node)
    {
        throw new InvalidOperationException(
            $"Node of type {node.GetType().Name} is read-only in the current state.");
    }
}
