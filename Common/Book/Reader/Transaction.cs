using System;
using System.Globalization;

namespace BookImpl.Reader;

internal class Transaction
{
    public Type? Type { get; set; }
    public string? Currency { get; set; }
    public double? Amount { get; set; }
    public double? CurrencyRate { get; set; }
    public string? Project { get; set; }
    public string? Category { get; set; }
    public string? ParentCategory { get; set; }
    public string? Account { get; set; }
    public string? PaymentAccount { get; set; }
    public string? ReceivableAccount { get; set; }
    public string? Merchant { get; set; }
    public string? Address { get; set; }
    public DateTime? Datetime { get; set; }
    public string? Tags { get; set; }
    public string? Author { get; set; }
    public string? Note { get; set; }

    internal bool IsValid()
    {
        if (Type is null ||
            Amount is null ||
            Category is null ||
            (Account is null && (PaymentAccount is null || ReceivableAccount is null)) ||
            Datetime is null)
        {
            return false;
        }

        return true;
    }

    internal void SetField(FieldRole role, string s)
    {
        double? d = null;
        double.TryParse(s, out var dd);
        if (!double.IsNaN(dd))
        {
            d = dd;
        }

        switch (role)
        {
            case FieldRole.type:
                Type = SproutsHelper.FindType(s); break;
            case FieldRole.currency:
                Currency = s; break;
            case FieldRole.amount:
                Amount = d; break;
            case FieldRole.currencyRate:
                CurrencyRate = d; break;
            case FieldRole.project:
                Project = s; break;
            case FieldRole.category:
                Category = s; break;
            case FieldRole.parentCategory:
                ParentCategory = s; break;
            case FieldRole.account:
                Account = s; break;
            case FieldRole.paymentAccount:
                PaymentAccount = s; break;
            case FieldRole.receivableAccount:
                ReceivableAccount = s; break;
            case FieldRole.merchant:
                Merchant = s; break;
            case FieldRole.address:
                Address = s; break;
            case FieldRole.datetime:
                //if (DateTime.TryParseExact(s, "yyyy-MM-dd HH:nn", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                if (DateTime.TryParse(s, CultureInfo.InvariantCulture, out var dt))
                {
                    Datetime = dt;
                }
                break;
            case FieldRole.tags:
                Tags = s; break;
            case FieldRole.author:
                Author = s; break;
            case FieldRole.note:
                Note = s; break;
            default:
                throw new NotImplementedException(nameof(role));
        }
    }
}

internal enum FieldRole
{
    type,
    currency,
    amount,
    currencyRate,
    project,
    category,
    parentCategory,
    account,
    paymentAccount,
    receivableAccount,
    merchant,
    address,
    datetime,
    tags,
    author,
    note,
}

internal enum Type
{
    expense,
    income,
    transfer,
}

//internal static class FieldRoleExtension
//{
//    public static bool IsRequired(this FieldRole field)
//        => field switch
//        {
//            FieldRole.type or FieldRole.amount or FieldRole.category or
//                FieldRole.account or FieldRole.paymentAccount or FieldRole.receivableAccount => true,
//            _ => false,
//        };
//}