using System;
using evelina.ViewModels.Common;
using PortfolioInterface;

namespace evelina.ViewModels;

public class TargetViewModel : ViewModelBase, IDisposable
{
    internal ITarget Model { get; }


    public TargetViewModel(ITarget model)
    {
        Model = model;
    }


    public void Dispose()
    {
    }
}