using Simscop.Pl.WPF.ViewModels;

// ReSharper disable once CheckNamespace
namespace Simscop.Pl.WPF;

public static class VmManager
{
    /// <summary>
    /// 
    /// </summary>
    public static MainViewModel MainViewModel = new();

    /// <summary>
    /// 
    /// </summary>
    public static CameraViewModel CameraViewModel = new();

    /// <summary>
    /// 
    /// </summary>
    public static HeatmapViewModel HeatmapViewModel = new();

    /// <summary>
    /// 
    /// </summary>
    public static LineChartViewModel LineChartViewModel = new();

    /// <summary>
    /// 
    /// </summary>
    public static MotorViewModel MotorViewModel = new();
}