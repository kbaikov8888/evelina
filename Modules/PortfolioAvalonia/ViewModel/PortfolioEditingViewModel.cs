using evelina.ViewModels.Common;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Windows.Input;
using PortfolioInterface;

namespace evelina.ViewModels;

public class PortfolioEditingViewModel : WindowViewModelBase, IDisposable
{
    public ICommand ApplyCommand { get; }
    public ICommand CancelCommand { get; }

    [Reactive]
    public string Name { get; set; }

    [Reactive]
    public string Description { get; set; }

    internal IPortfolio Model { get; }


    public PortfolioEditingViewModel(IPortfolio model, IMainViewModel main) : base(main)
    {
        Model = model;
        Name = model.Name;
        Description = model.Description;

        ApplyCommand = ReactiveCommand.Create(Apply);
        CancelCommand = ReactiveCommand.Create(Close);
    }


    public void Dispose()
    {
    }


    private void Apply()
    {
        Model.Name = Name;
        Model.Description = Description;
        Close();
    }

    private void Close()
    {
        TurnBack();
    }
}