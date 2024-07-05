using BookImpl.Elements;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BookImpl;

public class BookStat
{
    [Description("Total Amount")]
    public double TotalAmount { get; private set; } 
    public double IncomesAmount { get; private set; } 
    public double ExpenseAmount { get; private set; } 
    public double InvestedAmount { get; private set; } 

    public int TotalEntriesCount { get; private set; } 
    public int IncomesEntriesCount { get; private set; } 
    public int ExpenseEntriesCount { get; private set; } 

    private readonly Book _book;


    public BookStat(Book book)
    {
        _book = book;
    }


    public void Calculate()
    {
        var entries = _book.GetEntriesFromFirst();

        CalculateCounts(entries);
        CalculateAmounts(entries);
    }

    private void CalculateCounts(IReadOnlyCollection<Entry> entries)
    {
        TotalEntriesCount = entries.Count;
        ExpenseEntriesCount = entries.Count(x => x is ExpenseEntry);
        IncomesEntriesCount = entries.Count(x => x is IncomeEntry);
    }

    private void CalculateAmounts(IEnumerable<Entry> entries)
    {
        IncomesAmount = 0;
        ExpenseAmount = 0;
        InvestedAmount = 0;

        foreach (var entry in entries)
        {
            switch (entry)
            {
                case IncomeEntry income:
                    IncomesAmount += income.Amount;
                    break;
                case ExpenseEntry expense:
                    ExpenseAmount += expense.Amount;
                    break;
                case InvestingEntry investing:
                    InvestedAmount += investing.ReceiverAmountInDefaultCurrency;
                    break;
                case ReInvestingEntry reinvesting:
                    InvestedAmount -= reinvesting.SenderAmountInDefaultCurrency;
                    break;
            }
        }

        TotalAmount = IncomesAmount - ExpenseAmount - InvestedAmount;
    }
}