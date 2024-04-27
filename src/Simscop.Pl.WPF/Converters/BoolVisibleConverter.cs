using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Simscop.Pl.WPF.Converters;

public class BoolVisibleConverter : BaseValueConvert<BoolVisibleConverter>
{
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value switch
        {
            bool bv => bv ? Visibility.Visible : Visibility.Collapsed,
            Visibility vv => vv == Visibility.Visible,
            _ => throw new NotImplementedException()
        };

    public override object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value switch
        {
            bool bv => bv ? Visibility.Visible : Visibility.Collapsed,
            Visibility vv => vv == Visibility.Visible,
            _ => throw new NotImplementedException()
        };
}