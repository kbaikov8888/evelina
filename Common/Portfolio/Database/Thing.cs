using PortfolioInterface;

namespace PortfolioImpl.Database;

internal class Thing
{
    public string Id { get; set; }
    public string ParentId { get; set; }
    public long CreationDate { get; set; }
    public string JsonValue { get; set; }
    public EItemLevel Level { get; set; }

    public Thing(IItem item)
    {
        Id = item.Id;
        ParentId = item.ParentId;
        CreationDate = item.CreationDate;
        JsonValue = item.ToJson();
        Level = item.Level;
    }

    public Thing()
    {
        Id = "error";
        ParentId = "error";
        JsonValue = "error";
    }
}