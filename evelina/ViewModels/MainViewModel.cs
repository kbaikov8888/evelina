using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Windows.Input;
using evelina.ViewModels.Common;

namespace evelina.ViewModels;

public class MainViewModel : ViewModelBase, IMainViewModel
{
    public ICommand TriggerPaneCommand { get; }

    private WindowViewModelBase? _activeWindow;
    public WindowViewModelBase? ActiveWindow
    {
        get => _activeWindow;
        set
        {
            this.RaiseAndSetIfChanged(ref _activeWindow, value);
            this.RaisePropertyChanged(nameof(ShowMenu));

            if (value is PortfolioViewModel p)
            {
                CurrentPortfolio = p;
            }
        }
    }

    [Reactive]
    public bool IsPaneOpen { get; set; }

    public bool ShowMenu => ActiveWindow is IMenuCompatible;

    [Reactive]
    public PortfolioViewModel? CurrentPortfolio { get; set; }


    public MainViewModel()
    {
        ActiveWindow = new StartViewModel(this);
        TriggerPaneCommand = ReactiveCommand.Create(TriggerPane);
    }


    private void TriggerPane()
    {
        IsPaneOpen = !IsPaneOpen;
    }
}
