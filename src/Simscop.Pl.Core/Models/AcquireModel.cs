using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simscop.Pl.Core.Models;

public class AcquireModel
{
    // todo 其他信息

    /// <summary>
    /// 采集点
    /// </summary>
    public (double X, double Y) Point { get; set; }

    /// <summary>
    /// 行列坐标点
    /// </summary>
    public Point Index { get; set; }

    /// <summary>
    /// 采集结果
    /// </summary>
    public double[]? Y { get; set; }

    /// <summary>
    /// 波长
    /// </summary>
    public double[]? X { get; set; }

    /// <summary>
    /// 坐标
    /// </summary>
    public List<(double X, double Y)> Coordinates => Enumerable.Range(0, X.Length)
        .Select(item => (X[item], Y[item])).ToList();
}