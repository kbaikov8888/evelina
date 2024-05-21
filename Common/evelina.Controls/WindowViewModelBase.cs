using System;
using ReactiveUI;

namespace evelina.Controls;

public interface IMainViewModel
{
    WindowViewModelBase? ActiveWindow { get; set; }
}

public interface IReturnableToStart
{
    event Action? ReturnToStart;
}

public interface IMenuCompatible { }

/// <summary>
/// VM, способные заменять активный UserControl основного окна
/// </summary>
public class WindowViewModelBase : ReactiveObject
{
    internal event Action? ReturnBackEvent;

    public IMainViewModel Main { get; }

    private readonly WindowViewModelBase? _previous;


    public WindowViewModelBase(IMainViewModel main)
    {
        Main = main;
        _previous = Main.ActiveWindow;
    }


    protected void TurnBack()
    {
        if (ReturnBackEvent != null)
        {
            // if needs not previous, it may be custom
            ReturnBackEvent.Invoke();
        }
        else
        {
            Main.ActiveWindow = _previous;
        }
    }
}
