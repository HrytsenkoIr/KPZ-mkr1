using System;
using System.Collections.Generic;
using System.Linq;

public enum DisplayType { Block, Inline }
public enum ClosingType { SelfClosing, Normal }

public class LightElementNode : LightNode
{
    public string TagName { get; }
    public DisplayType Display { get; }
    public ClosingType Closing { get; }
    public List<string> Classes { get; } = new();
    public Dictionary<string, string> Styles { get; } = new();
    public List<LightNode> Children { get; } = new();

    public LightElementNode(string tagName, DisplayType display, ClosingType closing)
    {
        TagName = tagName;
        Display = display;
        Closing = closing;
        InitializeNode();
    }

    public bool AddClass(string className)
    {
        // ЦЕ ПАТТЕРН: STATE
        EnsureCanMutate();

        if (Classes.Contains(className))
        {
            return false;
        }

        Classes.Add(className);
        OnClassListApplied();
        return true;
    }

    public bool RemoveClass(string className)
    {
        EnsureCanMutate();

        bool removed = Classes.Remove(className);
        if (removed)
        {
            OnClassListApplied();
        }

        return removed;
    }

    public void SetStyle(string property, string value)
    {
        // ЦЕ ПАТТЕРН: STATE
        EnsureCanMutate();
        Styles[property] = value;
        OnStylesApplied();
    }

    public void AddChild(LightNode child)
    {
        // ЦЕ ПАТТЕРН: TEMPLATE METHOD
        InsertChild(Children.Count, child);
    }

    public void InsertChild(int index, LightNode child)
    {
        // ЦЕ ПАТТЕРН: TEMPLATE METHOD
        ArgumentNullException.ThrowIfNull(child);
        EnsureCanMutate();

        if (child.Parent is not null)
        {
            throw new InvalidOperationException("Child node is already attached to another parent.");
        }

        Children.Insert(index, child);
        child.AttachTo(this);
        OnChildAdded(child);
    }

    public bool RemoveChild(LightNode child)
    {
        // ЦЕ ПАТТЕРН: STATE
        ArgumentNullException.ThrowIfNull(child);
        EnsureCanMutate();

        int index = Children.IndexOf(child);
        if (index < 0)
        {
            return false;
        }

        RemoveChildAt(index);
        return true;
    }

    public LightNode RemoveChildAt(int index)
    {
        // ЦЕ ПАТТЕРН: TEMPLATE METHOD
        EnsureCanMutate();
        LightNode child = Children[index];
        Children.RemoveAt(index);
        child.DetachFrom(this);
        OnChildRemoved(child);
        return child;
    }

    public string InnerHTML()
    {
        return string.Join("", Children.Select(c => c.OuterHTML()));
    }

    public void VisitDescendants(ILightNodeVisitor visitor)
    {
        // ЦЕ ПАТТЕРН: VISITOR
        // ЦЕ ПАТТЕРН: ITERATOR
        foreach (LightNode node in new DepthFirstLightNodeIterator(this))
        {
            node.Accept(visitor);
        }
    }

    protected override void AcceptCore(ILightNodeVisitor visitor)
    {
        // ЦЕ ПАТТЕРН: VISITOR
        visitor.VisitElement(this);
    }

    protected override string RenderOuterHtmlCore()
    {
        string classAttr = Classes.Count > 0 ? $" class=\"{string.Join(" ", Classes)}\"" : "";
        string styleAttr = Styles.Count > 0
            ? $" style=\"{string.Join("; ", Styles.Select(pair => $"{pair.Key}: {pair.Value}"))}\""
            : "";

        if (Closing == ClosingType.SelfClosing)
        {
            return $"<{TagName}{classAttr}{styleAttr}/>";
        }

        return $"<{TagName}{classAttr}{styleAttr}>{InnerHTML()}</{TagName}>";
    }

    protected virtual void OnStylesApplied()
    {
        // ЦЕ ПАТТЕРН: TEMPLATE METHOD
    }

    protected virtual void OnClassListApplied()
    {
        // ЦЕ ПАТТЕРН: TEMPLATE METHOD
    }

    protected virtual void OnChildAdded(LightNode child)
    {
        // ЦЕ ПАТТЕРН: TEMPLATE METHOD
    }

    protected virtual void OnChildRemoved(LightNode child)
    {
        // ЦЕ ПАТТЕРН: TEMPLATE METHOD
    }
}
