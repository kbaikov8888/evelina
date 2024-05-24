using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CTS.Import;
using evelina.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using PortfolioInterface;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using VisualTools;

namespace PortfolioAvalonia.ViewModel;

public class PortfolioViewModel : MainViewModelBase, IDisposable, IMenuCompatible
{
    public ICommand EditCommand { get; }
    public ICommand CreateAssetCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand ImportCommand { get; }
    public ICommand ExportCommand { get; }
    public ICommand ShowTableCommand { get; }
    public ICommand ShowAssetsCommand { get; }

    public string Name => Model.Name;
    public double? Volume => Model.Stat.Volume;

    [Reactive]
    public AssetViewModel? SelectedAsset { get; set; }

    public ObservableCollection<AssetViewModel> Assets { get; private set; } = new();

    internal IPortfolio Model { get; }


    public PortfolioViewModel(IPortfolio model)
    {
        Model = model;
        Model.UpdateVisualStatEvent += Model_UpdateVisualStatEvent;

        foreach (var existed in model.GetAssets())
        {
            AddAsset(existed);
        }
        RefreshAssets();
        SelectedAsset = Assets.FirstOrDefault();

        SaveCommand = ReactiveCommand.Create(Save);
        EditCommand = ReactiveCommand.Create(EditPortfoliInfo);
        CreateAssetCommand = ReactiveCommand.Create(CreateAsset);
        ImportCommand = ReactiveCommand.Create(Import);
        ShowTableCommand = ReactiveCommand.Create(ShowTable);
        ShowAssetsCommand = ReactiveCommand.Create(ShowAssets);
        ExportCommand = ReactiveCommand.Create(() => { });

        ShowTable();
    }


    public void Dispose()
    {
        foreach (var vm in Assets)
        {
            vm.DeleteMeEvent -= DeleteAsset;
            vm.EditMeEvent -= EditAsset;
            vm.Dispose();
        }
        Assets.Clear();

        Model.UpdateVisualStatEvent -= Model_UpdateVisualStatEvent;
    }


    private void Model_UpdateVisualStatEvent()
    {
        this.RaisePropertyChanged(nameof(Volume));

        foreach (var asset in Assets)
        {
            //TODO how rework? attrs?
            asset.RaisePropertyChanged(nameof(AssetViewModel.Volume));
            asset.RaisePropertyChanged(nameof(AssetViewModel.SellPrice));
            asset.RaisePropertyChanged(nameof(AssetViewModel.Share));
            asset.RaisePropertyChanged(nameof(AssetViewModel.BuyedVolume));
            asset.RaisePropertyChanged(nameof(AssetViewModel.BuyedShare));
            asset.RaisePropertyChanged(nameof(AssetViewModel.Status));
            asset.RaisePropertyChanged(nameof(AssetViewModel.TargetVolume));
            asset.RaisePropertyChanged(nameof(AssetViewModel.TargetSellPrice));
            asset.RaisePropertyChanged(nameof(AssetViewModel.TargetShare));
        }
    }

    protected override void Close()
    {
        Save();
    }

    private void Save()
    {
        Model.Save();
    }

    private void EditPortfoliInfo()
    {
        ActiveWindow = new PortfolioEditingViewModel(Model, this);
    }

    internal void AddAsset(IAsset asset)
    {
        if (Assets.Any(x => x.Model == asset))
        {
            return;
        }

        var vm = new AssetViewModel(asset, this);
        vm.DeleteMeEvent += DeleteAsset;
        vm.EditMeEvent += EditAsset;

        Assets.Add(vm);
    }

    private void EditAsset(AssetViewModel vm)
    {
        ActiveWindow = new AssetEditingViewModel(this, vm.Model);
    }

    private void CreateAsset()
    {
        ActiveWindow = new AssetEditingViewModel(this);
    }

    private async void DeleteAsset(AssetViewModel vm)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(
            "Deleting",
            $"Are you sure to delete {vm.Name}",
            ButtonEnum.YesNo);

        var res = await box.ShowAsync();

        if (res != ButtonResult.Yes)
        {
            return;
        }

        Model.DeleteAsset(vm.Model);

        vm.DeleteMeEvent -= DeleteAsset;
        vm.EditMeEvent -= EditAsset;
        Assets.Remove(vm);
    }

    private async void Import()
    {
        var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;

        var topLevel = TopLevel.GetTopLevel(mainWindow);
        if (topLevel is null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select CSV file with CTS",
            AllowMultiple = false,
            FileTypeFilter = new[] { Constants.CSVFileType },
        });

        if (files.Count == 0)
        {
            return;
        }

        try
        {
            using (var importer = new CTSImporter(files[0].Path.AbsolutePath))
            {
                importer.Read();
                importer.AddToPortfolio(Model);
            }
        }
        catch (Exception ex)
        {
            //TODO
            return;
        }

        foreach (var existed in Model.GetAssets())
        {
            AddAsset(existed);
        }

        RefreshAssets();
        SelectedAsset ??= Assets.FirstOrDefault();
    }

    internal void RefreshAssets()
    {
        Assets = new ObservableCollection<AssetViewModel>(Assets.OrderBy(x => x.Name));
    }

    private void ShowTable()
    {
        if (ActiveWindow is AssetsTableViewModel)
        {
            return;
        }

        ActiveWindow = new AssetsTableViewModel(this);
    }

    private void ShowAssets()
    {
        if (ActiveWindow is AssetsPanelViewModel)
        {
            return;
        }

        ActiveWindow = new AssetsPanelViewModel(this);
    }
}