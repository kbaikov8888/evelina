using Avalonia;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace evelina.Controls;

public class SimpleCheckedViewModel : ReactiveObject
{
    public string? Name => Source.Name;

    [Reactive]
    public bool IsChecked { get; set; }

    public INamed Source { get; }


    public SimpleCheckedViewModel(INamed source)
    {
        Source = source;
    }
}