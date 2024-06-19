using PlotWrapper.Models;
using System.Collections.Generic;

namespace PlotWrapper.Interfaces;

public interface IPlot
{
    ISeries AddScatter(double[] x, SeriesInfo y);
    ISeries AddBars(double[] x, SeriesInfo y);
    ISeries AddStackedBars(double[] x, List<SeriesInfo> ys);
    ISeries AddArea(double[] x, List<SeriesInfo> ys, bool cumulative = false, bool normalize = false);

    void SetDateTimeX();
}