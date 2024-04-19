using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace Simscop.Pl.Ui.Extensions;

public static class ColorExtension
{
    public static OxyColor ToOxyColor(this Color color)
        => OxyColor.FromArgb(color.A, color.R, color.G, color.B);
}