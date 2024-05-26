using System.Collections.Generic;
using BookImpl;
using evelina.Controls;
using ScottPlot;

namespace BookAvalonia.ViewModel;

public class GraphPanelViewModel : WindowViewModelBase, IMenuCompatible
{
    public GraphTabViewModel Total { get; }


    internal GraphPanelViewModel(Book book, IMainViewModel main) : base(main)
    {
        Total = CreateTotalTab();
    }


    private GraphTabViewModel CreateTotalTab()
    {
        var plots = new List<Plot>();

        double[] dataX = { 1, 2, 3, 4, 5 };
        double[] dataY = { 1, 4, 9, 16, 25 };

        var total = new Plot();
        total.Add.Scatter(dataX, dataY);
        plots.Add(total);

        return new GraphTabViewModel("Total", plots);
    }
}