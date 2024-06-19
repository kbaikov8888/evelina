using Avalonia.Media;
using System;

namespace BookImpl.Enum;
public enum EntryType
{
    Expense,
    Income,
    Transfer,
    Invest,
    ReInvest,
}

public static class EntryType_Extension
{
    public static Color GetColor(this EntryType type)
        => type switch
        {
            EntryType.Expense => Colors.IndianRed,
            EntryType.Income => Colors.MediumSeaGreen,
            EntryType.Transfer => Colors.CornflowerBlue,
            EntryType.Invest => Colors.Gold,
            EntryType.ReInvest => Colors.DarkOrange,
            _ => throw new NotImplementedException(nameof(GetColor)),
        };
}