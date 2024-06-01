using System.ComponentModel;

namespace PortfolioInterface;

public enum EItemLevel
{
    Portfolio = 0,
    Asset = 1,
    Transaction = 2,
    Target = 3,
}

public enum ETransaction
{
    Buy = 0,
    Sell = 1,
}

public enum EAssetStatus
{
    [Description("waiting")]
    Waiting,
    [Description("buyed")]
    Buyed,
    [Description("buyed fully")]
    Buyed_fully,
    [Description("free")]
    Free,
}