using System;

public abstract class LightNode
{
    // ЦЕ ПАТТЕРН: STATE
    private ILightNodeState _state = NormalLightNodeState.Instance;

    public LightElementNode? Parent { get; private set; }

    public LightNodeStateKind StateKind => _state.Kind;

    protected void InitializeNode()
    {
        // ЦЕ ПАТТЕРН: TEMPLATE METHOD
        OnCreated();
    }

    public string OuterHTML()
    {
        // ЦЕ ПАТТЕРН: TEMPLATE METHOD
        BeforeRender();
        string html = _state.Render(this, RenderOuterHtmlCore);
        AfterRender(html);
        return html;
    }

    public void SetState(ILightNodeState state)
    {
        // ЦЕ ПАТТЕРН: STATE
        _state = state ?? throw new ArgumentNullException(nameof(state));
        OnStateChanged(state);
    }

    public void Accept(ILightNodeVisitor visitor)
    {
        // ЦЕ ПАТТЕРН: VISITOR
        ArgumentNullException.ThrowIfNull(visitor);
        AcceptCore(visitor);
    }

    internal void AttachTo(LightElementNode parent)
    {
        // ЦЕ ПАТТЕРН: TEMPLATE METHOD
        Parent = parent;
        OnInserted(parent);
    }

    internal void DetachFrom(LightElementNode parent)
    {
        // ЦЕ ПАТТЕРН: TEMPLATE METHOD
        Parent = null;
        OnRemoved(parent);
    }

    protected void EnsureCanMutate()
    {
        // ЦЕ ПАТТЕРН: STATE
        _state.EnsureCanMutate(this);
    }

    protected abstract void AcceptCore(ILightNodeVisitor visitor);

    protected abstract string RenderOuterHtmlCore();

    protected virtual void OnCreated()
    {
        // ЦЕ ПАТТЕРН: TEMPLATE METHOD
    }

    protected virtual void OnInserted(LightElementNode parent)
    {
        // ЦЕ ПАТТЕРН: TEMPLATE METHOD
    }

    protected virtual void OnRemoved(LightElementNode parent)
    {
        // ЦЕ ПАТТЕРН: TEMPLATE METHOD
    }

    protected virtual void BeforeRender()
    {
        // ЦЕ ПАТТЕРН: TEMPLATE METHOD
    }

    protected virtual void AfterRender(string html)
    {
        // ЦЕ ПАТТЕРН: TEMPLATE METHOD
    }

    protected virtual void OnStateChanged(ILightNodeState state)
    {
        // ЦЕ ПАТТЕРН: STATE
    }
}
