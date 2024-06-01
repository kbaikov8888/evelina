namespace PortfolioInterface;

public interface IItem
{
    string Id { get; }

    string ParentId { get; }

    long CreationDate { get; }

    EItemLevel Level { get; }

    string ToJson();

    void FromJson(string json);
}