using Avalonia.Controls;
using Avalonia.Input;
using PortfolioAvalonia.ViewModel;

namespace PortfolioAvalonia.View;

public partial class AssetsPanelView : UserControl
{
    public AssetsPanelView()
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