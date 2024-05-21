using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Windows.Input;

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

public class MainViewModelBase : ReactiveObject, IMainViewModel
{
    private WindowViewModelBase? _activeWindow;
    public WindowViewModelBase? ActiveWindow
    {
        get => _activeWindow;
        set
        {
            this.RaiseAndSetIfChanged(ref _activeWindow, value);
            this.RaisePropertyChanged(nameof(ShowMenu));
        }
    }

    public bool ShowMenu => ActiveWindow is IMenuCompatible;

    [Reactive]
    public bool IsPaneOpen { get; set; }

    public ICommand TriggerPaneCommand { get; }


    protected MainViewModelBase()
    {
        TriggerPaneCommand = ReactiveCommand.Create(TriggerPane);
    }


    private void TriggerPane()
    {
        IsPaneOpen = !IsPaneOpen;
    }
}
