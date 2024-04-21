using OxyPlot.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lift.UI.Tools.Extension;
using OxyPlot;

namespace Simscop.Pl.Ui.Charts;

internal class TextAnnotationExt : TextAnnotation
{
    /// <summary>
    /// 左边转换，将像素转成直角坐标系
    /// </summary>
    public new Func<double, double>? Transform { get; set; }

    /// <summary>
    /// 直角坐标系X轴坐标
    /// </summary>
    public double TargetX { get; set; } = double.NaN;

    /// <summary>
    /// 直角坐标系Y轴坐标
    /// </summary>
    public double TargetY { get; set; } = Double.NaN;

    /// <summary>
    /// 偏移X轴像素点距离
    /// </summary>
    public double OffsetX { get; set; } = 30;

    /// <summary>
    /// 偏移Y轴像素点距离
    /// </summary>
    public double OffsetY { get; set; } = 5;

    /// <summary>
    /// 
    /// </summary>
    public string? SerialTitle { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? TitleX { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? TitleY { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? FormatString { get; set; }

    public void Update()
    {
        _ = Transform is not null
            ? TextPosition = new DataPoint(TargetX + Transform(OffsetX), TargetY + Transform(OffsetY))
            : TextPosition = TextPosition;

        if (FormatString is not null)
            Text = FormatString?.Format(SerialTitle ?? "", TitleX ?? "", TargetX, TitleY ?? "", TargetY);
    }

}