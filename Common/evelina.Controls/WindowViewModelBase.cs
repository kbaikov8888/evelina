using System;

namespace evelina.ViewModels.Common;

public interface IMainViewModel
{
    WindowViewModelBase? ActiveWindow { get; set; }
}

public interface IMenuCompatible { }

/// <summary>
/// VM, способные заменять активный UserControl основного окна
/// </summary>
public class WindowViewModelBase : ViewModelBase
{
    internal event Action? ReturnBackEvent;

    protected IMainViewModel Main;

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
