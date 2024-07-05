using BookImpl.Elements;
using BookImpl.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookImpl;

public class BookDatedData
{
    public DateLevel Level { get; }

    public DateTime[] Dates { get; private set; } = Array.Empty<DateTime>();
    public double[] DatesAsDoubles { get; private set; } = Array.Empty<double>();

    // values
    public double[] Expenses { get; private set; } = Array.Empty<double>();
    public double[] Incomes { get; private set; } = Array.Empty<double>();
    public double[] Invests { get; private set; } = Array.Empty<double>();
    public double[] ReInvests { get; private set; } = Array.Empty<double>();
    public double[] Results { get; private set; } = Array.Empty<double>();

    // categorical
    public Dictionary<InvestAccountFamily, double[]> InvestsByFamilies { get; } = new();
    public Dictionary<IncomeCategory, double[]> ParentIncomeCategories { get; } = new();
    public Dictionary<ExpenseCategory, double[]> ParentExpenseCategories { get; } = new();
    public Dictionary<IncomeCategory, double[]> IncomeCategories { get; } = new();
    public Dictionary<ExpenseCategory, double[]> ExpenseCategories { get; } = new();

    private readonly Book _book;


    internal BookDatedData(DateLevel level, Book book)
    {
        Level = level;
        _book = book;
    }


    private void Clear()
    {
        InvestsByFamilies.Clear();
        ParentIncomeCategories.Clear();
        ParentExpenseCategories.Clear();
        IncomeCategories.Clear();
        ExpenseCategories.Clear();
    }

    public void Calculate(DateTime[] dates)
    {
        Clear();

        Dates = dates;
        DatesAsDoubles = dates.Select(x => x.ToOADate()).ToArray();

        Expenses = new double[Dates.Length];
        Incomes = new double[Dates.Length];
        Invests = new double[Dates.Length];
        ReInvests = new double[Dates.Length];
        Results = new double[Dates.Length];

        var entries = _book.GetEntriesFromFirst();

        int index = 0;
        foreach (var entry in entries)
        {
            for (int i = index; i < Dates.Length; i++)
            {
                if (Level.IsEqual(entry.DateTime, Dates[i]))
                {
                    index = i;
                    break;
                }
            }

            CalculateValues(entry, index);
            CalculateInvestsByAccount(entry, index);
            CalculateParentIncomeCategories(entry, index);
            CalculateParentExpenseCategories(entry, index);
            CalculateIncomeCategories(entry, index);
            CalculateExpenseCategories(entry, index);
        }
    }

    private void CalculateValues(Entry entry, int index)
    {
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

    private void CalculateInvestsByAccount(Entry entry, int index)
    {
        if (entry is TransferEntry transfer)
        {
            if (transfer.Sender is InvestAccount from)
            {
                if (!InvestsByFamilies.ContainsKey(from.Family))
                {
                    InvestsByFamilies[from.Family] = new double[Dates.Length];
                }

                InvestsByFamilies[from.Family][index] -= transfer.SenderAmount;
            }

            if (transfer.Receiver is InvestAccount to)
            {
                if (!InvestsByFamilies.ContainsKey(to.Family))
                {
                    InvestsByFamilies[to.Family] = new double[Dates.Length];
                }

                InvestsByFamilies[to.Family][index] += transfer.ReceiverAmount;
            }
        }
    }

    private void CalculateParentIncomeCategories(Entry entry, int index)
    {
        if (entry is IncomeEntry { Category.ParentCategory: IncomeCategory incomeCategory } income)
        {
            if (!ParentIncomeCategories.ContainsKey(incomeCategory))
            {
                ParentIncomeCategories[incomeCategory] = new double[Dates.Length];
            }

            ParentIncomeCategories[incomeCategory][index] += income.Amount;
        }
    }

    private void CalculateParentExpenseCategories(Entry entry, int index)
    {
        if (entry is ExpenseEntry { Category.ParentCategory: ExpenseCategory expenseCategory } expense)
        {
            if (!ParentExpenseCategories.ContainsKey(expenseCategory))
            {
                ParentExpenseCategories[expenseCategory] = new double[Dates.Length];
            }

            ParentExpenseCategories[expenseCategory][index] += expense.Amount;
        }
    }
    private void CalculateIncomeCategories(Entry entry, int index)
    {
        if (entry is IncomeEntry { Category: IncomeCategory incomeCategory } income)
        {
            if (!IncomeCategories.ContainsKey(incomeCategory))
            {
                IncomeCategories[incomeCategory] = new double[Dates.Length];
            }

            IncomeCategories[incomeCategory][index] += income.Amount;
        }
    }

    private void CalculateExpenseCategories(Entry entry, int index)
    {
        if (entry is ExpenseEntry { Category: ExpenseCategory expenseCategory } expense)
        {
            if (!ExpenseCategories.ContainsKey(expenseCategory))
            {
                ExpenseCategories[expenseCategory] = new double[Dates.Length];
            }

            ExpenseCategories[expenseCategory][index] += expense.Amount;
        }
    }
}
