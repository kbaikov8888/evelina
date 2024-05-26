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

        CalculateValues();
    }


    public void Dispose()
    {
    }

    private void CalculateValues()
    {
        int lastDateIndex = 0;
        foreach (var entry in _entries)
        {
            for (int i = lastDateIndex; i < Dates.Length; i++)
            {
                if (Level.IsEqual(entry.DateTime, Dates[i]))
                {
                    lastDateIndex = i;
                    break;
                }
            }

            switch (entry)
            {
                case ExpenseEntry:
                    Expenses[lastDateIndex] += entry.Amount; break;
                case IncomeEntry:
                    Incomes[lastDateIndex] += entry.Amount; break;
                case InvestingEntry:
                    Invests[lastDateIndex] += entry.Amount; break;
                case ReInvestingEntry:
                    ReInvests[lastDateIndex] += entry.Amount; break;
            }
        }
    }
}

