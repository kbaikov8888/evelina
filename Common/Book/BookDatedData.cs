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

    // categorical
    public Dictionary<InvestAccountFamily, double[]> InvestsByFamilies { get; } = new();

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
        InvestsByFamilies.Clear();
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
                case ExpenseEntry expense:
                    Expenses[index] += expense.Amount; break;
                case IncomeEntry income:
                    Incomes[index] += income.Amount; break;
                case InvestingEntry investing:
                    Invests[index] += investing.ReceiverAmountInDefaultCurrency; break;
                case ReInvestingEntry reInvesting:
                    ReInvests[index] += reInvesting.SenderAmountInDefaultCurrency; break;
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
        InvestsByFamilies.Clear();

        foreach (var family in _book.InvestAccountFamilies)
        {
            InvestsByFamilies[family] = new double[Dates.Length];
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

            if (entry is TransferEntry transfer)
            {
                if (transfer.Sender is InvestAccount from)
                {
                    InvestsByFamilies[from.Family][index] -= transfer.SenderAmount;
                }

                if (transfer.Receiver is InvestAccount to)
                {
                    InvestsByFamilies[to.Family][index] += transfer.ReceiverAmount;
                }
            }
        }
    }
}
