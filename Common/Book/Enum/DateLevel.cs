using System;

namespace BookImpl.Enum;
internal enum DateLevel
{
    Month,
}

internal static class DateLevelExtension
{
    public static bool IsEqual(this DateLevel level, DateTime d1, DateTime d2)
    {
        if (level == DateLevel.Month)
        {
            return d1.Year == d2.Year && d1.Month == d2.Month;
        }
        else
        {
            throw new NotImplementedException(nameof(DateLevel));
        }
    }
}
