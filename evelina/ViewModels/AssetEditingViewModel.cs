using evelina.ViewModels.Common;
using MsBox.Avalonia;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Windows.Input;
using PortfolioInterface;

namespace evelina.ViewModels;

public class AssetEditingViewModel : WindowViewModelBase, IDisposable
{
    public ICommand ApplyCommand { get; }
    public ICommand CancelCommand { get; }

    [Reactive]
    public string Name { get; set; }

    [Reactive]
    public double? TargetVolume { get; set; }

    [Reactive]
    public double? TargetSellPrice { get; set; }

    [Reactive]
    public double? TargetShare { get; set; }

    private readonly PortfolioViewModel _portfolio;
    private IAsset? _asset;


    public AssetEditingViewModel(PortfolioViewModel vm, IAsset asset, MainViewModel main) : this(main)
    {
        _asset = asset;
        _portfolio = vm;

        Name = asset.Name;
        TargetVolume = asset.TargetVolume;
        TargetSellPrice = asset.TargetSellPrice;
        TargetShare = asset.TargetShare;
    }

    public AssetEditingViewModel(PortfolioViewModel vm, MainViewModel main) : this(main)
    {
        _asset = null;
        _portfolio = vm;
    }

    private AssetEditingViewModel(MainViewModel main) : base(main)
    {
        ApplyCommand = ReactiveCommand.Create(Apply);
        CancelCommand = ReactiveCommand.Create(Close);
    }


    public void Dispose()
    {
    }

    private async void Apply()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Warning", "Fill Name!");

            await box.ShowAsync();
            return;
        }

        if (_asset is null)
        {
            var asset = _portfolio.Model.CreateAsset(Name);
            asset.TargetVolume = TargetVolume;
            asset.TargetSellPrice = TargetSellPrice;
            asset.TargetShare = TargetShare;

            _portfolio.AddAsset(asset);
        }
        else
        {
            _asset.Name = Name;
            _asset.TargetVolume = TargetVolume;
            _asset.TargetSellPrice = TargetSellPrice;
            _asset.TargetShare = TargetShare;
        }

        _portfolio.RefreshAssets();

        Close();
    }

    private void Close()
    {
        TurnBack();
    }
}