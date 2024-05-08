using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Simscop.Pl.WPF.Converters;

public class EnumVisibleConverter : BaseValueConvert<EnumVisibleConverter>
{
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null || parameter is null) return Binding.DoNothing;

        return (int)value == (int)parameter ? Visibility.Visible : Visibility.Collapsed;
    }
}