using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using DialogHostAvalonia;
using evelina.ViewModels.Common;
using evelina.ViewModels.Dialogs;
using ReactiveUI;
using System.IO;
using System.Windows.Input;
using VisualTools;

namespace evelina.ViewModels;

public class StartViewModel : WindowViewModelBase
{
    public ICommand CreatePortfolioCommand { get; }
    public ICommand OpenPortfolioCommand { get; }


    public StartViewModel(MainViewModel main) : base(main)
    {
        CreatePortfolioCommand = ReactiveCommand.Create(CreatePortfolio);
        OpenPortfolioCommand = ReactiveCommand.Create(OpenPortfolio);
    }


    private async void CreatePortfolio()
    {
        var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;
        var topLevel = TopLevel.GetTopLevel(mainWindow);
        if (topLevel is null) return;

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select place for Portfolio",
            AllowMultiple = false,
        });

        if (folders.Count == 0)
        {
            return;
        }

        var dialog = new InputDialogViewModel("Input name", "Input name of Portfolio");
        await DialogHost.Show(dialog);

        var name = dialog.Input;

        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        var portfolio = PortfolioFactory.CreatePortfolio(name);

        var path = Path.Combine(folders[0].Path.ToString(), $"{name}.{Constants.DB_EXTENSION}");

        var success = await portfolio.SaveAs(path);

        if (!success)
        {
            return;
        }

        Main.ActiveWindow = new PortfolioViewModel(portfolio, Main);

        portfolio.Logger.Info("test");
    }

    private async void OpenPortfolio()
    {
        var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;
        var topLevel = TopLevel.GetTopLevel(mainWindow);
        if (topLevel is null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Portfolio",
            AllowMultiple = false,
            FileTypeFilter = new[] { Constants.DbFileType },
        });

        if (files.Count == 0)
        {
            return;
        }

        var portfolio = PortfolioFactory.ReadPortfolio(files[0].Path.ToString());

        Main.ActiveWindow = new PortfolioViewModel(portfolio, Main);
    }
}