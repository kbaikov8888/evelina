using BookImpl;
using BookImpl.Enum;
using DynamicData;
using evelina.Controls;
using ScottPlot;
using System.Collections.Generic;
using System.Linq;

namespace BookAvalonia.ViewModel;

public class GraphPanelViewModel : WindowViewModelBase, IMenuCompatible
{
    public GraphTabViewModel Total { get; }

    private readonly Book _book;


    internal GraphPanelViewModel(Book book, IMainViewModel main) : base(main)
    {
        _book = book;

        Total = CreateTotalTab(_book.CalculatedData.Months);
    }


    private GraphTabViewModel CreateTotalTab(BookDatedData data)
    {
        var plots = new List<Plot>();

        var months = data.Dates;
        var doubleMonths = months.Select(x => x.ToOADate()).ToArray();

        plots.Add(GetCumulativeResultsPlot());
        plots.Add(GetBarPlot(EntryType.Expense, data.Expenses));
        plots.Add(GetBarPlot(EntryType.Income, data.Incomes));
        plots.Add(GetBarPlot(EntryType.Invest, data.Invests));
        plots.Add(GetBarPlot(EntryType.ReInvest, data.ReInvests));

        return new GraphTabViewModel("Total", plots);

        Plot GetCumulativeResultsPlot()
        {
            var res = new double[months.Length];
            for (int i = 0; i < res.Length; i++)
            {
                if (i == 0)
                {
                    res[i] = 0;
                }
                else
                {
                    res[i] = res[i - 1];
                }

                res[i] += _book.CalculatedData.Months.Results[i];
            }

            var plot = new Plot();
            plot.Add.Scatter(months, res);
            plot.Axes.DateTimeTicksBottom();

            return plot;
        }

        Plot GetBarPlot(EntryType type, double[] values)
        {
            var plot = new Plot();
            var barPlot = plot.Add.Bars(doubleMonths, values);
            barPlot.Color = type.GetScottPlotColor();
            barPlot.SetSize(20);
            barPlot.SetBorderColor(type.GetScottPlotColor());
            plot.Axes.DateTimeTicksBottom();

            return plot;
        }
    }
}