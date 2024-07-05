using BookImpl;
using evelina.Controls;
using System.Collections.Generic;
using Tools;

namespace BookAvalonia.ViewModel;

public class InfoPanelViewModel : WindowViewModelBase, IMenuCompatible
{
    public List<InfoDataViewModel> BookStatData { get; }


    public InfoPanelViewModel(Book book, IMainViewModel main) : base(main)
    {
        BookStatData = GetBookStatData(book.TotalStat);
    }

    private List<InfoDataViewModel> GetBookStatData(BookStat stat)
    {
        var infos = stat.GetInfos();

        var res = new List<InfoDataViewModel>();

        foreach (var (description, value) in infos)
        {
            if (value is double d)
            {
                res.Add(new InfoDataViewModel(description, d));
            }
            else
            {
                res.Add(new InfoDataViewModel(description, value));
            }
        }

        return res;
    }
}