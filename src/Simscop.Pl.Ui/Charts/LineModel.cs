

using System.Windows.Media;
using OxyPlot;

namespace Simscop.Pl.Ui.Charts;

public class LineModel
{
    /// <summary>
    /// 
    /// </summary>
    public double[] X { get; set; } = Array.Empty<double>();

    /// <summary>
    /// 
    /// </summary>
    public double[] Y { get; set; } = Array.Empty<double>();

    /// <summary>
    /// 曲线线宽
    /// </summary>
    public double Thickness { get; set; }

    /// <summary>
    /// 线的颜色
    /// </summary>
    public Color LineColor { get; set; }

    /// <summary>
    /// 线的透明度
    /// </summary>
    public double Alpha { get; set; }

    /// <summary>
    /// 曲线类型
    /// </summary>
    public LineStyle LineStyle { get; set; }
}

