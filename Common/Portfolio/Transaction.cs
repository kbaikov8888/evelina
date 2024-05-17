using System.Text.Json;
using PortfolioImpl.DTO;
using PortfolioInterface;

namespace PortfolioImpl;

internal class Transaction : Item, ITransaction
{
    public long Datetime { get; set; }

    private ETransaction _type;
    public ETransaction Type
    {
        get => _type;
        set
        {
            _type = value;
            _parent.UpdateStat();
        }
    }

    private double _price;
    public double Price
    {
        get => _price;
        set
        {
            _price = value;
            _parent.UpdateStat();
        }
    }

    private double _amount;
    public double Amount
    {
        get => _amount;
        set
        {
            _amount = value;
            _parent.UpdateStat();
        }
    }

    public string? Note { get; set; }

    private readonly Asset _parent;


    public Transaction(string id, long creationDate, string parentId, Asset parent) : base(id, creationDate, parentId)
    {
        Level = EItemLevel.Transaction;
        _parent = parent;
    }


    public override void FromJson(string json)
    {
        var dto = JsonSerializer.Deserialize<TransactionDTO>(json);
        if (dto is null) return;

        Datetime = dto.Datetime;
        Type = dto.Type;
        Price = dto.Price;
        Amount = dto.Amount;
        Note = dto.Note;
    }

    public override string ToJson()
    {
        var dto = new TransactionDTO()
        {
            Datetime = Datetime,
            Type = Type,
            Price = Price,
            Amount = Amount,
            Note = Note,
        };

        return JsonSerializer.Serialize(dto);
    }
}