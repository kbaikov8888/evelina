using evelina.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace evelina.ViewModels;

public class MainViewModel : ReactiveObject
{
    [Reactive]
    public object? ActiveWindow { get; set; }

    private StartViewModel _start;

    internal MainViewModel()
    {
        _start = new StartViewModel();
        _start.SetNewModel += OnSetNewModel;

        ActiveWindow = _start;
    }

    private void OnSetNewModel(object? obj)
    {
        if (obj is IReturnableToStart returnable)
        {
            returnable.ReturnToStart += OnReturnToStart;
        }

        ActiveWindow = obj;
    }

    private void OnReturnToStart()
    {
        ActiveWindow = _start;
    }
}
