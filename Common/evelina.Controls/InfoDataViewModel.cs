namespace evelina.Controls;

public class InfoDataViewModel
{
    public string Header { get; }
    public string Value { get; }

    public InfoDataViewModel(string header, object? value)
    {
        Header = header;
        Value = value?.ToString() ?? "NULL";
    }

    public InfoDataViewModel(string header, double value)
    {
        Header = header;
        Value = value.ToString("N2");
    }
}