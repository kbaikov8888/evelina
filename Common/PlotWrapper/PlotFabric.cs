using PlotWrapper.Interfaces;
using ScottPlot.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlotWrapper;

public static class PlotFabric
{
    public static IPlot CreatePlot()
    {
        return new Wrappers.ScottPlot.PlotImpl();
    }

    public static IDisposable SyncPlots(IEnumerable<IPlot> plots, bool syncX, bool syncY)
    {
        var views = plots.Select(x => x.View).Cast<AvaPlot>();

        return new Wrappers.ScottPlot.ScottPlotSyncer(views, syncX, syncY);
    }
}
