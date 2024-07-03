using ScottPlot;
using ScottPlot.Plottables;

namespace PlotWrapper.Wrappers.ScottPlot;

internal static class ScottPlot_Extension
{
    public static Color GetScottPlotColor(this Avalonia.Media.Color color) => Color.FromARGB(color.ToUInt32());

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