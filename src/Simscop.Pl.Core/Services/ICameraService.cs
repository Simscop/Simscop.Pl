using OpenCvSharp;

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
    /// 曝光，单位ms
    /// </summary>
    public double Exposure { get; set; }

    /// <summary>
    /// 图像Gamma值，默认为1
    /// </summary>
    public double Gamma { get; set; }

    /// <summary>
    /// 图像对比度，默认为0
    /// </summary>
    public double Contrast { get; set; }

    /// <summary>
    /// 图像亮度，默认为0
    /// </summary>
    public double Brightness { get; set; }

    /// <summary>
    /// 相机增益，仅设配硬件增益
    /// </summary>
    public int Gain { get; set; }

    /// <summary>
    /// 是否自动色阶
    /// </summary>
    public double IsAutoLevel { get; set; }

    /// <summary>
    /// 是否自动曝光
    /// </summary>
    public double IsAutoExposure { get; set; }

    /// <summary>
    /// 相机当前帧率
    /// </summary>
    public double Frame { get; set; }

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
    public Size ImageSize { get; set; }

    /// <summary>
    /// 当前模式色阶上下限
    /// </summary>
    public LevelRange LevelRange { get; set; }

    /// <summary>
    /// 当前实际左右色阶
    /// </summary>
    public LevelRange CurrentLevel { get; set; }

    /// <summary>
    /// 存取图片
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool SaveImage(string path);
}