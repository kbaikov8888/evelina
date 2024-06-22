using Avalonia.Controls;
using PlotWrapper.Models;
using System.Collections.Generic;

namespace PlotWrapper.Interfaces;

public interface IPlot
{
    Control View { get; }

    void AddScatter(double[] x, SeriesInfo y);
    void AddBars(double[] x, SeriesInfo y);
    void AddStackedBars(double[] x, List<SeriesInfo> ys);
    void AddArea(double[] x, List<SeriesInfo> ys, bool cumulative = false, bool normalize = false);

    void SetDateTimeX();
}
