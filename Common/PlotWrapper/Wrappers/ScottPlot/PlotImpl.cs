using PlotWrapper.Interfaces;
using PlotWrapper.Models;
using ScottPlot;
using System.Collections.Generic;

namespace PlotWrapper.Wrappers.ScottPlot;

internal class PlotImpl : IPlot
{
    private readonly Plot _plot;


    public PlotImpl()
    {
        _plot = new Plot();
    }


    public void AddScatter(double[] x, SeriesInfo y)
    {
        var series = _plot.Add.Scatter(x, y.Values, y.Color.GetScottPlotColor());
        series.LegendText = y.Name;
    }

    public void AddBars(double[] x, SeriesInfo y)
    {
        var series = _plot.Add.Bars(x, y.Values);
        series.Color = y.Color.GetScottPlotColor();
        series.LegendText = y.Name;
        series.SetSize(1000 / y.Values.Length); // magic
        series.SetBorderColor(y.Color.GetScottPlotColor());
    }

    public void AddStackedBars(double[] x, List<SeriesInfo> ys)
    {
        var values = new double[ys.Count][];

        for (int i = 0; i < ys.Count; i++)
        {
            double[] bottom;
            if (i == 0)
            {
                bottom = new double[x.Length];
            }
            else
            {
                bottom = values[i - 1];
            }

            var newValues = new double[x.Length];
            for (int j = 0; j < x.Length; j++)
            {
                newValues[j] = bottom[j] + ys[i].Values[j];
            }

            values[i] = newValues;
        }

        for (int i = ys.Count - 1; i >= 0; i--)
        {
            var barPlot = _plot.Add.Bars(x, values[i]);
            barPlot.Color = ys[i].Color.GetScottPlotColor();
            barPlot.LegendText = ys[i].Name;
            barPlot.SetSize(1000 / x.Length); // magic
            barPlot.SetBorderColor(ys[i].Color.GetScottPlotColor());
        }
    }

    public void AddArea(double[] x, List<SeriesInfo> ys, bool cumulative = false, bool normalize = false)
    {
        if (normalize)
        {
            for (int j = 0; j < x.Length; j++)
            {
                double sum = 0;
                for (int i = 0; i < ys.Count; i++)
                {
                    sum += ys[i].Values[j];
                }

                for (int i = 0; i < ys.Count; i++)
                {
                    ys[i].Values[j] = ys[i].Values[j] / sum * 100;
                }
            }
        }

        if (cumulative && ys.Count > 1)
        {
            for (int i = 1; i < ys.Count; i++)
            {
                var values = ys[i].Values;
                var newValues = new double[x.Length];
                for (int j = 0; j < x.Length; j++)
                {
                    newValues[j] = values[j] + ys[i - 1].Values[j];
                }

                ys[i].Values = newValues;
            }
        }

        for (int i = 0; i < ys.Count; i++)
        {
            double[] bottom;
            if (i == 0)
            {
                bottom = new double[x.Length];
            }
            else
            {
                bottom = ys[i - 1].Values;
            }

            var fill = _plot.Add.FillY(x, bottom, ys[i].Values);
            fill.FillColor = ys[i].Color.GetScottPlotColor();
            fill.LineColor = ys[i].Color.GetScottPlotColor();
            fill.LegendText = ys[i].Name;
        }
    }

    public void SetDateTimeX()
    {
        _plot.Axes.DateTimeTicksBottom();
    }
}