using Avalonia.Media;
using Avalonia.Threading;
using BookImpl.Elements;
using DynamicData;
using DynamicData.Binding;
using evelina.Controls;
using evelina.Controls.SimpleCheckedList;
using PlotWrapper.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows.Input;
using VisualTools;

namespace BookAvalonia.ViewModel;

public class CategoryAnalysisViewModel : ReactiveObject, IMenuCompatible
{
    public event Action<CategoryAnalysisViewModel>? GoBackEvent;
    public event Action<CategoryAnalysisViewModel, Category>? CategoryChoosedEvent;
    
    public ICommand GoBackCommand { get; }

    [Reactive]
    public bool ShowBackButton { get; private set; }

    public SimpleCheckedListViewModel Settings { get; }

    [Reactive]
    public IPlot? Plot { get; private set; }

    private readonly Dictionary<Category, double[]> _data = new();
    private readonly Dictionary<Category, Color> _colors = new();
    private double[] _x = Array.Empty<double>();

    private readonly DispatcherTimer _updatePlot = new() { Interval = new TimeSpan(0, 0, 0, 0, 50) };


    public CategoryAnalysisViewModel()
    {
        GoBackCommand = ReactiveCommand.Create(() => GoBackEvent?.Invoke(this));

        _updatePlot.Tick += UpdatePlotOnTick;

        Settings = new SimpleCheckedListViewModel();
        Settings.Items.ToObservableChangeSet()
            .SubscribeMany(x => x.WhenAnyValue(y => y.IsChecked).Subscribe(_ => UpdatePlot()))
            .Subscribe();
    }


    private void ItemOnDoubleClickedEvent(SimpleCheckedViewModel obj)
    {
        if (obj.Source is not Category category) return;

        CategoryChoosedEvent?.Invoke(this, category);
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

        //TODO _hexColors

        Plot = GraphPanelViewModel.GetCategorical(filtered, _x);
    }

    public void UpdateData(Dictionary<Category, double[]> data, double[] x, bool showBackButton)
    {
        ShowBackButton = showBackButton;

        Settings.Items.Clear();
        _colors.Clear();
        _data.Clear();
        _x = x;

        var categories = data.Keys;

        int counter = 0;
        foreach (var cat in categories)
        {
            var item = new SimpleCheckedViewModel(cat);
            item.DoubleClickedEvent += ItemOnDoubleClickedEvent;

            Settings.Items.Add(item);
            _colors[cat] = PrettyColors.Get(counter++);
        }

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
