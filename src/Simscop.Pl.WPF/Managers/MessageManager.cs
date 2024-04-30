using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using OpenCvSharp;
using Size = System.Windows.Size;
using Rect = System.Windows.Rect;

namespace Simscop.Pl.WPF.Managers;

// Note 约定俗称的命名内容统一放到最后作为flag

/// <summary>
/// 获取单帧图像
/// </summary>
public class CaptureRequestMessage : RequestMessage<Mat?> { }

/// <summary>
/// 获取相机采集图像的尺寸
/// </summary>
public class CaptureSizeRequestMessage : RequestMessage<Size> { }

/// <summary>
/// 发起拉曼信号采集任务
/// </summary>
public class AcquireRamanDataMessage { }

/// <summary>
/// 标记信息请求
/// </summary>
public class MarkderInfoRequestMessage : RequestMessage<Rect> { }

/// <summary>
/// 波长选择更更改
/// </summary>
/// <param name="Length"></param>
public record SelectedWaveChangedMessage(double Length);

/// <summary>
/// 
/// </summary>
/// <param name="Rows"></param>
/// <param name="Columns"></param>
/// <param name="Array"></param>
public record Line2MapMessage(int Rows, int Columns, double[,] Array);

/// <summary>
/// 从mapping到line
/// </summary>
/// <param name="Rows"></param>
/// <param name="Columns"></param>
public record Map2LineMessage(int Rows, int Columns);

/// <summary>
/// 更新曲线的显示
/// </summary>
public record LineChangedMessage(List<(double X, double Y)> Points);