namespace BookImpl;

public abstract class Category
{
    public string Name { get; }

    protected Category(string name)
    {
        Name = name;
    }
}

public class IncomeCategory : Category
{
    public IncomeCategory(string name) : base(name)
    {
    }
}

public class ExpenseCategory : Category
{
    public ExpenseCategory(string name) : base(name)
    {
    }
}