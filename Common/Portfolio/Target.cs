using System.Text.Json;
using PortfolioImpl.DTO;
using PortfolioInterface;

namespace PortfolioImpl;

internal class Target : Item, ITarget
{
    public double Price { get; set; }
    public double Volume { get; set; }


    public Target(string id, long creationDate, string parentId) : base(id, creationDate, parentId)
    {
        Level = EItemLevel.Target;
    }


    #region IItem
    public override string ToJson()
    {
        var dto = new TargetDTO()
        {
            Price = Price,
            Volume = Volume,
        };

        return JsonSerializer.Serialize(dto);
    }

    public override void FromJson(string json)
    {
        var dto = JsonSerializer.Deserialize<TargetDTO>(json);
        if (dto is null) return;

        Price = dto.Price; 
        Volume = dto.Volume;
    }
    #endregion
}