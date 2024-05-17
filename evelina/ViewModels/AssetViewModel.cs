using evelina.ViewModels.Common;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PortfolioInterface;

namespace evelina.ViewModels;

public class AssetViewModel : WindowViewModelBase, IDisposable
{
    internal delegate void DeleteMe(AssetViewModel vm);
    internal event DeleteMe? DeleteMeEvent;

    internal delegate void EditMe(AssetViewModel vm);
    internal event EditMe? EditMeEvent;

    public ICommand CreateTransactionCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }


    public string Name => Model.Name;
    public double? TargetVolume => Model.TargetVolume;
    public double? TargetSellPrice => Model.TargetSellPrice;
    public double? TargetShare => Model.TargetShare;
    public double? Volume
    {
        get
        {
            if (Model.Stat.Volume < IPortfolio.POSSIBLE_DELTA)
            {
                return null;
            }

            return Model.Stat.Volume;
        }
    }
    public double? SellPrice => Model.Stat.SellPrice;
    public double? Share
    {
        get
        {
            if (Model.Stat.Share < IPortfolio.POSSIBLE_DELTA / 100)
            {
                return null;
            }

            return Model.Stat.Share;
        }
    }
    public double? BuyedVolume => Model.Stat.BuyedVolume;
    public double? BuyedShare => Model.Stat.BuyedShare;
    public EAssetStatus? Status => Model.Stat.Status;
    public bool? IsFree => Status is EAssetStatus.Free;

    public ObservableCollection<TransactionViewModel> Transactions { get; private set; }

    public ObservableCollection<TargetViewModel> Targets { get; private set; }

    internal IAsset Model { get; }


    public AssetViewModel(IAsset model, MainViewModel main) : base(main)
    {
        Model = model;
        Transactions = new ObservableCollection<TransactionViewModel>();
        Targets = new ObservableCollection<TargetViewModel>();

        foreach (var transaction in model.GetTransactions())
        {
            AddTransaction(transaction);
        }
        RefreshTransactions();

        CreateTransactionCommand = ReactiveCommand.Create(CreateTransaction);
        EditCommand = ReactiveCommand.Create(Edit);
        DeleteCommand = ReactiveCommand.Create(Delete);
    }


    public void Dispose()
    {
        foreach (var transaction in Transactions)
        {
            transaction.DeleteMeEvent -= DeleteTransaction;
            transaction.EditMeEvent -= EditTransaction;
            transaction.Dispose();
        }
        Transactions.Clear();
    }

    internal void AddTransaction(ITransaction transaction)
    {
        var vm = new TransactionViewModel(transaction);
        vm.DeleteMeEvent += DeleteTransaction;
        vm.EditMeEvent += EditTransaction;
        Transactions.Add(vm);
    }

    private void EditTransaction(TransactionViewModel vm)
    {
        Main.ActiveWindow = new TransactionEditingViewModel(vm.Model, this, Main);
    }

    private async void DeleteTransaction(TransactionViewModel vm)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(
            "Deleting",
            $"Are you sure to delete transaction from {vm.DatetimeString}",
            ButtonEnum.YesNo);

        var res = await box.ShowAsync();
        if (res != ButtonResult.Yes)
        {
            return;
        }

        Model.DeleteTransaction(vm.Model);

        vm.EditMeEvent -= EditTransaction;
        vm.DeleteMeEvent -= DeleteTransaction;
        Transactions.Remove(vm);
    }

    private void CreateTransaction()
    {
        Main.ActiveWindow = new TransactionEditingViewModel(this, Main);
    }

    private void Edit()
    {
        EditMeEvent?.Invoke(this);
    }

    private void Delete()
    {
        DeleteMeEvent?.Invoke(this);
    }

    internal void RefreshTransactions()
    {
        Transactions = new ObservableCollection<TransactionViewModel>(Transactions.OrderByDescending(x => x.Model.Datetime));
    }
}