using System;
using System.Collections.Generic;

public interface IDomCommand
{
    // ЦЕ ПАТТЕРН: COMMAND
    string Name { get; }
    void Execute();
    void Undo();
}

public sealed class DomCommandManager
{
    // ЦЕ ПАТТЕРН: COMMAND
    private readonly Stack<IDomCommand> _undoStack = new();
    private readonly Stack<IDomCommand> _redoStack = new();

    public void Execute(IDomCommand command)
    {
        // Виконується дія і кладеться в історію, якщо після цього була нова дія, стара історія повторного відтворення вже не потрібна
        ArgumentNullException.ThrowIfNull(command);
        command.Execute();
        _undoStack.Push(command);
        _redoStack.Clear();
    }

    public bool TryUndo()
    {
        // Якщо історія порожня, нічого скасовувати
        if (_undoStack.Count == 0)
        {
            return false;
        }
        // Беремо останню дію, скасовуємо її і переносимо в стек для повтору
        IDomCommand command = _undoStack.Pop();
        command.Undo();
        _redoStack.Push(command);
        return true;
    }

    public bool TryRedo()
    {
        // Якщо немає що повторювати, повертаємо false
        if (_redoStack.Count == 0)
        {
            return false;
        }
        // Повторно виконуємо останню скасовану дію і знову повертаємо її в основну історію
        IDomCommand command = _redoStack.Pop();
        command.Execute();
        _undoStack.Push(command);
        return true;
    }
}

public sealed class AddChildCommand : IDomCommand
{
    // ЦЕ ПАТТЕРН: COMMAND
    private readonly LightElementNode _parent;
    private readonly LightNode _child;
    private int? _index;

    public string Name => "Add child";

    public AddChildCommand(LightElementNode parent, LightNode child)
    {
        _parent = parent;
        _child = child;
    }

    public void Execute()
    {
        // Запам'ятовуємо місце вставки, щоб відкотити дію і потім при потребі повторити її
        int targetIndex = _index ?? _parent.Children.Count;
        _parent.InsertChild(targetIndex, _child);
        _index = targetIndex;
    }

    public void Undo()
    {
        if (_index is null)
        {
            return;
        }

        _parent.RemoveChildAt(_index.Value);
    }
}

public sealed class AddClassCommand : IDomCommand
{
    // ЦЕ ПАТТЕРН: COMMAND
    private readonly LightElementNode _element;
    private readonly string _className;
    private bool _wasAdded;

    public string Name => "Add class";

    public AddClassCommand(LightElementNode element, string className)
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
    // ЦЕ ПАТТЕРН: COMMAND
    private readonly LightTextNode _node;
    private readonly string _newText;
    private string? _previousText;

    public string Name => "Set text";

    public SetTextCommand(LightTextNode node, string newText)
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
