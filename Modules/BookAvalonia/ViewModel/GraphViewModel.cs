using BookImpl.Enum;
using ReactiveUI;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Plottables;

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

public static class ScottPlotExtension
{
    public static Color GetScottPlotColor(this EntryType type) => Color.FromARGB(type.GetColor().ToUInt32());

    public static void SetSize(this BarPlot plot, double size)
    {
        foreach (var bar in plot.Bars)
        {
            bar.Size = size;
        }
    }
    public static void SetBorderColor(this BarPlot plot, Color color)
    {
        foreach (var bar in plot.Bars)
        {
            bar.BorderColor = color;
        }
    }
}
