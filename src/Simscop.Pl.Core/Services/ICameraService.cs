using System.Windows;
using OpenCvSharp;
using System.Windows.Threading;

namespace Simscop.Pl.Core.Services;

/// <summary>
/// 相机接口
///
/// todo NOTE：图像处理顺序
/// todo 平场矫正，暗场矫正，锐化，降噪，自动对焦，HDR，坏点矫正
/// </summary>
public interface ICameraService : IDeviceService
{
    /// <summary>
    /// 采集并返回一张当前的图像
    /// </summary>
    /// <param name="img"></param>
    /// <returns></returns>
    public bool Capture(out Mat? img);

    /// <summary>
    /// 执行一次相机自动白平衡
    /// </summary>
    /// <returns></returns>
    public bool AutoWhiteBlanceOnce();

    /// <summary>
    /// 开始采集数据
    /// </summary>
    /// <returns></returns>
    public bool StartCapture();

    /// <summary>
    /// 停止捕获数据
    /// </summary>
    /// <returns></returns>
    public bool StopCapture();

    /// <summary>
    /// 曝光范围
    /// </summary>
    public (double Min, double Max) ExposureRange { get; }

    /// <summary>
    /// 曝光，单位ms
    /// </summary>
    public double Exposure { get; set; }

    /// <summary>
    /// 色温范围
    /// </summary>
    public (double Min, double Max) TemperatureRange { get; }


    /// <summary>
    /// 色温
    /// </summary>
    public double Temperature { get; set; }

    /// <summary>
    /// 色调范围
    /// </summary>
    public (double Min, double Max) TintRange { get; }

    /// <summary>
    /// 色调
    /// </summary>
    public double Tint { get; set; }

    /// <summary>
    /// 图像Gamma值，默认为1
    /// -1 - 1
    /// </summary>
    public double Gamma { get; set; }

    /// <summary>
    /// 图像对比度，默认为0
    /// -1 - 1
    /// </summary>
    public double Contrast { get; set; }

    /// <summary>
    /// 图像亮度，默认为0
    /// -1 - 1
    /// </summary>
    public double Brightness { get; set; }

    /// <summary>
    /// 相机增益，仅设配硬件增益
    /// </summary>
    public int Gain { get; set; }

    /// <summary>
    /// 是否自动色阶
    /// </summary>
    public bool IsAutoLevel { get; set; }

    /// <summary>
    /// 是否自动曝光
    /// </summary>
    public bool IsAutoExposure { get; set; }

    /// <summary>
    /// 相机当前帧率
    /// </summary>
    public double Frame { get; }

    /// <summary>
    /// 顺时针旋转角度
    /// <code>
    /// 0 - 0
    /// 1 - 90
    /// 2 - 180
    /// 3 - 270
    /// </code>
    /// </summary>
    public int ClockwiseRotation { get; set; }

    /// <summary>
    /// 是否水平翻转
    /// </summary>
    public bool IsFlipHorizontally { get; set; }

    /// <summary>
    /// 是否垂直翻转
    /// </summary>
    public bool IsFlipVertially { get; set; }

    /// <summary>
    /// 图像深度，8bit or 16bit
    /// </summary>
    public int ImageDetph { get; set; }

    /// <summary>
    /// 采集图像的分辨率
    /// </summary>
    public Size ImageSize { get; }

    /// <summary>
    /// 当前模式色阶上下限
    /// </summary>
    public (double Left, double Right) LevelRange { get; }

    /// <summary>
    /// 当前实际左右色阶
    /// </summary>
    public (double Left, double Right) CurrentLevel { get; set; }

    /// <summary>
    /// 存取图片
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool SaveImage(string path);

    /// <summary>
    /// 当前分辨率
    /// </summary>
    public (uint Width, uint Height) Resolution { get; set; }

    /// <summary>
    /// 支持的分辨率种类
    /// </summary>
    public List<(uint Width, uint Height)> Resolutions { get; set; }

    /// <summary>
    /// 当捕获结果刷新
    /// </summary>
    public event Action<Mat> OnCaptureChanged;

    /// <summary>
    /// 
    /// </summary>
    public Dispatcher? SafeThreading { get; set; }
}