using System;
using BookImpl.Enum;

namespace BookImpl.Elements;

// Currency = CurrencyRate * DefaultCurrency

public abstract class Entry
{
    public DateTime DateTime { get; }
    public Project? Project { get; internal set; }
    public string? Note { get; internal set; }

    public abstract EntryType Type { get; }

    protected Entry(DateTime dateTime)
    {
        DateTime = dateTime;
    }
}

public abstract class ExternalEntry : Entry
{
    public Account Account { get; }
    public double Amount { get; }
    public double CurrencyRate { get; } 
    public abstract Category Category { get; }

    protected ExternalEntry(
        double amount,
        DateTime dateTime,
        BankAccount account,
        double currencyRate) : base(dateTime)
    {
        Amount = amount;
        Account = account;
        CurrencyRate = currencyRate;
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
        BankAccount account,
        ExpenseCategory expenseCategory,
        double currencyRate) : base(amount, dateTime, account, currencyRate)
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
        BankAccount account,
        IncomeCategory incomeCategory,
        double currencyRate) : base(amount, dateTime, account, currencyRate)
    {
        IncomeCategory = incomeCategory;
    }
}

public class TransferEntry : Entry
{
    public Account Sender { get; }
    public double SenderCurrencyRate { get; }
    public double SenderAmount { get; }
    public double SenderAmountInDefaultCurrency => SenderAmount * SenderCurrencyRate;

    public Account Receiver { get; }
    public double ReceiverCurrencyRate { get; }
    public double ReceiverAmount { get; }
    public double ReceiverAmountInDefaultCurrency => ReceiverAmount * ReceiverCurrencyRate;


    public override EntryType Type => EntryType.Transfer;

    public TransferEntry(
        DateTime dateTime,
        double senderAmount,
        double receiverAmount,
        Account sender,
        Account receiver,
        double senderCurrencyRate,
        double receiverCurrencyRate) : base(dateTime)
    {
        Sender = sender;
        Receiver = receiver;
        SenderCurrencyRate = senderCurrencyRate;
        ReceiverCurrencyRate = receiverCurrencyRate;
        SenderAmount = senderAmount;
        ReceiverAmount = receiverAmount;
    }
}

public sealed class InvestingEntry : TransferEntry
{
    public override EntryType Type => EntryType.Invest;

    public InvestAccount InvestAccount => (InvestAccount)Receiver;

    public InvestingEntry(
        DateTime dateTime,
        double senderAmount,
        double receiverAmount,
        Account sender,
        InvestAccount receiver,
        double senderCurrencyRate,
        double receiverCurrencyRate) : base(dateTime, senderAmount, receiverAmount, sender, receiver, senderCurrencyRate, receiverCurrencyRate)
    {
    }
}
public sealed class ReInvestingEntry : TransferEntry
{
    public override EntryType Type => EntryType.ReInvest;

    public InvestAccount InvestAccount => (InvestAccount)Sender;

    public ReInvestingEntry(
        DateTime dateTime,
        double senderAmount,
        double receiverAmount,
        InvestAccount sender,
        Account receiver,
        double senderCurrencyRate,
        double receiverCurrencyRate) : base(dateTime, senderAmount, receiverAmount, sender, receiver, senderCurrencyRate, receiverCurrencyRate)
    {
    }
}