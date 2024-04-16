using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simscop.Pl.Core.Services;

/// <summary>
/// 电机控制
/// </summary>
public interface IMotorService : IDeviceService
{
    /// <summary>
    /// 当前设备使用的单位
    /// </summary>
    public string Unit { get; }

    /// <summary>
    /// 获取当前xyz坐标
    /// </summary>
    public (double X, double Y, double Z) Xyz { get; }

    /// <summary>
    /// 当前x坐标
    /// </summary>
    public double X { get; }

    /// <summary>
    /// 当前y坐标
    /// </summary>
    public double Y { get; }

    /// <summary>
    /// 当前z坐标
    /// </summary>
    public double Z { get; }

    /// <summary>
    /// 设置相对位置
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="pos"></param>
    public void SetRelativePosition(Tuple<int> axis, Tuple<double> pos);

    /// <summary>
    /// 设置绝对位置
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="pos"></param>
    public void SetAbsolutePosition(Tuple<int> axis, Tuple<double> pos);
}