using evelina.ViewModels.Common;
using ReactiveUI;
using System;
using System.Windows.Input;
using PortfolioInterface;

namespace evelina.ViewModels;

public class TransactionViewModel : ViewModelBase, IDisposable
{
    internal delegate void DeleteMe(TransactionViewModel transaction);
    internal event DeleteMe? DeleteMeEvent;

    internal delegate void EditMe(TransactionViewModel transaction);
    internal event EditMe? EditMeEvent;

    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

    public ETransaction? Type => Model.Type;
    public double? Amount => Model.Amount;
    public double? Price => Model.Price;
    public double? Volume => Model.Volume;
    public string DatetimeString => $"{new DateTime(Model.Datetime) : 0)}";
        
    internal ITransaction Model { get; }


    public TransactionViewModel(ITransaction model)
    {
        Model = model;

        EditCommand = ReactiveCommand.Create(Edit);
        DeleteCommand = ReactiveCommand.Create(Delete);
    }


    public void Dispose()
    {
    }

    private void Edit()
    {
        EditMeEvent?.Invoke(this);
    }

    private void Delete()
    {
        DeleteMeEvent?.Invoke(this);
    }
}