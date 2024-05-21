using MsBox.Avalonia;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Windows.Input;
using PortfolioInterface;
using evelina.Controls;

namespace PortfolioAvalonia.ViewModel;

public class AssetEditingViewModel : WindowViewModelBase, IDisposable
{
    public ICommand ApplyCommand { get; }
    public ICommand CancelCommand { get; }

    [Reactive]
    public string Name { get; set; } = string.Empty;

    [Reactive]
    public double? TargetVolume { get; set; }

    [Reactive]
    public double? TargetSellPrice { get; set; }

    [Reactive]
    public double? TargetShare { get; set; }

    private readonly PortfolioViewModel _portfolio;
    private IAsset? _asset;


    internal AssetEditingViewModel(PortfolioViewModel portfolio, IAsset? asset = null) : base(portfolio)
    {
        _asset = asset;
        _portfolio = portfolio;

        if (asset is not null)
        {
            Name = asset.Name;
            TargetVolume = asset.TargetVolume;
            TargetSellPrice = asset.TargetSellPrice;
            TargetShare = asset.TargetShare;
        }

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