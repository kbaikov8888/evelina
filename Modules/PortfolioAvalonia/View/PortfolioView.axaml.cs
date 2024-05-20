using Avalonia.Controls;
using Avalonia.Input;
using evelina.ViewModels;

namespace evelina.Views;

public partial class PortfolioView : UserControl
{
    public PortfolioView()
    {
        InitializeComponent();
    }

    private void Border_DoubleTapped(object sender, TappedEventArgs e)
    {
        if (sender is Control { DataContext: AssetViewModel vm })
        {
            vm.EditCommand.Execute(null);
        }
    }
}