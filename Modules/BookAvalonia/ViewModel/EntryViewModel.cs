﻿using Avalonia.Media;
using BookImpl.Elements;
using BookImpl.Enum;
using ReactiveUI;
using System;

namespace BookAvalonia.ViewModel;

public class EntryViewModel : ReactiveObject
{
    public double Amount => _entry.Amount;
    public DateTime DateTime => _entry.DateTime;
    public string? ProjectName => _entry.Project?.Name;
    public string? Note => _entry.Note;
    public string? Type => _entry.Type.ToString();
    public Color Color => _entry.Type.GetColor();

    public string? CategoryName
    {
        get
        {
            if (_entry is ExternalEntry externalEntry)
            {
                return externalEntry.Category.Name;
            }

            return null;
        }
    }
    
    public string? ParentCategoryName
    {
        get
        {
            if (_entry is ExternalEntry externalEntry)
            {
                return externalEntry.Category.ParentCategory?.Name;
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