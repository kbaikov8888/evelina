using Avalonia.Controls;
using Avalonia.Interactivity;

namespace evelina.Controls.InputDialog;

public partial class InputDialogView : UserControl
{
    public InputDialogView()
    {
        InitializeComponent();
    }

    public void Ok_Click(object sender, RoutedEventArgs args)
    {
    }

    public void Cancel_Click(object sender, RoutedEventArgs args)
    {
        if (DataContext is InputDialogViewModel vm)
        {
            vm.Input = string.Empty;
        }
    }
}