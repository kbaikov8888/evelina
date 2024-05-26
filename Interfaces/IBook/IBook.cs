namespace BookInterface;

public interface IBook
{
    string Name { get; }

    IList<IEntry> GetAllEntries();
    IList<ICategory> GetAllCategories();
    IList<IAccount> GetAllAccounts();
}