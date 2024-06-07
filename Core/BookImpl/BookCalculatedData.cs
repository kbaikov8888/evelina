using BookImpl.Enum;
using System;
using System.Collections.Generic;

namespace BookImpl;

public class BookCalculatedData
{
    public BookDatedData Years { get; private set; }
    public BookDatedData Months { get; private set; }

    private readonly Book _book;


    internal BookCalculatedData(Book book)
    {
        _book = book;

        Years = new BookDatedData(DateLevel.Year, Array.Empty<DateTime>(), _book);
        Months = new BookDatedData(DateLevel.Month, Array.Empty<DateTime>(), _book);
    }


    public BookDatedData GetData(DateLevel level)
    {
        return level switch
        {
            DateLevel.Year => _book.CalculatedData.Years,
            DateLevel.Month => _book.CalculatedData.Months,
            _ => throw new NotImplementedException(nameof(DateLevel))
        };
    }

    internal void Calculate()
    {
        Months.Dispose();
        Months = new BookDatedData(DateLevel.Month, GetMonths(), _book);

        Years.Dispose();
        Years = new BookDatedData(DateLevel.Year, GetYears(), _book);
    }

    private DateTime[] GetMonths()
    {
        var res = new List<DateTime>();

        int lastYear = -1;
        int lastMonth = -1;
        foreach (var entry in _book.GetEntriesFromFirst())
        {
            if (entry.DateTime.Year > lastYear || entry.DateTime.Month > lastMonth)
            {
                res.Add(entry.DateTime);
                lastYear = entry.DateTime.Year;
                lastMonth = entry.DateTime.Month;
            }
        }

        return res.ToArray();
    }

    private DateTime[] GetYears()
    {
        var res = new List<DateTime>();

        int lastYear = -1;
        foreach (var entry in _book.GetEntriesFromFirst())
        {
            if (entry.DateTime.Year > lastYear)
            {
                res.Add(entry.DateTime);
                lastYear = entry.DateTime.Year;
            }
        }

        return res.ToArray();
    }
}