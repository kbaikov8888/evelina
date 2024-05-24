using Avalonia.Media;
using BookImpl;
using BookImpl.Enum;
using ReactiveUI;
using System;

namespace BookAvalonia.ViewModel;

public class EntryViewModel : ReactiveObject
{
    public double Amount => _entry.Amount;
    public DateTime DateTime => _entry.DateTime;
    public Project? Project => _entry.Project;
    public string? Note => _entry.Note;
    public Color Color => _entry.Type.GetColor();

    public Category? Category
    {
        get
        {
            if (_entry is ExternalEntry externalEntry)
            {
                return externalEntry.Category;
            }

            return null;
        }
    }

    private readonly Entry _entry;

    internal EntryViewModel(Entry entry)
    {
        _entry = entry;
    }
}