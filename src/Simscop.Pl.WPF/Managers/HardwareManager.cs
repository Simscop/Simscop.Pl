using Simscop.Pl.Core.Services;

// ReSharper disable once CheckNamespace
namespace Simscop.Pl.Core;

public static class HardwareManager
{
    /// <summary>
    /// 相机
    /// </summary>
    public static ICameraService? Camera;

    /// <summary>
    /// 电动轴
    /// </summary>
    public static IMotorService? Motor;

    /// <summary>
    /// 光谱仪
    /// </summary>
    public static ISpectrometerService? Spectrometer;

    /// <summary>
    /// 连接方案
    /// </summary>
    public static IOmniDriverService? OmniDriver;

    /// <summary>
    /// 
    /// </summary>
    public static bool IsCameraOk = false;

    /// <summary>
    /// 
    /// </summary>
    public static bool IsMotorOk = false;

    /// <summary>
    /// 
    /// </summary>
    public static bool IsSpectrometerOk = false;

    /// <summary>
    /// 
    /// </summary>
    public static bool IsAllOk => IsCameraOk && IsMotorOk && IsSpectrometerOk;
}