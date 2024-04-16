using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simscop.Pl.Core.Services;

public interface IDeviceService
{
    /// <summary>
    /// 型号
    /// </summary>
    public string Model { get; set; }

    /// <summary>
    /// 序列号
    /// </summary>
    public string SerialNumber { get; set; }

    /// <summary>
    /// 固件版本
    /// </summary>
    public string Fireware { get; set; }

    /// <summary>
    /// 硬件型号
    /// </summary>
    public string HardwareVersion { get; set; }

    /// <summary>
    /// 保留配置
    /// </summary>
    public Dictionary<string, string>? Reserved { get; set; }

    /// <summary>
    /// 设备运行环境检验
    /// </summary>
    /// <returns></returns>
    public bool Valid();
}