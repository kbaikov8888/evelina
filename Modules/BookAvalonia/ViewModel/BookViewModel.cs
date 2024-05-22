using evelina.Controls;
using System;
using BookImpl;

namespace BookAvalonia.ViewModel;

public class BookViewModel : MainViewModelBase, IDisposable, IMenuCompatible
{
    public string Name => Model.Name;

    internal readonly Book Model;


    public BookViewModel(Book model)
    {
        Model = model;
    }


    public void Dispose()
    {
    }

    protected override void Close()
    {

    }
}