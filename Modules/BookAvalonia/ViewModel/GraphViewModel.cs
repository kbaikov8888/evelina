using ReactiveUI;
using ScottPlot;
using ScottPlot.Avalonia;

namespace BookAvalonia.ViewModel;

public class GraphViewModel : ReactiveObject
{
    // https://github.com/ScottPlot/ScottPlot/issues/494#issuecomment-665236518
    public AvaPlot PlotView { get; }

    internal GraphViewModel(Plot plot)
    {
        PlotView = new AvaPlot();
        PlotView.Reset(plot);
        PlotView.Refresh();
    }
}