using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using BookAvalonia.ViewModel;
using BookImpl.Reader;
using DialogHostAvalonia;
using evelina.Controls.InputDialog;
using PortfolioAvalonia.ViewModel;
using PortfolioImpl;
using PortfolioInterface;
using ReactiveUI;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using BookImpl.Generator;
using ReactiveUI.Fody.Helpers;
using VisualTools;

namespace evelina.ViewModels;

public class StartViewModel : ReactiveObject
{
    internal event Action<object?>? SetNewModel;

    public ICommand CreatePortfolioCommand { get; }
    public ICommand OpenPortfolioCommand { get; }

    public ICommand ReadSproutsCommand { get; }
    public ICommand GenerateDemoBookCommand { get; }

    [Reactive]
    public bool DemoMode { get; set; }


    internal StartViewModel()
    {
        CreatePortfolioCommand = ReactiveCommand.Create(CreatePortfolio);
        OpenPortfolioCommand = ReactiveCommand.Create(OpenPortfolio);
        ReadSproutsCommand = ReactiveCommand.Create(ReadSprouts);
        GenerateDemoBookCommand = ReactiveCommand.Create(GenerateDemoBook);
    }


    private async Task CreatePortfolio()
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

        SetNewModel?.Invoke(new PortfolioViewModel(portfolio));

        portfolio.Logger.Info("test");
    }

    private async Task OpenPortfolio()
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

        var portfolio = PortfolioFactory.ReadPortfolio(files[0].Path.LocalPath);
        if (portfolio is null)
        {
            return;
        }

        SetNewModel?.Invoke(new PortfolioViewModel(portfolio));
    }

    private async Task ReadSprouts()
    {
        var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;
        var topLevel = TopLevel.GetTopLevel(mainWindow);
        if (topLevel is null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Sprouts",
            AllowMultiple = false,
            FileTypeFilter = new[] { Constants.CSVFileType },
        });

        if (files.Count == 0)
        {
            return;
        }

        using (var reader = new SproutsReader(DemoMode))
        {
            var book = reader.TryRead(files[0].Path.LocalPath);
            if (book is null) return;

            SetNewModel?.Invoke(new BookViewModel(book));
        }
    }

    private void GenerateDemoBook()
    {
        using (var generator = new BookDemoGenerator(1234, new GeneratorParameters()))
        {
            var book = generator.Generate();

            SetNewModel?.Invoke(new BookViewModel(book));
        }
    }
}