using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Simscop.Pl.WPF.Converters;

public class EnumBoolConverter : BaseValueConvert<EnumBoolConverter>
{
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null || parameter is null) return Binding.DoNothing;

        return (int)value == (int)parameter;
    }

    public override object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null || parameter is null) return Binding.DoNothing;

        return value is true ? parameter : Binding.DoNothing;
    }
}