namespace Patterns.Command;

public abstract class CommandNode
{
    public CommandElementNode? Parent { get; private set; }

    internal void AttachTo(CommandElementNode parent)
    {
        Parent = parent;
    }

    internal void Detach()
    {
        Parent = null;
    }

    public abstract string OuterHtml();
}

public sealed class CommandElementNode : CommandNode
{
    public string TagName { get; }
    public List<string> Classes { get; } = new();
    public List<CommandNode> Children { get; } = new();

    public CommandElementNode(string tagName)
    {
        TagName = tagName;
    }

    public void InsertChild(int index, CommandNode child)
    {
        ArgumentNullException.ThrowIfNull(child);

        if (child.Parent is not null)
        {
            throw new InvalidOperationException("Node already has a parent.");
        }

        Children.Insert(index, child);
        child.AttachTo(this);
    }

    public CommandNode RemoveChildAt(int index)
    {
        CommandNode child = Children[index];
        Children.RemoveAt(index);
        child.Detach();
        return child;
    }

    public bool AddClass(string className)
    {
        if (Classes.Contains(className))
        {
            return false;
        }

        Classes.Add(className);
        return true;
    }

    public bool RemoveClass(string className)
    {
        return Classes.Remove(className);
    }

    public override string OuterHtml()
    {
        string classAttribute = Classes.Count == 0
            ? string.Empty
            : $" class=\"{string.Join(" ", Classes)}\"";

        return $"<{TagName}{classAttribute}>{string.Join("", Children.Select(child => child.OuterHtml()))}</{TagName}>";
    }
}

public sealed class CommandTextNode : CommandNode
{
    public string Text { get; private set; }

    public CommandTextNode(string text)
    {
        Text = text;
    }

    public void SetText(string text)
    {
        Text = text;
    }

    public override string OuterHtml()
    {
        return Text;
    }
}

public interface IDomCommand
{
    string Name { get; }
    void Execute();
    void Undo();
}

public sealed class CommandHistory
{
    private readonly Stack<IDomCommand> _undoStack = new();
    private readonly Stack<IDomCommand> _redoStack = new();

    public void Execute(IDomCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        command.Execute();
        _undoStack.Push(command);
        _redoStack.Clear();
    }

    public bool TryUndo()
    {
        if (_undoStack.Count == 0)
        {
            return false;
        }

        IDomCommand command = _undoStack.Pop();
        command.Undo();
        _redoStack.Push(command);
        return true;
    }

    public bool TryRedo()
    {
        if (_redoStack.Count == 0)
        {
            return false;
        }

        IDomCommand command = _redoStack.Pop();
        command.Execute();
        _undoStack.Push(command);
        return true;
    }
}

public sealed class AddChildCommand : IDomCommand
{
    private readonly CommandElementNode _parent;
    private readonly CommandNode _child;
    private int? _index;

    public string Name => "Add child";

    public AddChildCommand(CommandElementNode parent, CommandNode child)
    {
        _parent = parent;
        _child = child;
    }

    public void Execute()
    {
        int targetIndex = _index ?? _parent.Children.Count;
        _parent.InsertChild(targetIndex, _child);
        _index = targetIndex;
    }

    public void Undo()
    {
        if (_index is not null)
        {
            _parent.RemoveChildAt(_index.Value);
        }
    }
}

public sealed class AddClassCommand : IDomCommand
{
    private readonly CommandElementNode _element;
    private readonly string _className;
    private bool _wasAdded;

    public string Name => "Add class";

    public AddClassCommand(CommandElementNode element, string className)
    {
        _element = element;
        _className = className;
    }

    public void Execute()
    {
        _wasAdded = _element.AddClass(_className);
    }

    public void Undo()
    {
        if (_wasAdded)
        {
            _element.RemoveClass(_className);
        }
    }
}

public sealed class SetTextCommand : IDomCommand
{
    private readonly CommandTextNode _node;
    private readonly string _newText;
    private string? _previousText;

    public string Name => "Set text";

    public SetTextCommand(CommandTextNode node, string newText)
    {
        _node = node;
        _newText = newText;
    }

    public void Execute()
    {
        _previousText = _node.Text;
        _node.SetText(_newText);
    }

    public void Undo()
    {
        if (_previousText is not null)
        {
            _node.SetText(_previousText);
        }
    }
}

public static class CommandDemo
{
    public static string BuildSampleHtml()
    {
        CommandElementNode list = new("ul");
        CommandTextNode firstText = new("Item 1");
        CommandHistory history = new();

        CommandElementNode firstItem = new("li");
        firstItem.InsertChild(0, firstText);

        CommandElementNode secondItem = new("li");
        secondItem.InsertChild(0, new CommandTextNode("Item 2"));

        history.Execute(new AddChildCommand(list, firstItem));
        history.Execute(new AddChildCommand(list, secondItem));
        history.Execute(new AddClassCommand(list, "my-list"));
        history.Execute(new SetTextCommand(firstText, "Updated item 1"));

        history.TryUndo();
        history.TryRedo();

        return list.OuterHtml();
    }
}
