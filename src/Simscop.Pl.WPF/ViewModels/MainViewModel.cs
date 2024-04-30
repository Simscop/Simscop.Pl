using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Simscop.Pl.Core;
using Simscop.Pl.Core.Models;
using Simscop.Pl.Core.Services;
using Simscop.Pl.WPF.Managers;

namespace Simscop.Pl.WPF.ViewModels;

public class AcquireCanceledException : Exception { }

public partial class MainViewModel : ObservableObject
{
    internal IMotorService Motor =
        HardwareManager.Motor ?? throw new NotImplementedException("Motor not exist.");

    internal ICameraService Camera =
        HardwareManager.Camera ?? throw new NotImplementedException("Camera not exist.");

    internal ISpectrometerService Spectrometer =
        HardwareManager.Spectrometer ?? throw new NotImplementedException("Spectrometer not exist.");

    public MainViewModel()
    {
        WeakReferenceMessenger.Default.Register<SelectedWaveChangedMessage>(this,
            (obj, msg) =>
        {
            if (AcquireCollection.Count != AcquireRows * AcquireColumns) return;

            var length = msg.Length;
            var index = AcquireCollection[0].X!
                .Select((v, i) => new { Index = i, Distance = Math.Abs(length - v) })
                .MinBy(item => item.Distance)!.Index;

            var array1 = AcquireCollection.Select(item => item.Y![index]).ToArray();

            // 1-d to 2-d
            var array2 = new double[AcquireColumns, AcquireRows];
            for (var i = 0; i < AcquireRows; i++)
                for (var j = 0; j < AcquireColumns; j++)
                    array2[j, i] = array1[i * AcquireColumns + j];

            WeakReferenceMessenger.Default.Send(new Line2MapMessage(AcquireRows, AcquireColumns, array2));
        });

        WeakReferenceMessenger.Default.Register<Map2LineMessage>(this, (obj, msg) =>
        {
            var index = msg.Rows * AcquireColumns + msg.Columns;

            WeakReferenceMessenger.Default.Send(new LineChangedMessage(AcquireCollection[index].Coordinates));

        });
    }

    #region Spectrometer



    #endregion

    #region Acquire

    private string _acquireLog = "暂停使用";

    protected List<AcquireModel> AcquireCollection = new();

    public string AcquireLog
    {
        get => _acquireLog;
        set
        {
            if (_acquireLog.Length > 4000)
                _acquireLog = _acquireLog.Substring(2000);

            SetProperty(ref _acquireLog, value);
        }
    }

    private double _acquirePercent = 0;

    /// <summary>
    /// 0 - 100
    /// </summary>
    public double AcquirePercent
    {
        get => _acquirePercent;
        set => SetProperty(ref _acquirePercent, value > 100 ? 100 : (value < 0 ? 0 : value));
    }

    /// <summary>
    /// 
    /// </summary>
    [ObservableProperty]
    private int _acquireRows = 2;

    /// <summary>
    /// 
    /// </summary>
    [ObservableProperty]
    private int _acquireColumns = 2;

    /// <summary>
    /// 是否正在采集
    /// </summary>
    [ObservableProperty]
    private bool _isAcquiring = false;

    /// <summary>
    /// 
    /// </summary>
    [RelayCommand]
    void AcquireRamanData()
        => WeakReferenceMessenger.Default.Send<AcquireRamanDataMessage>();

    [RelayCommand]
    private async Task AcquireStart()
    {
        if (AcquireRows <= 0 || AcquireColumns <= 0)
            throw new Exception("");

        IsAcquiring = !IsAcquiring;

        var rmMark = WeakReferenceMessenger.Default.Send<MarkderInfoRequestMessage>();
        var info = rmMark.Response;

        var coor = Motor.Xyz;

        var imgSize = Camera.ImageSize;

        var xFlag = 1; // 相对向右
        var yFlag = 1; // 相对向下

        var half = (imgSize.Width / 2.0, imgSize.Height / 2.0);

        var x1 = xFlag * (info.Left - half.Item1) * ValueManager.PhysicalPixelSize + coor.X;
        var x2 = xFlag * (info.Right - half.Item1) * ValueManager.PhysicalPixelSize + coor.X;

        var y1 = yFlag * (info.Top - half.Item2) * ValueManager.PhysicalPixelSize + coor.Y;
        var y2 = yFlag * (info.Bottom - half.Item2) * ValueManager.PhysicalPixelSize + coor.Y;

        var xInterval = Math.Abs(x1 - x2) / AcquireColumns;
        var yInterval = Math.Abs(y1 - y2) / AcquireRows;

        // todo 验证坐标是否有效且可行
        var xStart = Math.Min(x1, x2);
        var yStart = Math.Max(y1, y2);

        // todo 优化扫描方式
        //var coors = new List<(double X, double Y)>();

        AcquireCollection.Clear();

        // todo 这里的扫描是有问题的

        await Task.Run(() =>
        {
            try
            {
                AcquirePercent = 0;

                for (var i = 0; i < AcquireRows; i++)
                    for (var j = 0; j < AcquireColumns; j++)
                    {
                        if (!IsAcquiring) throw new AcquireCanceledException();

                        var pos = (xStart + j * xInterval, yStart + i * yInterval);

                        Motor.SetAbsolutePosition(
                            new[] { true, true, false },
                            new[] { pos.Item1, pos.Item2, -1 });

                        Thread.Sleep(0);

                        var data = Spectrometer.GetSpectrum();
                        var wave = Spectrometer.GetWavelengths();

                        var model = new AcquireModel()
                        {
                            X = wave,
                            Y = data,
                            Point = pos,
                            Index = new Point(j, i)
                        };
                        AcquireCollection.Add(model);

                        WeakReferenceMessenger.Default.Send(new LineChangedMessage(model.Coordinates));

                        AcquirePercent += 100.0 / (AcquireRows * AcquireColumns);
                    }

                WeakReferenceMessenger.Default.Send(new LineChangedMessage(AcquireCollection[0].Coordinates));

                ToastManager.Success("采集完成");
                IsAcquiring = false;
            }
            catch (AcquireCanceledException _)
            {
                AcquirePercent = 0;
                ToastManager.Warning("采集已经停止");
            }
        });
    }

    [RelayCommand]
    void AcquireStop() => IsAcquiring = !IsAcquiring;

    #endregion

}