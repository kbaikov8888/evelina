using PortfolioInterface;

namespace PortfolioImpl;

internal class AssetStat : IAssetStat
{
    public double Volume { get; set; }
    public double SellPrice { get; set; }
    public double Share { get; set; }
    public double Amount { get; set; }
    public double BuyedVolume { get; set; }
    public double BuyedShare { get; set; }
    public EAssetStatus Status { get; set; }
}