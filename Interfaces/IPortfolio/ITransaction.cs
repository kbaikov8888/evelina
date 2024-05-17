namespace PortfolioInterface;

public interface ITransaction : IItem
{
    #region required
    long Datetime { get; set; }
    ETransaction Type { get; set; }
    double Price { get; set; }
    double Amount { get; set; }
    #endregion

    #region additional
    string? Note { get; set; }
    #endregion

    double Volume => Price * Amount;
}