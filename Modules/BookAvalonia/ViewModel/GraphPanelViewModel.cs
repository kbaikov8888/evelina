﻿using BookAvalonia.Model;
using BookImpl;
using BookImpl.Enum;
using DynamicData;
using evelina.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    public GraphTabViewModel Total { get; set; }

    [Reactive]
    public GraphTabViewModel Invests { get; set; }

    private readonly Book _book;


    internal GraphPanelViewModel(Book book, IMainViewModel main) : base(main)
    {
        _book = book;

        RefreshTabs();
    }

    private void RefreshTabs()
    {
        var data = SelectedDateLevel switch
        {
            DateLevel.Year => _book.CalculatedData.Years,
            DateLevel.Month => _book.CalculatedData.Months,
            _ => throw new NotImplementedException(nameof(DateLevel))
        };

        Total = CreateTotalTab(data);
        Invests = CreateInvestsTab(data);
    }

    private GraphTabViewModel CreateTotalTab(BookDatedData data)
    {
        var plots = new List<Plot>();

        var dateDoubles = data.Dates.Select(x => x.ToOADate()).ToArray();

        plots.Add(GetCumulativeResultsPlot());
        plots.Add(GetBarPlot(EntryType.Expense, data.Expenses));
        plots.Add(GetBarPlot(EntryType.Income, data.Incomes));
        plots.Add(GetBarPlot(EntryType.Invest, data.Invests));
        plots.Add(GetBarPlot(EntryType.ReInvest, data.ReInvests));

        return new GraphTabViewModel("Total", plots);

        Plot GetCumulativeResultsPlot()
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

            var plot = new Plot();

            var total = plot.Add.Scatter(data.Dates, res);
            //total.FillY = true;
            //total.FillYColor = total.Color.WithAlpha(.2);

            var withInvest = plot.Add.Scatter(data.Dates, resWithInvest);
            withInvest.Color = EntryType.Invest.GetScottPlotColor();
            //withInvest.FillY = true;
            //withInvest.FillYColor = w0ithInvest.Color.WithAlpha(.2);

            plot.Axes.DateTimeTicksBottom();

            return plot;
        }

        Plot GetBarPlot(EntryType type, double[] values)
        {
            var plot = new Plot();
            var barPlot = plot.Add.Bars(dateDoubles, values);
            barPlot.Color = type.GetScottPlotColor();
            barPlot.SetSize(1000 / values.Length); // magic
            barPlot.SetBorderColor(type.GetScottPlotColor());
            plot.Axes.DateTimeTicksBottom();

            return plot;
        }
    }

    private GraphTabViewModel CreateInvestsTab(BookDatedData data)
    {
        var plots = new List<Plot>();

        var dateDoubles = data.Dates.Select(x => x.ToOADate()).ToArray();

        plots.Add(GetAccountsArea());

        return new GraphTabViewModel("Invests", plots);

        Plot GetAccountsArea()
        {
            var plot = new Plot();

            var series = new List<(double[], SeriesInfo)>();

            foreach (var (account, values) in data.InvestsByAccount)
            {
                var info = new SeriesInfo(account.Name, Color.RandomHue());

                series.Add((values.CumulativeSum().ToArray(), info));
            }

            plot.AddArea(series, dateDoubles, true);
            plot.Axes.DateTimeTicksBottom();

            return plot;
        }
    }
}
