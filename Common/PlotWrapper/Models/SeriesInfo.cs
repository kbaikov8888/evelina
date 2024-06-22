using Avalonia.Media;
using System.Diagnostics.CodeAnalysis;

namespace PlotWrapper.Models;

public record SeriesInfo
{
    public required string Name { get; init; }
    public required Color Color { get; init; }
    public required double[] Values { get; set; }

    public SeriesInfo()
    {
    }

    [SetsRequiredMembers]
    public SeriesInfo(string name, Color color, double[] values)
    {
        Name = name;
        Color = color;
        Values = values;
    }
}