using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace evelina.Styles.Converters;

public class EnumDescriptionConverter : MarkupExtension, IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is Enum val ? GetDescription(val) : AvaloniaProperty.UnsetValue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return AvaloniaProperty.UnsetValue;
    }

    public static string GetDescription(Enum en)
    {
        var type = en.GetType();
        MemberInfo[] memInfo = type.GetMember(en.ToString());
        if (memInfo.Length > 0)
        {
            var attrs = memInfo[0].GetCustomAttribute<DescriptionAttribute>(false);
            if (attrs is { })
            {
                return attrs.Description;
            }
        }
        return en.ToString();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}