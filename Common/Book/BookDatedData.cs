using BookImpl.Elements;
using BookImpl.Enum;
using System;
using System.Collections.Generic;

namespace BookImpl;

public class BookDatedData : IDisposable
{
    internal DateLevel Level { get; }

    public DateTime[] Dates { get; }

    // values
    public double[] Expenses { get; }
    public double[] Incomes { get; }
    public double[] Invests { get; }
    public double[] ReInvests { get; }
    public double[] Results { get; }

    private readonly List<Entry> _entries;


    internal BookDatedData(DateLevel level, DateTime[] dates, List<Entry> entries)
    {
        Level = level;
        Dates = dates;
        _entries = entries;

        Expenses = new double[Dates.Length];
        Incomes = new double[Dates.Length];
        Invests = new double[Dates.Length];
        ReInvests = new double[Dates.Length];
        Results = new double[Dates.Length];

        CalculateValues();
    }


    public void Dispose()
    {
    }

    private void CalculateValues()
    {
        int index = 0;
        foreach (var entry in _entries)
        {
            for (int i = index; i < Dates.Length; i++)
            {
                if (Level.IsEqual(entry.DateTime, Dates[i]))
                {
                    index = i;
                    break;
                }
            }

            switch (entry)
            {
                case ExpenseEntry:
                    Expenses[index] += entry.Amount; break;
                case IncomeEntry:
                    Incomes[index] += entry.Amount; break;
                case InvestingEntry:
                    Invests[index] += entry.Amount; break;
                case ReInvestingEntry:
                    ReInvests[index] += entry.Amount; break;
            }

            Results[index] = Incomes[index] - Expenses[index] - Invests[index] + ReInvests[index];
        }
    }
}

