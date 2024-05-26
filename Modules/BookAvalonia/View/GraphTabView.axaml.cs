using Avalonia.ReactiveUI;
using BookAvalonia.ViewModel;

namespace BookAvalonia.View;

public partial class GraphTabView : ReactiveUserControl<GraphTabViewModel>
{
    public GraphTabView()
    {
        InitializeComponent();
    }
}