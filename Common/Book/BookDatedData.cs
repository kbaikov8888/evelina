using BookImpl.Elements;
using BookImpl.Enum;
using System;
using System.Collections.Generic;
using System.Security.Principal;

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

    // categorical
    public Dictionary<InvestAccount, double[]> InvestsByAccount { get; } = new();

    private readonly List<Entry> _entries;
    private readonly Book _book;


    internal BookDatedData(DateLevel level, DateTime[] dates, Book book)
    {
        Level = level;
        Dates = dates;
        _entries = book.GetEntriesFromFirst();
        _book = book;

        Expenses = new double[Dates.Length];
        Incomes = new double[Dates.Length];
        Invests = new double[Dates.Length];
        ReInvests = new double[Dates.Length];
        Results = new double[Dates.Length];

        CalculateValues();
        CalculateCategorical();
    }


    public void Dispose()
    {
        InvestsByAccount.Clear();
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

    private void CalculateCategorical()
    {
        CalculateInvestsByAccount();
    }

    private void CalculateInvestsByAccount()
    {
        InvestsByAccount.Clear();

        foreach (var account in _book.InvestAccounts)
        {
            InvestsByAccount[account] = new double[Dates.Length];
        }

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

            if (entry is InvestingEntry investing)
            {
                InvestsByAccount[investing.InvestAccount][index] += entry.Amount;
            }
            else if (entry is ReInvestingEntry reInvesting)
            {
                InvestsByAccount[reInvesting.InvestAccount][index] -= entry.Amount;
            }
        }
    }
}

//TODO move to Tools
public static class IEnumerableExtension
{
    public static IEnumerable<double> CumulativeSum(this IEnumerable<double> sequence)
    {
        double sum = 0;
        foreach (var item in sequence)
        {
            sum += item;
            yield return sum;
        }
    }
}