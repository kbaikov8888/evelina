namespace BookInterface;

public interface IEntry
{
    DateTime DateTime { get; }
    double Amount { get; }
    double CurrencyRate { get; }
    IProject? Project { get; }
}

public interface IExternalEntry : IEntry
{
    IAccount Account { get; }
}

public interface IExpenseEntry : IExternalEntry
{
    IExpenseCategory ExpenseCategory { get; }
}

public interface IIncomeEntry : IExternalEntry
{
    IIncomeCategory IncomeCategory { get; }
}

public interface ITransferEntry : IEntry
{
    IAccount Sender { get; }
    IAccount Receiver { get; }
}

public interface IInvestingEntry : ITransferEntry
{
}