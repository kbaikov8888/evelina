using BookImpl;
using BookImpl.Enum;
using evelina.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using BookImpl.Elements;

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

    public AnalysisTabViewModel Incomes { get; } 

    public AnalysisTabViewModel Expenses { get; }

    private readonly Book _book;
    

    public AnalysisPanelViewModel(Book book, IMainViewModel main) : base(main)
    {
        _book = book;

        Incomes = new AnalysisTabViewModel("Incomes", _book.ParentIncomeCategories);
        Expenses = new AnalysisTabViewModel("Expenses", _book.ParentExpenseCategories);

        RefreshTabs();
    }


    private void RefreshTabs()
    {
        var data = _book.CalculatedData.GetData(SelectedDateLevel);

        var dateDoubles = data.Dates.Select(x => x.ToOADate()).ToArray();

        Incomes.UpdateData(data.ParentIncomeCategories, dateDoubles);
        Expenses.UpdateData(data.ParentExpenseCategories, dateDoubles);
    }
}