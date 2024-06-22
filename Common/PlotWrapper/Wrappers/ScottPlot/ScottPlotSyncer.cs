using Avalonia.Threading;
using ScottPlot;
using ScottPlot.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlotWrapper.Wrappers.ScottPlot;

internal class ScottPlotSyncer : IDisposable
{
    private readonly DispatcherTimer _updateAxisLimits = new() { Interval = new TimeSpan(0, 0, 0, 0, 5) };
    private readonly HashSet<AvaPlot> _plots = new();
    private readonly bool _syncX;
    private readonly bool _syncY;

    private Plot? _lastChanged;
    private bool _inChanging = false;


    public ScottPlotSyncer(IEnumerable<AvaPlot> plots, bool syncX, bool syncY)
    {
        if (!syncX && !syncY)
        {
            throw new Exception("Nothing to sync!");
        }

        _plots = plots.ToHashSet();
        _syncX = syncX;
        _syncY = syncY;

        foreach (var plot in _plots)
        {
            plot.Plot.RenderManager.AxisLimitsChanged += AxisLimitsChanged;
        }

        _updateAxisLimits.Tick += UpdateAxisLimitsOnTick;
    }


    public void Dispose()
    {
        _updateAxisLimits.Stop();
        _updateAxisLimits.Tick -= UpdateAxisLimitsOnTick;

        foreach (var plot in _plots)
        {
            plot.Plot.RenderManager.AxisLimitsChanged -= AxisLimitsChanged;
        }

        _plots.Clear();
    }

    private void UpdateAxisLimitsOnTick(object? sender, EventArgs e)
    {
        _updateAxisLimits.Stop();

        if (_lastChanged is null)
        {
            return;
        }

        _inChanging = true;

        var limits = _lastChanged.Axes.GetLimits();

        foreach (var plot in _plots.Where(plot => plot.Plot != _lastChanged))
        {
            if (_syncX)
            {
                plot.Plot.Axes.SetLimitsX(limits);
            }

            if (_syncY)
            {
                plot.Plot.Axes.SetLimitsY(limits);
            }

            plot.Refresh();
        }

        _inChanging = false;
    }

    private void AxisLimitsChanged(object? sender, RenderDetails e)
    {
        if (_inChanging || sender is not Plot plot)
        {
            return;
        }

        _lastChanged = plot;

        _updateAxisLimits.Start();
    }
}