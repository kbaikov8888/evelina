using System;
using System.Collections.Generic;

namespace BookImpl.Reader;

internal static class SproutsHeaders
{
    private static readonly Dictionary<FieldRole, string> _keys = new()
    {
        { FieldRole.type, "Type" },
        { FieldRole.currency, "Currency" },
        { FieldRole.amount, "Amount" },
        { FieldRole.currencyRate, "Currency rate (Relative standard currency)" },
        { FieldRole.project, "Project" },
        { FieldRole.category, "Category" },
        { FieldRole.parentCategory, "Parent Category" },
        { FieldRole.account, "Account" },
        { FieldRole.paymentAccount, "Payment account" },
        { FieldRole.receivavleAccount, "Account receivable" },
        { FieldRole.merchant, "Merchant" },
        { FieldRole.address, "Address" },
        { FieldRole.datetime, "Date time" },
        { FieldRole.tags, "Tags" },
        { FieldRole.author, "Author" },
        { FieldRole.note, "Note" },
    };

    internal static FieldRole? FindFieldRole(string field)
    {
        foreach ((var role, var key) in _keys)
        {
            if (string.Equals(key, field, StringComparison.OrdinalIgnoreCase))
            {
                return role;
            }
        }

        return null;
    }
}