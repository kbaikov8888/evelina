using System;
using evelina.Controls;
using PortfolioInterface;

namespace PortfolioAvalonia.ViewModel;

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