using System;
using BookImpl.Enum;

namespace BookImpl;

public abstract class Entry
{
    public DateTime DateTime { get; }
    public double Amount { get; }
    public Project? Project { get; internal set; }
    public string? Note { get; internal set; }

    public abstract EntryType Type { get; }

    protected Entry(double amount, DateTime dateTime)
    {
        Amount = amount;
        DateTime = dateTime;
    }
}

public abstract class ExternalEntry : Entry
{
    public Account Account { get; }
    public abstract Category Category { get; }

    protected ExternalEntry(
        double amount, 
        DateTime dateTime, 
        Account account) : base(amount, dateTime)
    {
        Account = account;
    }
}

public sealed class ExpenseEntry : ExternalEntry
{
    public ExpenseCategory ExpenseCategory { get; }
    public override Category Category => ExpenseCategory;

    public override EntryType Type => EntryType.Expense;

    public ExpenseEntry(
        double amount, 
        DateTime dateTime, 
        Account account,
        ExpenseCategory expenseCategory) : base(amount, dateTime, account)
    {
        ExpenseCategory = expenseCategory;
    }
}

public sealed class IncomeEntry : ExternalEntry
{
    public IncomeCategory IncomeCategory { get; }
    public override Category Category => IncomeCategory;
    public override EntryType Type => EntryType.Income;

    public IncomeEntry(
        double amount, 
        DateTime dateTime, 
        Account account,
        IncomeCategory incomeCategory) : base(amount, dateTime, account)
    {
        IncomeCategory = incomeCategory;
    }
}

public class TransferEntry : Entry
{
    public Account Sender { get; }
    public Account Receiver { get; }

    public override EntryType Type => EntryType.Transfer;

    public TransferEntry(
        double amount, 
        DateTime dateTime,
        Account sender,
        Account receiver) : base(amount, dateTime)
    {
        Sender = sender;
        Receiver = receiver;
    }
}

public sealed class InvestingEntry : TransferEntry
{
    public override EntryType Type => EntryType.Invest;

    public InvestingEntry(
        double amount, 
        DateTime dateTime,
        Account sender,
        InvestAccount receiver) : base(amount, dateTime, sender, receiver)
    {
    }
}
public sealed class ReInvestingEntry : TransferEntry
{
    public override EntryType Type => EntryType.ReInvest;

    public ReInvestingEntry(
        double amount, 
        DateTime dateTime,
        InvestAccount sender,
        Account receiver) : base(amount, dateTime, sender, receiver)
    {
    }
}