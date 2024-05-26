namespace BookInterface;

public interface ICategory
{
    string Title { get; }
}

public interface IIncomeCategory : ICategory
{
}

public interface IExpenseCategory : ICategory
{
}