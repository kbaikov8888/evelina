using System;
using evelina.ViewModels.Common;

namespace evelina.ViewModels;

public class AssetsTableViewModel : WindowViewModelBase, IDisposable, IMenuCompatible
{
    public PortfolioViewModel Portfolio { get; }

    public AssetsTableViewModel(PortfolioViewModel portfolio, MainViewModel main) : base(main)
    {
        Portfolio = portfolio;
    }

    public void Dispose()
    {
    }
}