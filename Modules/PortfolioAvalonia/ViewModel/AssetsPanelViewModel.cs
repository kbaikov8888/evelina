using evelina.Controls;

namespace PortfolioAvalonia.ViewModel;

public class AssetsPanelViewModel : WindowViewModelBase, IMenuCompatible
{
    public PortfolioViewModel Portfolio { get; }

    internal AssetsPanelViewModel(PortfolioViewModel portfolio) : base(portfolio)
    {
        Portfolio = portfolio;
    }
}