namespace PortfolioImpl.DTO;

internal class AssetDTO
{
    public string? Name { get; set; }
    public double? TargetVolume { get; set; }
    public double? TargetSellPrice { get; set; }
    public double? TargetShare { get; set; }

    public AssetDTO() { }
}