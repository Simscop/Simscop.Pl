using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace Simscop.Pl.WPF.Converters;

public class ColorBrushConverter : BaseValueConvert<ColorBrushConverter>
{
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value switch
        {
            Color color => new SolidColorBrush(new System.Windows.Media.Color()
            {
                A = color.A,
                B = color.B,
                G = color.G,
                R = color.R
            }),
            SolidColorBrush solid => Color.FromArgb(solid.Color.A, solid.Color.R, solid.Color.G, solid.Color.B),
            _ => throw new NotImplementedException()
        };

    public override object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value switch
        {
            Color color => new SolidColorBrush(new System.Windows.Media.Color()
            {
                A = color.A,
                B = color.B,
                G = color.G,
                R = color.R
            }),
            SolidColorBrush solid => Color.FromArgb(solid.Color.A, solid.Color.R, solid.Color.G, solid.Color.B),
            _ => throw new NotImplementedException()
        };
}