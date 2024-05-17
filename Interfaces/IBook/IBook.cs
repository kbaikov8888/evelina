namespace BookInterface;

public interface IBook
{
    string Name { get; }

    long CreateDate { get; }


    IList<IEntry> GetAllTransactions();

    IList<ICategory> GetAllCategories();
}