using evelina.Controls;

namespace PortfolioAvalonia.ViewModel;

public class AssetsTableViewModel : WindowViewModelBase, IMenuCompatible
{
    public PortfolioViewModel Portfolio { get; }

    public AssetsTableViewModel(PortfolioViewModel portfolio, IMainViewModel main) : base(main)
    {
        Portfolio = portfolio;
    }
}