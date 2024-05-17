namespace PortfolioInterface;

public delegate void UpdateVisualStat();

public interface IPortfolio : IItem
{
    event UpdateVisualStat UpdateVisualStatEvent;
    const double POSSIBLE_DELTA = 1;

    NLog.Logger Logger { get; }

    #region required
    string Name { get; set; }
    #endregion

    #region additional
    string Description { get; set; }
    #endregion

    IPortfolioStat Stat { get; }

    IAsset CreateAsset(string assetName);
    IList<IAsset> GetAssets();
    void DeleteAsset(IAsset asset);
    IAsset? GetAsset(string assetName);

    Task<bool> Save();
    Task<bool> SaveAs(string path);
}