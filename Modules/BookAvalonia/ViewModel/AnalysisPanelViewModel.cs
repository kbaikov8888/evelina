using BookImpl;
using BookImpl.Elements;
using BookImpl.Enum;
using evelina.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookAvalonia.ViewModel;

public class AnalysisPanelViewModel : WindowViewModelBase, IMenuCompatible
{
    public IEnumerable<DateLevel> DateLevels => Enum.GetValues(typeof(DateLevel)).Cast<DateLevel>();

    private DateLevel _selectedDateLevel = DateLevel.Month;
    public DateLevel SelectedDateLevel
    {
        get => _selectedDateLevel;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedDateLevel, value);
            RefreshTabs();
        }
    }

    [Reactive]
    public AnalysisTabViewModel<IncomeCategory>? Incomes { get; set; }

    [Reactive]
    public AnalysisTabViewModel<ExpenseCategory>? Expenses { get; set; }

    private readonly Book _book;
    

    public AnalysisPanelViewModel(Book book, IMainViewModel main) : base(main)
    {
        _book = book;
    }


    private void RefreshTabs()
    {
        var data = SelectedDateLevel switch
        {
            DateLevel.Year => _book.CalculatedData.Years,
            DateLevel.Month => _book.CalculatedData.Months,
            _ => throw new NotImplementedException(nameof(DateLevel))
        };



    }
}