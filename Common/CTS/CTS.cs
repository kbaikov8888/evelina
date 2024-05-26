using System;
using System.Globalization;

namespace CTS;

/// <summary>
/// Common Transaction Standard
/// </summary>
public class CTS
{
    public string? Id { get; set; } // Not required
    public string Symbol { get; set; } = string.Empty;
    public double Price { get; set; }
    public double Amount { get; set; }
    public long Datetime { get; set; }
    public EType Type { get; set; }
    public double Tax { get; set; } // Not required
    public double Fee { get; set; } // Not required
    public string? Currency { get; set; } // Not required

    internal void SetField(ECTSFields field, string value)
    {
        double.TryParse(value, CultureInfo.InvariantCulture, out var d);

        switch (field)
        {
            case ECTSFields.id:
                Id = value; break;
            case ECTSFields.symbol:
                Symbol = value; break;
            case ECTSFields.price:
                Price = d; break;
            case ECTSFields.amount:
                Amount = d; break;
            case ECTSFields.type:
                if (!Enum.TryParse<EType>(value.ToLower(), out var e))
                {
                    throw new Exception($"wrong TYPE format: {value}");
                }
                Type = e;
                break;
            case ECTSFields.tax:
                Tax = d; break;
            case ECTSFields.fee:
                Fee = d; break;
            case ECTSFields.currency:
                Currency = value; break;
            case ECTSFields.datetime:
                if (!long.TryParse(value, CultureInfo.InvariantCulture, out var l))
                {
                    if(!DateTime.TryParse(value, out var dt))
                    {
                        throw new Exception($"Worng DATETIME format: {value}");
                    }

                    l = dt.Ticks;
                }
                Datetime = l;
                break;
        }
    }
}

public enum ECTSFields
{
    id,
    symbol,
    price,
    amount,
    datetime,
    type,
    tax,
    fee,
    currency,
}

public enum EType
{
    buy,
    sell,
}

internal static class CTS_Extension
{
    public static bool IsRequired(this ECTSFields field)
        => field switch
        {
            ECTSFields.id => false,
            ECTSFields.symbol => true,
            ECTSFields.price => true,
            ECTSFields.amount => true,
            ECTSFields.datetime => true,
            ECTSFields.type => true,
            ECTSFields.tax => false,
            ECTSFields.fee => false,
            ECTSFields.currency => false,
            _ => throw new NotImplementedException(),
        };
}