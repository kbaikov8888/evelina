namespace BookImpl.Reader;

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
    receivavleAccount,
    merchant,
    address,
    datetime,
    tags,
    author,
    note,
}

internal static class FieldRole_Extension
{
    public static bool IsRequired(this FieldRole field)
        => field switch
        {
            FieldRole.type or FieldRole.amount or FieldRole.category or
                FieldRole.account or FieldRole.paymentAccount or FieldRole.receivavleAccount => true,
            _ => false,
        };
}