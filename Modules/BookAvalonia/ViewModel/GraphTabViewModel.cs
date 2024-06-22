using PlotWrapper;
using PlotWrapper.Interfaces;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookAvalonia.ViewModel;

public class GraphTabViewModel : ReactiveObject, IDisposable
{
    public string Name { get; }
    public IReadOnlyList<IPlot> Plots { get; }

    private readonly IDisposable _syncer;


    internal GraphTabViewModel(string name, ICollection<IPlot> plots)
    {
        Name = name;
        Plots = plots.ToList();

        _syncer = PlotFabric.SyncPlots(plots, true, false);
    }


    public void Dispose()
    {
        _syncer.Dispose();
    }
}