using Avalonia.Threading;
using BookImpl.Elements;
using DynamicData;
using DynamicData.Binding;
using evelina.Controls.SimpleCheckedList;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using VisualTools;

namespace BookAvalonia.ViewModel;

public class AnalysisTabViewModel : ReactiveObject
{
    public string Name { get; }

    public SimpleCheckedListViewModel Settings { get; }

    [Reactive]
    public GraphViewModel? Plot { get; private set; }

    private readonly Dictionary<Category, double[]> _data = new();
    private readonly Dictionary<Category, string> _hexColors = new();
    private double[] _x = Array.Empty<double>();

    private readonly DispatcherTimer _updatePlot = new() { Interval = new TimeSpan(0, 0, 0, 0, 50) };


    public AnalysisTabViewModel(string name, IEnumerable<Category?> categories)
    {
        Name = name;

        var items = new List<SimpleCheckedViewModel>();
        int counter = 0;
        foreach (var cat in categories)
        {
            if (cat is null) continue;

            items.Add(new SimpleCheckedViewModel(cat));
            _hexColors[cat] = PrettyColors.Hexs[counter++];
        }

        Settings = new SimpleCheckedListViewModel(items);

        _updatePlot.Tick += UpdatePlotOnTick;

        Settings.Items.ToObservableChangeSet()
            .SubscribeMany(x => x.WhenAnyValue(y => y.IsChecked).Subscribe(_ => UpdatePlot()))
            .Subscribe();
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
