using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simscop.Pl.Core.Services;

/// <summary>
/// 光谱仪的使用接口
/// </summary>
public interface IOmniDriverService : IDeviceService
{
    /// <summary>
    /// 开启指定光谱仪
    /// </summary>
    /// <param name="index"></param>
    public bool OpenSpectrometers(int index);

    /// <summary>
    /// 开启所有的光谱仪
    /// </summary>
    public bool OpenAllSpectrometers();

    /// <summary>
    /// 关闭指定光谱仪
    /// </summary>
    /// <param name="index"></param>
    public void CloseSpectrometers(int index);

    /// <summary>
    /// 关闭所有的光谱仪
    /// </summary>
    public void CloseAllSpectrometers();

    /// <summary>
    /// 获取当前所有的光谱仪
    /// </summary>
    /// <returns></returns>
    public ISpectrometerService[] GetAllSpectrometer();

    /// <summary>
    /// 光谱仪个数
    /// </summary>
    public int NumberOfSpectrometers { get; }
}