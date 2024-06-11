using System;
using Avalonia;
using Avalonia.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Windows.Input;

namespace evelina.Controls.SimpleCheckedList;

public class SimpleCheckedViewModel : ReactiveObject
{
    public event Action<SimpleCheckedViewModel>? DoubleClickedEvent;

    public ICommand DoubleClickCommand { get; }

    public string? Name => Source.Name;

    [Reactive]
    public bool IsChecked { get; set; } = true;

    [Reactive]
    public bool IsEnabled { get; set; } = true;

    public INamed Source { get; }


    public SimpleCheckedViewModel(INamed source)
    {
        Source = source;

        DoubleClickCommand = ReactiveCommand.Create<PointerPressedEventArgs>(DoubleClick);
    }


    private void DoubleClick(PointerPressedEventArgs args)
    {
        if (args.ClickCount == 2)
        {
            DoubleClickedEvent?.Invoke(this);
        }
    }
}

public class SimpleCheckedViewModelComparer : IComparer<SimpleCheckedViewModel>
{
    public int Compare(SimpleCheckedViewModel x, SimpleCheckedViewModel y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (ReferenceEquals(null, y)) return 1;
        if (ReferenceEquals(null, x)) return -1;
        return x.Name.CompareTo(y.Name);
    }
}