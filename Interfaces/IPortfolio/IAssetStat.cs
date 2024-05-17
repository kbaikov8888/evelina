namespace PortfolioInterface;

public interface IAssetStat
{
    /// <summary>
    /// Volume of current investments.
    /// >=0
    /// </summary>
    double Volume { get; set; }

    /// <summary>
    /// Acturl sell price
    /// </summary>
    double SellPrice { get; set; }

    /// <summary>
    /// Share in portfolio.
    /// in [0;100]
    /// </summary>
    double Share { get; set; }

    /// <summary>
    /// Current amount. 
    /// >=0
    /// </summary>
    double Amount { get; set; }

    /// <summary>
    /// Volume of only Buy transactions
    /// </summary>
    double BuyedVolume { get; set; }

    /// <summary>
    /// Share in portfolio of only Buy transactions
    /// </summary>
    double BuyedShare { get; set; }

    EAssetStatus Status { get; set; }
}