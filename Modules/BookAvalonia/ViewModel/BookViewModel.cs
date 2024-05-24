using evelina.Controls;
using System;
using System.Collections.Generic;
using BookImpl;
using System.Linq;
using System.Windows.Input;
using ReactiveUI;

namespace BookAvalonia.ViewModel;

public class BookViewModel : MainViewModelBase, IDisposable, IMenuCompatible
{
    public ICommand ShowEntryTableCommand { get; }

    public string Name => _book.Name;

    private readonly IReadOnlyList<EntryViewModel> _entries;

    private readonly Book _book;


    public BookViewModel(Book book)
    {
        _book = book;
        _entries = book.GetEntries().Select(x => new EntryViewModel(x)).ToList();

        ShowEntryTableCommand = ReactiveCommand.Create(ShowEntryTable);

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

        ActiveWindow = new EntryTableViewModel(_entries, this);
    }

    protected override void Close()
    {
    }
}