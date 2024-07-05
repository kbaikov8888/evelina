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
            UpdateData(ExpenseAnalysis);
            UpdateData(IncomeAnalysis);
        }
    }

    public static IEnumerable<EntryType> EntryTypes { get; } = new List<EntryType> { EntryType.Expense, EntryType.Income };

    public CategoryAnalysisViewModel ExpenseAnalysis { get; } 
    public CategoryAnalysisViewModel IncomeAnalysis { get; } 

    private readonly Book _book;
    

    public AnalysisPanelViewModel(Book book, IMainViewModel main) : base(main)
    {
        _book = book;

        ExpenseAnalysis = new CategoryAnalysisViewModel();
        ExpenseAnalysis.GoBackEvent += UpdateData;
        ExpenseAnalysis.CategoryChoosedEvent += CategoryChoosed;

        IncomeAnalysis = new CategoryAnalysisViewModel();
        IncomeAnalysis.GoBackEvent += UpdateData;
        IncomeAnalysis.CategoryChoosedEvent += CategoryChoosed;

        UpdateData(ExpenseAnalysis);
        UpdateData(IncomeAnalysis);
    }


    private void UpdateData(CategoryAnalysisViewModel sender)
    {
        var data = _book.CalculatedData.GetData(SelectedDateLevel);
        var dateDoubles = data.Dates.Select(x => x.ToOADate()).ToArray();

        Dictionary<Category, double[]> vals;

        if (sender == ExpenseAnalysis)
        {
            vals = data.ParentExpenseCategories.ToDictionary(x => x.Key as Category, x => x.Value);
        }
        else if (sender == IncomeAnalysis)
        {
            vals = data.ParentIncomeCategories.ToDictionary(x => x.Key as Category, x => x.Value);
        }
        else
        {
            throw new NotImplementedException();
        }

        sender.UpdateData(vals, dateDoubles, false);
    }

    private void CategoryChoosed(CategoryAnalysisViewModel sender, Category category)
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

        sender.UpdateData(vals, dateDoubles, true);
    }
}