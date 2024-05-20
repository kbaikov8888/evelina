using Avalonia.Controls;
using Avalonia.Input;
using PortfolioAvalonia.ViewModel;

namespace PortfolioAvalonia.View;

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