using System.Windows.Threading;

namespace Simscop.Pl.Core.Services;

// todo 需要优化一下属性的限定域

public interface IDeviceService
{
    /// <summary>
    /// 型号
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// 序列号
    /// </summary>
    public string? SerialNumber { get; set; }

    /// <summary>
    /// 固件版本
    /// </summary>
    public string? Fireware { get; set; }

    /// <summary>
    /// 硬件型号
    /// </summary>
    public string? HardwareVersion { get; set; }

    /// <summary>
    /// 保留配置
    /// </summary>
    public Dictionary<string, string>? Reserved { get; set; }

    /// <summary>
    /// 设备运行环境检验,不需要连接的情况下
    /// </summary>
    /// <returns></returns>
    public bool Valid();

    /// <summary>
    /// 初始化设备
    /// </summary>
    /// <returns></returns>
    public bool Initialize();

    /// <summary>
    /// 反初始化设备
    /// </summary>
    /// <returns></returns>
    public bool DeInitialize();

    /// <summary>
    /// 最后一个错误
    /// </summary>
    public string? LastErrorMessage { get; }
}