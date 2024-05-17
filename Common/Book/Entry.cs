namespace BookImpl;

public abstract class Entry
{
    public DateTime DateTime { get; }
    public double Amount { get; }
    public double CurrencyRate { get; }
    public Project? Project { get; }
    public string? Note { get; }

    protected Entry(
        double currencyRate, 
        double amount, 
        DateTime dateTime)
    {
        CurrencyRate = currencyRate;
        Amount = amount;
        DateTime = dateTime;
    }
}

public class ExternalEntry : Entry
{
    public Account Account { get; }

    public ExternalEntry(
        double currencyRate, 
        double amount, 
        DateTime dateTime, 
        Account account) : base(currencyRate, amount, dateTime)
    {
        Account = account;
    }
}

public class ExpenseEntry : ExternalEntry
{
    public ExpenseCategory ExpenseCategory { get; }

    public ExpenseEntry(
        double currencyRate, 
        double amount, 
        DateTime dateTime, 
        Account account,
        ExpenseCategory expenseCategory) : base(currencyRate, amount, dateTime, account)
    {
        ExpenseCategory = expenseCategory;
    }
}

public class IncomeEntry : ExternalEntry
{
    public IncomeCategory IncomeCategory { get; }

    public IncomeEntry(
        double currencyRate, 
        double amount, 
        DateTime dateTime, 
        Account account,
        IncomeCategory incomeCategory) : base(currencyRate, amount, dateTime, account)
    {
        IncomeCategory = incomeCategory;
    }
}

public class TransferEntry : Entry
{
    public Account Sender { get; }
    public Account Receiver { get; }

    public TransferEntry(
        double currencyRate, 
        double amount, 
        DateTime dateTime,
        Account sender,
        Account receiver) : base(currencyRate, amount, dateTime)
    {
        Sender = sender;
        Receiver = receiver;
    }
}

public class InvestingEntry : TransferEntry
{
    public InvestingEntry(
        double currencyRate, 
        double amount, 
        DateTime dateTime,
        Account sender,
        Account receiver) : base(currencyRate, amount, dateTime, sender, receiver)
    {
    }
}