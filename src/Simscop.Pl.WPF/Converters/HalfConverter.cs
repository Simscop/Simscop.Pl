using System.Globalization;

namespace Simscop.Pl.WPF.Converters;

public class HalfConverter : BaseValueConvert<HalfConverter>
{
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value switch
        {
            double dv => dv / 2,
            _ => throw new NotImplementedException()
        };
}