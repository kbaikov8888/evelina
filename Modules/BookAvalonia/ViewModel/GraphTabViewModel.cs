using System;
using ReactiveUI;
using ScottPlot;
using System.Collections.Generic;
using System.Linq;
using VisualTools;

namespace BookAvalonia.ViewModel;

public class GraphTabViewModel : ReactiveObject, IDisposable
{
    public string Name { get; }
    public IReadOnlyList<GraphViewModel> Plots { get; }

    private readonly ScottPlotSyncer _syncer;


    internal GraphTabViewModel(string name, ICollection<Plot> plots)
    {
        Name = name;
        Plots = plots.Select(p => new GraphViewModel(p)).ToList();

        _syncer = new ScottPlotSyncer(Plots.Select(x => x.PlotView), true, false);
    }


    public void Dispose()
    {
        _syncer.Dispose();
    }
}