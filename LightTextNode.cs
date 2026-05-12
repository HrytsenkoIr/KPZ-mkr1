public class LightTextNode : LightNode
{
    public string Text { get; private set; }

    public LightTextNode(string text)
    {
        Text = text;
        InitializeNode();
    }

    public void SetText(string text)
    {
        // ЦЕ ПАТТЕРН: STATE
        // ЦЕ ПАТТЕРН: TEMPLATE METHOD
        EnsureCanMutate();
        Text = text;
        OnTextRendered();
    }

    protected override void AcceptCore(ILightNodeVisitor visitor)
    {
        // ЦЕ ПАТТЕРН: VISITOR
        visitor.VisitText(this);
    }

    protected override string RenderOuterHtmlCore()
    {
        // ЦЕ ПАТТЕРН: TEMPLATE METHOD
        OnTextRendered();
        return Text;
    }

    protected virtual void OnTextRendered()
    {
        // ЦЕ ПАТТЕРН: TEMPLATE METHOD
    }
}
