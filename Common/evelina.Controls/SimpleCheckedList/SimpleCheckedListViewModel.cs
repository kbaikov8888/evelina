using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;

namespace evelina.Controls.SimpleCheckedList;

public class SimpleCheckedListViewModel : ReactiveObject
{
    public ReadOnlyObservableCollection<SimpleCheckedViewModel> VisibleItems { get; }
    public ObservableCollection<SimpleCheckedViewModel> Items { get; }
    private static ObservableCollection<SimpleCheckedViewModel> CheckAllItem { get; } = new()
    {
        new SimpleCheckedViewModel(new SelectAllItem())
    };

    private bool _internalChangind;


    public SimpleCheckedListViewModel(IEnumerable<SimpleCheckedViewModel> items)
    {
        Items = new ObservableCollection<SimpleCheckedViewModel>(items);

        Items.ToObservableChangeSet().AutoRefresh(model => model.IsEnabled)
            .Filter(model => model.IsEnabled)
            .Sort(new SimpleCheckedViewModelComparer())
            .Bind(out var filtered)
            .Subscribe();
        CheckAllItem.ToObservableChangeSet()
            .Or(filtered.ToObservableChangeSet())
            .Bind(out var all)
            .Subscribe();
        VisibleItems = all;

        CheckAllItem.First().WhenAnyValue(x => x.IsChecked)
            .Subscribe(CheckAll);

        Items.ToObservableChangeSet()
            .SubscribeMany(
                x =>
                    x.WhenAnyValue(y => y.IsChecked)
                        .Subscribe(_ => SomeCheckChanged()))
            .Subscribe();
    }


    private void CheckAll(bool value)
    {
        if (_internalChangind) return;

        foreach (var item in Items)
        {
            item.IsChecked = value;
        }
    }

    private void SomeCheckChanged()
    {
        var allChecked = true;
        foreach (var item in Items)
        {
            allChecked &= item.IsChecked;
        }

        _internalChangind = true;
        CheckAllItem.First().IsChecked = allChecked;
        _internalChangind = false;
    }
}

public class SelectAllItem : INamed
{
    public string Name => "Check All";
}