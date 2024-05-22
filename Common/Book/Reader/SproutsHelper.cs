using System;
using System.Collections.Generic;

namespace BookImpl.Reader;

internal static class SproutsHelper
{
    private static readonly Dictionary<FieldRole, string> _headers = new()
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
        { FieldRole.receivableAccount, "Account receivable" },
        { FieldRole.merchant, "Merchant" },
        { FieldRole.address, "Address" },
        { FieldRole.datetime, "Date time" },
        { FieldRole.tags, "Tags" },
        { FieldRole.author, "Author" },
        { FieldRole.note, "Note" },
    };

    private static readonly Dictionary<Type, string> _types = new()
    {
        { Type.expense, "Expense" },
        { Type.income, "Income" },
        { Type.transfer, "Transfer" },
    };

    internal static FieldRole? FindFieldRole(string field)
    {
        foreach ((var role, var key) in _headers)
        {
            if (string.Equals(key, field, StringComparison.OrdinalIgnoreCase))
            {
                return role;
            }
        }

        return null;
    }

    internal static Type? FindType(string type)
    {
        foreach ((var role, var key) in _types)
        {
            if (string.Equals(key, type, StringComparison.OrdinalIgnoreCase))
            {
                return role;
            }
        }

        return null;
    }
}