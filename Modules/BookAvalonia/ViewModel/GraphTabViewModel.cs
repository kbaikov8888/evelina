using ReactiveUI;
using ScottPlot;
using System.Collections.Generic;
using System.Linq;

namespace BookAvalonia.ViewModel;

public class GraphTabViewModel : ReactiveObject
{
    public string Name { get; }
    public IReadOnlyList<GraphViewModel> Plots { get; }


    internal GraphTabViewModel(string name, IEnumerable<Plot> plots)
    {
        Name = name;
        Plots = plots.Select(p => new GraphViewModel(p)).ToList();
    }
}