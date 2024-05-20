using evelina.ViewModels.Common;
using MsBox.Avalonia;
using PortfolioInterface;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace evelina.ViewModels;

public class TransactionEditingViewModel : WindowViewModelBase
{
    public ICommand ApplyCommand { get; }
    public ICommand CancelCommand { get; }

    [Reactive]
    public DateTimeOffset Datetime { get; set; }

    [Reactive]
    public ETransaction Type { get; set; }

    public IEnumerable<ETransaction> Types => Enum.GetValues(typeof(ETransaction)).Cast<ETransaction>();

    [Reactive]
    public double? Price { get; set; }

    [Reactive]
    public double? Amount { get; set; }

    [Reactive]
    public string? Note { get; set; }


    private readonly AssetViewModel _asset;
    private ITransaction? _transaction;


    public TransactionEditingViewModel(AssetViewModel asset, IMainViewModel main) : this(main)
    {
        _asset = asset;
        _transaction = null;

        Datetime = new DateTimeOffset(DateTime.Now);
        Type = ETransaction.Buy;
    }

    public TransactionEditingViewModel(ITransaction transaction, AssetViewModel asset, IMainViewModel main) : this(main)
    {
        _asset = asset;
        _transaction = transaction;

        Datetime = new DateTimeOffset(new DateTime(transaction.Datetime));
        Type = transaction.Type;
        Amount = transaction.Amount;
        Price = transaction.Price;
        Note = transaction.Note;
    }

    private TransactionEditingViewModel(IMainViewModel main) : base(main)
    {
        ApplyCommand = ReactiveCommand.Create(Apply);
        CancelCommand = ReactiveCommand.Create(Close);
    }


    private async void Apply()
    {
        if (!Price.HasValue || !Amount.HasValue)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Warning", "Fill Amount and Price!");
                
            await box.ShowAsync();
            return;
        }

        if (_transaction is null)
        {
            var transaction = _asset.Model.CreateTransaction(Datetime.Ticks, Type, Price.Value, Amount.Value);
            transaction.Note = Note;
            _asset.AddTransaction(transaction);
        }
        else
        {
            _transaction.Datetime = Datetime.Ticks;
            _transaction.Type = Type;
            _transaction.Price = Price.Value;
            _transaction.Amount = Amount.Value;
            _transaction.Note = Note;
        }

        _asset.RefreshTransactions();

        Close();
    }

    private void Close()
    {
        TurnBack();
    }
}