using BookInterface;

namespace BookImpl;

internal class Book : IBook
{
    public string Name { get; set; }

    public long CreateDate { get; set; }


    internal Book(string name)
    {
        Name = name;
    }

    public IList<ICategory> GetAllCategories()
    {
        throw new NotImplementedException();
    }

    public IList<IEntry> GetAllEntries()
    {
        throw new NotImplementedException();
    }
}