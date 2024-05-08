using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simscop.Pl.WPF.Converters;

public class BoolReverseConverter : BaseValueConvert<BoolReverseConverter>
{
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value switch
        {
            bool bv => !bv,
            _ => throw new NotImplementedException(),
        };

    public override object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value switch
        {
            bool bv => !bv,
            _ => throw new NotImplementedException(),
        };
}