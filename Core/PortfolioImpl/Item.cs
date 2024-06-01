using PortfolioInterface;

namespace PortfolioImpl;

internal abstract class Item : IItem
{
    public string Id { get; }

    public long CreationDate { get; }

    public string ParentId { get; }

    public EItemLevel Level { get; protected set; }


    protected Item(string id, long creationDate, string parentId)
    {
        Id = id;
        CreationDate = creationDate;
        ParentId = parentId;
    }


    public abstract void FromJson(string json);

    public abstract string ToJson();
}