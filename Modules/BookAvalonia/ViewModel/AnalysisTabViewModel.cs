using Avalonia.Threading;
using BookImpl;
using BookImpl.Elements;
using BookImpl.Enum;
using DynamicData;
using DynamicData.Binding;
using evelina.Controls;
using evelina.Controls.SimpleCheckedList;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using VisualTools;

namespace BookAvalonia.ViewModel;

public class AnalysisTabViewModel : WindowViewModelBase, IMenuCompatible
{
    public string Name { get; }

    public SimpleCheckedListViewModel Settings { get; }

    [Reactive]
    public GraphViewModel? Plot { get; private set; }

    private readonly Book _book;

    private readonly IMainViewModel _main;

    private readonly Dictionary<Category, double[]> _data = new();
    private readonly Dictionary<Category, string> _hexColors = new();
    private double[] _x = Array.Empty<double>();

    private readonly DispatcherTimer _updatePlot = new() { Interval = new TimeSpan(0, 0, 0, 0, 50) };


    public AnalysisTabViewModel(string name, IEnumerable<Category?> categories, Book book, IMainViewModel main) : base(main)
    {
        Name = name;
        _book = book;
        _main = main;

        var items = new List<SimpleCheckedViewModel>();
        int counter = 0;
        foreach (var cat in categories)
        {
            if (cat is null) continue;

            var item = new SimpleCheckedViewModel(cat);
            item.DoubleClickedEvent += ItemOnDoubleClickedEvent;

            items.Add(item);
            _hexColors[cat] = PrettyColors.Hexs[counter++];
        }

        Settings = new SimpleCheckedListViewModel(items);

        _updatePlot.Tick += UpdatePlotOnTick;

        Settings.Items.ToObservableChangeSet()
            .SubscribeMany(x => x.WhenAnyValue(y => y.IsChecked).Subscribe(_ => UpdatePlot()))
            .Subscribe();
    }


    private void ItemOnDoubleClickedEvent(SimpleCheckedViewModel obj)
    {
        if (obj.Source is not Category category) return;

        var categories = _book.AllCategories.Where(x => x.ParentCategory == category);

        var vm = new AnalysisTabViewModel(string.Empty, categories, _book, _main);

        //TODO
        var data = _book.CalculatedData.GetData(DateLevel.Month);
        var dateDoubles = data.Dates.Select(x => x.ToOADate()).ToArray();

        if (category is ExpenseCategory)
        {
            vm.UpdateData(data.ExpenseCategories, dateDoubles);
        }
        else if (category is IncomeCategory)
        {
            vm.UpdateData(data.IncomeCategories, dateDoubles);
        }

        Main.ActiveWindow = vm;
    }

    private void UpdatePlotOnTick(object? sender, EventArgs e)
    {
        _updatePlot.Stop();

        var filtered = new Dictionary<Category, double[]>();

        foreach (var setting in Settings.Items)
        {
            if (!setting.IsChecked) continue;

            var cat = (Category)setting.Source;

            filtered[cat] = _data[cat];
        }

        var plot = GraphPanelViewModel.GetCategorical(filtered, _x);
        Plot = new GraphViewModel(plot);
    }

    public void UpdateData<T>(Dictionary<T, double[]> data, double[] x) where T : Category
    {
        _data.Clear();

        _x = x;

        foreach (var (cat, val) in data)
        {
            _data[cat] = val;
        }

        UpdatePlot();
    }

    private void UpdatePlot()
    {
        _updatePlot.Start();
    }
}
