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

    // todo 添加xyz单个和组合的移动方式

    // todo 添加移动校验

    /// <summary>
    /// 设置相对位置
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pos"></param>
    public void SetRelativePosition(bool[] index, double[] pos);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Task AsyncSetRelativePosition(bool[] index, double[] pos);

    /// <summary>
    /// 设置绝对位置
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pos"></param>
    public void SetAbsolutePosition(bool[] index, double[] pos);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Task AsyncSetAbsolutePosition(bool[] index, double[] pos);
}