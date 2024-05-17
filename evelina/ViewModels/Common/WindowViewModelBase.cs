using System;

namespace evelina.ViewModels.Common;

/// <summary>
/// VM, способные заменять активный UserControl основного окна
/// </summary>
public class WindowViewModelBase : ViewModelBase
{
    internal event Action? ReturnBackEvent;

    protected MainViewModel Main;

    private readonly WindowViewModelBase? _previous;


    public WindowViewModelBase(MainViewModel main)
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

public interface IMenuCompatible { }