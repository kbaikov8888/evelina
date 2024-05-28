using BookAvalonia.Model;
using BookImpl.Enum;
using ScottPlot;
using ScottPlot.Plottables;
using System.Collections.Generic;

namespace BookAvalonia;

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

    public static void AddArea(this Plot plot, List<(double[], SeriesInfo)> series, double[] x, bool cumulative)
    {
        if (cumulative && series.Count > 1)
        {
            for (int i = 1; i < series.Count; i++)
            {
                double[] values = series[i].Item1;
                double[] newValues = new double[values.Length];
                for (int j = 0; j < values.Length; j++)
                {
                    newValues[j] = values[j] + series[i - 1].Item1[j];
                }
            }
        }

        for (int i = 0; i < series.Count; i++)
        {
            double[] values = series[i].Item1;
            SeriesInfo seriesInfo = series[i].Item2;

            double[] bottom;
            if (i == 0)
            {
                bottom = new double[x.Length];
            }
            else
            {
                bottom = series[i - 1].Item1;
            }

            var fill = plot.Add.FillY(x, bottom, values);
            fill.FillColor = seriesInfo.Color;
            fill.LineColor = seriesInfo.Color;
            fill.LegendText = seriesInfo.Name;
        }
    }
}