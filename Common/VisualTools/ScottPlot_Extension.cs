using ScottPlot;
using ScottPlot.Colormaps;
using ScottPlot.Plottables;
using System.Collections.Generic;
using PlotWrapper.Models;

namespace VisualTools;

public static class ScottPlot_Extension
{
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

    public static void AddArea(this Plot plot, List<SeriesInfo> series, double[] x, bool cumulative = false, bool normalize = false)
    {
        if (normalize)
        {
            for (int j = 0; j < x.Length; j++)
            {
                double sum = 0;
                for (int i = 0; i < series.Count; i++)
                {
                    sum += series[i].Values[j];
                }

                for (int i = 0; i < series.Count; i++)
                {
                    series[i].Values[j] = series[i].Values[j] / sum * 100;
                }
            }
        }

        if (cumulative && series.Count > 1)
        {
            for (int i = 1; i < series.Count; i++)
            {
                var values = series[i].Values;
                var newValues = new double[x.Length];
                for (int j = 0; j < x.Length; j++)
                {
                    newValues[j] = values[j] + series[i - 1].Values[j];
                }

                series[i].Values = newValues;
            }
        }

        for (int i = 0; i < series.Count; i++)
        {
            double[] bottom;
            if (i == 0)
            {
                bottom = new double[x.Length];
            }
            else
            {
                bottom = series[i - 1].Values;
            }

            var fill = plot.Add.FillY(x, bottom, series[i].Values);
            fill.FillColor = series[i].Color;
            fill.LineColor = series[i].Color;
            fill.LegendText = series[i].Name;
        }
    }

    public static void AddStackedBars(this Plot plot, List<SeriesInfo> series, double[] x)
    {
        var values = new double[series.Count][];

        for (int i = 0; i < series.Count; i++)
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
                newValues[j] = bottom[j] + series[i].Values[j];
            }

            values[i] = newValues;
        }

        for (int i = series.Count - 1; i >= 0; i--)
        {
            var barPlot = plot.Add.Bars(x, values[i]);
            barPlot.Color = series[i].Color;
            barPlot.LegendText = series[i].Name;
            barPlot.SetSize(1000 / x.Length); // magic
            barPlot.SetBorderColor(series[i].Color);
        }
    }
}