using ScottPlot;

namespace BookAvalonia.Model;

public struct SeriesInfo
{
    public string Name { get; }
    public Color Color { get; }

    public SeriesInfo(string name, Color color)
    {
        Name = name;
        Color = color;
    }
}