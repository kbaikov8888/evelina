using System.Collections.Generic;

namespace Tools;

public static class Array_Extension
{
    public static IEnumerable<double> CumulativeSum(this IEnumerable<double> sequence)
    {
        double sum = 0;
        foreach (var item in sequence)
        {
            sum += item;
            yield return sum;
        }
    }
}