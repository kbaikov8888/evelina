using evelina.ViewModels.Common;
using ReactiveUI.Fody.Helpers;

namespace evelina.ViewModels.Dialogs;

public class InputDialogViewModel : ViewModelBase
{
    public string Title { get; }

    public string Text { get; }

    [Reactive]
    public string Input { get; set; } = string.Empty;

    public InputDialogViewModel(string title, string text)
    {
        Title = title;
        Text = text;
    }

    public InputDialogViewModel()
    {
        Title = "Title";
        Text = "Text";
    }
}