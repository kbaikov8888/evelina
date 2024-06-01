using ScottPlot;
using ScottPlot.Plottables;
using System.Collections.Generic;

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

    public static void AddArea(this Plot plot, List<SeriesInfo> series, double[] x, bool cumulative)
    {
        if (cumulative && series.Count > 1)
        {
            for (int i = 1; i < series.Count; i++)
            {
                double[] values = series[i].Values;
                double[] newValues = new double[values.Length];
                for (int j = 0; j < values.Length; j++)
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
}