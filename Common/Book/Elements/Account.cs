namespace BookImpl.Elements;

public abstract class Account
{
    public string Name { get; }

    public Account(string name)
    {
        Name = name;
    }
}

public sealed class BankAccount : Account
{
    public BankAccount(string name) : base(name)
    {
    }
}

public sealed class InvestAccount : Account
{
    public InvestAccount(string name) : base(name)
    {
    }
}