using evelina.ViewModels.Common;

namespace evelina.ViewModels;

public class AssetsTableViewModel : WindowViewModelBase, IMenuCompatible
{
    public PortfolioViewModel Portfolio { get; }

    public AssetsTableViewModel(PortfolioViewModel portfolio, IMainViewModel main) : base(main)
    {
        Portfolio = portfolio;
    }
}