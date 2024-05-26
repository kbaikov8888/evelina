using PortfolioInterface;
using ReactiveUI;
using System;

namespace PortfolioAvalonia.ViewModel;

public class TargetViewModel : ReactiveObject, IDisposable
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