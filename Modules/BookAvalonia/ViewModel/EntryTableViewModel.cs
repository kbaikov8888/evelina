using evelina.Controls;
using System.Collections.Generic;

namespace BookAvalonia.ViewModel;

public class EntryTableViewModel : WindowViewModelBase, IMenuCompatible
{
    public IReadOnlyList<EntryViewModel> Entries { get; }

    internal EntryTableViewModel(IReadOnlyList<EntryViewModel> entries, IMainViewModel main) : base(main)
    {
        Entries = entries;
    }
}