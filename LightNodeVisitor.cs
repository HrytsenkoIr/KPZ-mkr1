public interface ILightNodeVisitor
{
    // ЦЕ ПАТТЕРН: VISITOR
    void VisitElement(LightElementNode node);
    void VisitText(LightTextNode node);
}

public sealed class HtmlStatisticsVisitor : ILightNodeVisitor
{
    // ЦЕ ПАТТЕРН: VISITOR
    public int ElementCount { get; private set; }
    public int TextNodeCount { get; private set; }
    public int TotalTextLength { get; private set; }

    public void VisitElement(LightElementNode node)
    {
        ElementCount++;
    }

    public void VisitText(LightTextNode node)
    {
        TextNodeCount++;
        TotalTextLength += node.Text.Length;
    }
}
