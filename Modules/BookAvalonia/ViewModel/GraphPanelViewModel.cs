using BookImpl;
using BookImpl.Enum;
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

        Total = CreateTotalTab();
    }


    private GraphTabViewModel CreateTotalTab()
    {
        var plots = new List<Plot>();

        var months = _book.CalculatedData.Months.Dates;
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

            res[i] += _book.CalculatedData.Months.Incomes[i];
            res[i] += _book.CalculatedData.Months.ReInvests[i];
            res[i] -= _book.CalculatedData.Months.Invests[i];
            res[i] -= _book.CalculatedData.Months.Expenses[i];
        }

        var total = new Plot();
        total.Add.Scatter(months, res);
        total.Axes.DateTimeTicksBottom();
        plots.Add(total);

        var expensesbars = new Plot();
        var expenses = expensesbars.Add.Bars(months.Select(x => x.ToOADate()).ToArray(), _book.CalculatedData.Months.Expenses);
        expenses.Color = Color.FromARGB(EntryType.Expense.GetColor().ToUInt32());
        expensesbars.Axes.DateTimeTicksBottom();
        plots.Add(expensesbars);

        var incomesbars = new Plot();
        var incomes = incomesbars.Add.Bars(months.Select(x => x.ToOADate()).ToArray(), _book.CalculatedData.Months.Incomes);
        incomes.Color = Color.FromARGB(EntryType.Income.GetColor().ToUInt32());
        incomesbars.Axes.DateTimeTicksBottom();
        plots.Add(incomesbars);


        return new GraphTabViewModel("Total", plots);
    }
}