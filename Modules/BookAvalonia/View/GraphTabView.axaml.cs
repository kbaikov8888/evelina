using Avalonia.Input;
using Avalonia.ReactiveUI;
using BookAvalonia.ViewModel;

namespace BookAvalonia.View;

public partial class GraphTabView : ReactiveUserControl<GraphTabViewModel>
{
    public GraphTabView()
    {
        InitializeComponent();
    }

    private void InputElement_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        e.Handled= true;
    }
}