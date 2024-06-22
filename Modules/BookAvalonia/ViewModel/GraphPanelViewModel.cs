using Avalonia.Media;
using BookImpl;
using BookImpl.Elements;
using BookImpl.Enum;
using DynamicData;
using evelina.Controls;
using PlotWrapper;
using PlotWrapper.Interfaces;
using PlotWrapper.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Tools;
using VisualTools;

namespace BookAvalonia.ViewModel;

public class GraphPanelViewModel : WindowViewModelBase, IMenuCompatible
{
    public IEnumerable<DateLevel> DateLevels => Enum.GetValues(typeof(DateLevel)).Cast<DateLevel>();

    private DateLevel _selectedDateLevel = DateLevel.Month;
    public DateLevel SelectedDateLevel
    {
        get => _selectedDateLevel;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedDateLevel, value);
            RefreshTabs();
        }
    }

    [Reactive]
    public GraphTabViewModel? Total { get; set; }

    [Reactive]
    public GraphTabViewModel? Invests { get; set; }

    [Reactive]
    public GraphTabViewModel? Categories { get; set; }

    private readonly Book _book;


    internal GraphPanelViewModel(Book book, IMainViewModel main) : base(main)
    {
        _book = book;

        RefreshTabs();
    }

    private void RefreshTabs()
    {
        var data = _book.CalculatedData.GetData(SelectedDateLevel);

        Total?.Dispose();
        Total = CreateTotalTab(data);

        Invests?.Dispose();
        Invests = CreateInvestsTab(data);

        Categories?.Dispose();
        Categories = CreateCategoriesTab(data);
    }

    private GraphTabViewModel CreateTotalTab(BookDatedData data)
    {
        var plots = new List<IPlot>();

        var dateDoubles = data.Dates.Select(x => x.ToOADate()).ToArray();

        plots.Add(GetCumulativeResultsPlot());
        plots.Add(GetBarPlot(EntryType.Expense, data.Expenses));
        plots.Add(GetBarPlot(EntryType.Income, data.Incomes));
        plots.Add(GetBarPlot(EntryType.Invest, data.Invests));
        plots.Add(GetBarPlot(EntryType.ReInvest, data.ReInvests));

        return new GraphTabViewModel("Total", plots);

        IPlot GetCumulativeResultsPlot()
        {
            var res = new double[data.Dates.Length];
            var resWithInvest = new double[data.Dates.Length];

            for (int i = 0; i < res.Length; i++)
            {
                if (i == 0)
                {
                    res[i] = 0;
                    resWithInvest[i] = 0;
                }
                else
                {
                    res[i] = res[i - 1];
                    resWithInvest[i] = resWithInvest[i - 1];
                }

                res[i] += data.Results[i];
                resWithInvest[i] += data.Incomes[i] - data.Expenses[i];
            }

            var plot = PlotFabric.CreatePlot();

            plot.AddScatter(data.DatesAsDoubles, new SeriesInfo("Res", Colors.Blue, res));
            //total.FillY = true;
            //total.FillYColor = total.Color.WithAlpha(.2);

            plot.AddScatter(data.DatesAsDoubles, new SeriesInfo("ResWithInvest", EntryType.Invest.GetColor(), resWithInvest));
            //withInvest.FillY = true;
            //withInvest.FillYColor = w0ithInvest.Color.WithAlpha(.2);

            plot.SetDateTimeX();

            return plot;
        }

        IPlot GetBarPlot(EntryType type, double[] values)
        {
            var plot = PlotFabric.CreatePlot();

            var info = new SeriesInfo()
            {
                Color = type.GetColor(),
                Name = type.ToString(),
                Values = values
            };

            plot.AddBars(dateDoubles, info);
            plot.SetDateTimeX();

            return plot;
        }
    }

    private GraphTabViewModel CreateInvestsTab(BookDatedData data)
    {
        var plots = new List<IPlot>
        {
            GetAccountsArea(false),
            GetAccountsArea(true)
        };

        return new GraphTabViewModel("Invests", plots);

        IPlot GetAccountsArea(bool normalize)
        {
            var plot = PlotFabric.CreatePlot();

            var series = new List<SeriesInfo>();

            int counter = 0;
            foreach (var (family, values) in data.InvestsByFamilies)
            {
                var info = new SeriesInfo()
                {
                    Name = family.Name,
                    Color = PrettyColors.Get(counter++),
                    Values = values.CumulativeSum().ToArray()
                };

                series.Add(info);
            }

            plot.AddArea(data.DatesAsDoubles, series, true, normalize);
            plot.SetDateTimeX();

            return plot;
        }
    }
    
    private GraphTabViewModel CreateCategoriesTab(BookDatedData data)
    {
        var plots = new List<IPlot>
        {
            GetCategorical(data.ParentIncomeCategories, data.DatesAsDoubles),
            GetCategorical(data.ParentExpenseCategories, data.DatesAsDoubles)
        };

        return new GraphTabViewModel("Categories", plots);
    }

    public static IPlot GetCategorical<T>(Dictionary<T, double[]> dict, double[] x) where T : Category
    {
        var plot = PlotFabric.CreatePlot();

        var series = new List<SeriesInfo>();

        int counter = 0;
        foreach (var (category, values) in dict)
        {
            var info = new SeriesInfo()
            {
                Name = category.Name,
                Color = PrettyColors.Get(counter++),
                Values = values
            };

            series.Add(info);
        }

        plot.AddStackedBars(x, series);
        plot.SetDateTimeX();

        return plot;
    }
}
