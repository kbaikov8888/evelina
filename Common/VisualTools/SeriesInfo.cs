using ScottPlot;

namespace VisualTools;

public class SeriesInfo
{
    public required string Name { get; init; }
    public required Color Color { get; init; }
    public required double[] Values { get; set; }
}