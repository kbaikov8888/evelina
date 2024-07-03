using BookImpl;
using BookImpl.Elements;
using BookImpl.Enum;
using evelina.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookAvalonia.ViewModel;

public class AnalysisPanelViewModel : WindowViewModelBase, IMenuCompatible
{
    public static IEnumerable<DateLevel> DateLevels { get; } = Enum.GetValues(typeof(DateLevel)).Cast<DateLevel>();

    private DateLevel _selectedDateLevel = DateLevel.Month;
    public DateLevel SelectedDateLevel
    {
        get => _selectedDateLevel;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedDateLevel, value);
            UpdateData();
        }
    }

    public static IEnumerable<EntryType> EntryTypes { get; } = new List<EntryType> { EntryType.Expense, EntryType.Income };

    private EntryType _selectedEntryType = EntryType.Expense;
    public EntryType SelectedEntryType
    {
        get => _selectedEntryType;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedEntryType, value);
            UpdateData();
        }
    }

    public CategoryAnalysisViewModel CategoryAnalysis { get; } 

    private readonly Book _book;
    

    public AnalysisPanelViewModel(Book book, IMainViewModel main) : base(main)
    {
        _book = book;

        CategoryAnalysis = new CategoryAnalysisViewModel();
        CategoryAnalysis.GoBackEvent += UpdateData;
        CategoryAnalysis.CategoryChoosedEvent += CategoryChoosed;

        UpdateData();
    }


    private void UpdateData()
    {
        var data = _book.CalculatedData.GetData(SelectedDateLevel);
        var dateDoubles = data.Dates.Select(x => x.ToOADate()).ToArray();

        Dictionary<Category, double[]> vals;

        if (SelectedEntryType == EntryType.Expense)
        {
            vals = data.ParentExpenseCategories.ToDictionary(x => x.Key as Category, x => x.Value);
        }
        else if (SelectedEntryType == EntryType.Income)
        {
            vals = data.ParentIncomeCategories.ToDictionary(x => x.Key as Category, x => x.Value);
        }
        else
        {
            throw new NotImplementedException();
        }

        CategoryAnalysis.UpdateData(vals, dateDoubles, false);
    }

    private void CategoryChoosed(Category category)
    {
        var data = _book.CalculatedData.GetData(SelectedDateLevel);
        var dateDoubles = data.Dates.Select(x => x.ToOADate()).ToArray();

        var categories = _book.AllCategories.Where(x => x.ParentCategory == category);

        var vals = new Dictionary<Category, double[]>();

        if (category is ExpenseCategory)
        {
            foreach (var (cat, val) in data.ExpenseCategories)
            {
                if (categories.Contains(cat))
                {
                    vals[cat] = val;
                }
            }
        }
        else if (category is IncomeCategory)
        {
            foreach (var (cat, val) in data.IncomeCategories)
            {
                if (categories.Contains(cat))
                {
                    vals[cat] = val;
                }
            }
        }

        if (vals.Count == 0) return;

        CategoryAnalysis.UpdateData(vals, dateDoubles, true);
    }
}