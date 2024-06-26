﻿using System.IO;
using System.Windows.Markup;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OpenCvSharp;
using OxyPlot;
using Simscop.Pl.Core;
using Simscop.Pl.Core.Models;
using Simscop.Pl.Core.Services;
using Simscop.Pl.WPF.Managers;
using Point = System.Drawing.Point;

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
            if (AcquireCollection.Count == 0) return;

            var index = msg.Rows * AcquireColumns + msg.Columns;
            WeakReferenceMessenger.Default.Send(new LineChangedMessage(AcquireCollection[index].Coordinates));
        });

        IntegrationTime = Spectrometer.IntegrationTime;
        MinIntegrationTime = Spectrometer.MinimumIntegrationTime;
        MaxIntegrationTime = Spectrometer.MaximumIntegrationTime;
    }

    #region Spectrometer

    [ObservableProperty]
    private string? _direcotry;

    [ObservableProperty]
    private bool _isSaveMark = false;

    [RelayCommand]
    private void SaveAcquireData()
    {
        if (AcquireCollection.Count == 0)
        {
            ToastManager.Warning("当前没有采集结果");
            return;
        }

        if (Direcotry is null)
        {
            ToastManager.Warning("文件夹为空");
            return;
        }

        if (!Directory.Exists(Direcotry)) Directory.CreateDirectory(Direcotry);

        if (IsSaveMark && VmManager.CameraViewModel.MarkImg is { } img)
        {
            img.SaveImage(Path.Join(Direcotry, "mark.bmp"));
        }


        AcquireCollection.ForEach(acquire
            => File.WriteAllText(Path.Join(Direcotry, $"{acquire.Index.X}_{acquire.Index.Y}.csv")
                , acquire.ToCsvString()));


        ToastManager.Success("存储完毕");
    }

    [ObservableProperty]
    private int _integrationTime = 0;

    [ObservableProperty]
    private int _minIntegrationTime = 0;

    [ObservableProperty]
    private int _maxIntegrationTime = 0;

    [RelayCommand]
    private void SetIntegrationTime()
    {
        var tempLive = IsLive;
        IsLive = false;

        Spectrometer.IntegrationTime = IntegrationTime;

        IsLive = tempLive;
    }

    [ObservableProperty]
    private bool _isLive = false;

    partial void OnIsLiveChanged(bool value)
    {
        if (!IsLive) return;

        Task.Run(() =>
        {
            while (IsLive)
            {
                var data = Spectrometer.GetSpectrum();
                var wave = Spectrometer.GetWavelengths();

                var model = new AcquireModel()
                {
                    X = wave,
                    Y = data
                };

                WeakReferenceMessenger.Default.Send(new LineChangedMessage(model.Coordinates));
            }
        });
    }

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
        var tempLive = IsLive;
        IsLive = false;

        var rmMark = WeakReferenceMessenger.Default.Send<MarkderInfoRequestMessage>();
        var info = rmMark.Response;

        var coor = Motor.Xyz;

        var imgSize = Camera.ImageSize;

        var xFlag = 1; // 正 - 画面向左边移动
        var yFlag = 1; // 正 - 画面向上面移动

        var half = (imgSize.Width / 2.0, imgSize.Height / 2.0);

        var startX = coor.X + ValueManager.PhysicalPixelSize * (info.Left - half.Item1);
        var startY = coor.Y + ValueManager.PhysicalPixelSize * (info.Top - half.Item2);

        var intervalX = Math.Abs(info.Left - info.Right) * ValueManager.PhysicalPixelSize / AcquireColumns;
        var intervalY = Math.Abs(info.Top - info.Bottom) * ValueManager.PhysicalPixelSize / AcquireRows;

        // todo 优化扫描方式
        //var coors = new List<(double X, double Y)>();

        AcquireCollection.Clear();

        // todo 这里的扫描是有问题的

        try
        {
            VmManager.CameraViewModel.Camera.StopCapture();
            AcquirePercent = 0;

            await Task.Run(async () =>
            {
                for (var i = 0; i < AcquireRows; i++)
                    for (var j = 0; j < AcquireColumns; j++)
                    {
                        if (!IsAcquiring) throw new AcquireCanceledException();

                        var pos = (startX + j * intervalX + intervalX / 2, startY + i * intervalY + intervalY / 2);

                        await Motor.AsyncSetAbsolutePosition(
                            new[] { true, true, false },
                            new[] { pos.Item1, pos.Item2, -1 });

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
            });

            WeakReferenceMessenger.Default.Send(new LineChangedMessage(AcquireCollection[0].Coordinates));

            ToastManager.Success("采集完成");
            IsAcquiring = false;

            await Motor.AsyncSetAbsolutePosition(
                new[] { true, true, false },
                new[] { coor.Item1, coor.Item2, -1 });

            VmManager.CameraViewModel.Camera.StartCapture();
        }
        catch (AcquireCanceledException _)
        {
            AcquirePercent = 0;
            ToastManager.Warning("采集已经停止");
        }

        IsLive = tempLive;
    }

    [RelayCommand]
    void AcquireStop() => IsAcquiring = !IsAcquiring;

    #endregion

}