using evelina.Controls;
using System;
using System.Collections.Generic;
using BookImpl;
using System.Linq;
using System.Windows.Input;
using BookImpl.Elements;
using ReactiveUI;

namespace BookAvalonia.ViewModel;

public class BookViewModel : MainViewModelBase, IDisposable, IMenuCompatible
{
    public ICommand ShowEntryTableCommand { get; }
    private EntryTableViewModel? _entryTable;

    public ICommand ShowGraphPanelCommand { get; }
    private GraphPanelViewModel? _graphPanel;

    public ICommand ShowAnalysisPanelCommand { get; }
    private AnalysisPanelViewModel? _analysisPanel;

    public string Name => _book.Name;

    private readonly IReadOnlyList<EntryViewModel> _entries;
    private readonly Book _book;


    public BookViewModel(Book book)
    {
        _book = book;
        _entries = book.GetEntriesFromLast().Where(x=> x is ExternalEntry).Select(x => new EntryViewModel(x)).ToList();

        ShowEntryTableCommand = ReactiveCommand.Create(ShowEntryTable);
        ShowGraphPanelCommand = ReactiveCommand.Create(ShowGraphPanel);
        ShowAnalysisPanelCommand = ReactiveCommand.Create(ShowAnalysisPanel);

        ShowEntryTable();
    }


    public void Dispose()
    {
    }

    private void ShowEntryTable()
    {
        if (ActiveWindow is EntryTableViewModel)
        {
            return;
        }

        _entryTable ??= new EntryTableViewModel(_entries, this);

        ActiveWindow = _entryTable;
    }


    private void ShowGraphPanel()
    {
        if (ActiveWindow is GraphPanelViewModel)
        {
            return;
        }

        _graphPanel ??= new GraphPanelViewModel(_book, this);

        ActiveWindow = _graphPanel;
    }

    private void ShowAnalysisPanel()
    {
        if (ActiveWindow is AnalysisPanelViewModel)
        {
            return;
        }

        _analysisPanel ??= new AnalysisPanelViewModel(_book, this);

        ActiveWindow = _analysisPanel;
    }

    protected override void Close()
    {
    }
}