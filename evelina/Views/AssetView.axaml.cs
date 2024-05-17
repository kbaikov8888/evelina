using Avalonia.Controls;
using Avalonia.Input;
using evelina.ViewModels;

namespace evelina.Views;

public partial class AssetView : UserControl
{
    public AssetView()
    {
        InitializeComponent();
    }

    private void Border_DoubleTapped(object sender, TappedEventArgs e)
    {
        if (sender is Control { DataContext: TransactionViewModel vm })
        {
            vm.EditCommand.Execute(null);
        }
    }
}