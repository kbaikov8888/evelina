using PortfolioInterface;

namespace PortfolioImpl.DTO;

internal class TransactionDTO
{
    public long Datetime { get; set; }
    public ETransaction Type { get; set; }
    public double Price { get; set; }
    public double Amount { get; set; }
    public string? Note { get; set; }

    public TransactionDTO() { }
}