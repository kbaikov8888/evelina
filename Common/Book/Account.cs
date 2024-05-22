namespace BookImpl;

public class Account
{
    public string Name { get; }

    public Account(string name)
    {
        Name = name;
    }
}

public sealed class InvestAccount : Account
{
    public InvestAccount(string name) : base(name)
    {
    }
}