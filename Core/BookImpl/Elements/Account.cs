using BookImpl.Enum;

namespace BookImpl.Elements;

public abstract class Account
{
    public string Name { get; }
    public Currency Currency { get; }

    protected Account(string name, Currency currency)
    {
        Name = name;
        Currency = currency;
    }
}

public sealed class BankAccount : Account
{
    public BankAccount(string name, Currency currency) : base(name, currency)
    {
    }
}

public sealed class InvestAccount : Account
{
    public InvestAccountFamily Family { get; }

    public InvestAccount(string name, Currency currency, InvestAccountFamily family) : base(name, currency)
    {
        Family = family;
    }
}

public sealed class InvestAccountFamily
{
    public string Name { get; }

    public InvestAccountFamily(string name)
    {
        Name = name;
    }
}