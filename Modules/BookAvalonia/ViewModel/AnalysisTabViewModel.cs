using BookImpl.Elements;
using evelina.Controls;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DynamicData.Binding;
using VisualTools;

namespace BookAvalonia.ViewModel;

public class AnalysisTabViewModel<T> : ReactiveObject where T : Category
{
    public string Name { get; }

    public ICommand CheckAllCommand { get; }
    public ICommand UncheckAllCommand { get; }
    public ObservableCollection<SimpleCheckedViewModel> Settings { get; } = new();

    public GraphViewModel Plot { get; }

    private readonly Dictionary<T, double[]> _data;
    private readonly Dictionary<T, string> _hexColors = new();


    public AnalysisTabViewModel(string name, Dictionary<T, double[]> data)
    {
        Name = name;
        _data = data;

        int counter = 0;
        foreach (var cat in data.Keys)
        {
            _hexColors[cat] = PrettyColors.Hexs[counter++];
        }

        CheckAllCommand = ReactiveCommand.Create(CheckAll);
        UncheckAllCommand = ReactiveCommand.Create(UncheckAll);

        foreach (var cat in data.Keys)
        {
            Settings.Add(new SimpleCheckedViewModel(cat));
        }

        CheckAll();

        UpdatePlot();
    }


    private void CheckAll()
    {
        foreach (var setting in Settings)
        {
            setting.IsChecked = true;
        }
    }

    private void UncheckAll()
    {
        foreach (var setting in Settings)
        {
            setting.IsChecked = false;
        }
    }

    private void UpdatePlot()
    {



    }
}