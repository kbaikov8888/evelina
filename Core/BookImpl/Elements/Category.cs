namespace BookImpl.Elements;

public abstract class Category
{
    public string Name { get; }
    public Category? ParentCategory { get; internal set; }

    protected Category(string name)
    {
        Name = name;
    }
}

public sealed class IncomeCategory : Category
{
    public IncomeCategory(string name) : base(name)
    {
    }
}

public sealed class ExpenseCategory : Category
{
    public ExpenseCategory(string name) : base(name)
    {
    }
}