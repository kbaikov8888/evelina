using CTS;
using PortfolioInterface;
using System.Collections.Generic;

namespace PortfolioAvalonia;

public static class IPortfolio_Extension
{
    public static void AddToPortfolio(this IPortfolio portfolio, List<CTS.CTS> transactions, bool createNewAssets = true)
    {
        foreach (var cts in transactions)
        {
            var asset = portfolio.GetAsset(cts.Symbol);
            if (asset is null)
            {
                if (!createNewAssets)
                {
                    continue;
                }

                asset = portfolio.CreateAsset(cts.Symbol);
            }

            var type = cts.Type == EType.buy ? ETransaction.Buy : ETransaction.Sell;

            asset.CreateTransaction(cts.Datetime, type, cts.Price, cts.Amount);
        }
    }
}