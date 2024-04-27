using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Fake.Hardware;
using Lift.Core.Exception;
using Lift.UI.V2.Controls.PropertyGrid;
using Simscop.Pl.Core;
using Simscop.Pl.Core.Services;
using Simscop.Pl.WPF.Managers;

namespace Simscop.Pl.WPF.ViewModels;

public partial class CameraViewModel : ObservableObject
{
    public static IMessenger Messager = WeakReferenceMessenger.Default;

    #region 硬件部分

    [PropertyGrid(Ignore = true)]
    public ICameraService Camera;

    [PropertyGrid(Ignore = true)]
    public List<(uint Width, uint Height)>? Resolutions;

    public CameraViewModel()
    {
        HardwareManager.Camera = new FakeCamera();

        Camera = HardwareManager.Camera
                 ?? throw new NotImplementedException("The Camera not implemented.");

        Messager.Register<CaptureRequestMessage>(this, (_, msg) =>
        {
            if (msg.HasReceivedResponse)
                throw new InvalidException("The message has been done.");

            Camera.Capture(out var img);
            msg.Reply(img);
        });

        Exposure = 1000;
    }

    [Range(10, 10000)] // todo 后面手动给相机的实际结果
    public double Exposure
    {
        get => Camera.Exposure;
        set => SetProperty(Camera.Exposure, value, (_) => Camera.Exposure = value);
    }

    [Range(10, 10000)] // todo 后面手动给相机的实际结果
    public double Temperature
    {
        get => Camera.Temperature;
        set => SetProperty(Camera.Temperature, value, (_) => Camera.Temperature = value);
    }

    [Range(10, 10000)] // todo 后面手动给相机的实际结果
    public double Tint
    {
        get => Camera.Tint;
        set => SetProperty(Camera.Tint, value, (_) => Camera.Tint = value);
    }

    [Range(-1, 1)]
    public double Gamma
    {
        get => Camera.Gamma;
        set => SetProperty(Camera.Gamma, value, (_) => Camera.Gamma = value);
    }

    [Range(-1, 1)]
    public double Contrast
    {
        get => Camera.Contrast;
        set => SetProperty(Camera.Contrast, value, (_) => Camera.Contrast = value);
    }

    [Range(-1, 1)]
    public double Brightness
    {
        get => Camera.Brightness;
        set => SetProperty(Camera.Brightness, value, (_) => Camera.Brightness = value);
    }

    public bool IsAutoLevel
    {
        get => Camera.IsAutoLevel;
        set => SetProperty(Camera.IsAutoLevel, value, (_) => Camera.IsAutoLevel = value);
    }

    public bool IsAutoExposure
    {
        get => Camera.IsAutoLevel;
        set => SetProperty(Camera.IsAutoExposure, value, (_) => Camera.IsAutoExposure = value);
    }

    [ObservableProperty]
    private double _frame = 0;

    public int ClockwiseRotation
    {
        get => Camera.ClockwiseRotation;
        set => SetProperty(Camera.ClockwiseRotation, value, (_) => Camera.ClockwiseRotation = value);
    }

    public bool IsFlipHorizontally
    {
        get => Camera.IsFlipHorizontally;
        set => SetProperty(Camera.IsFlipHorizontally, value, (_) => Camera.IsFlipHorizontally = value);
    }

    public bool IsFlipVertially
    {
        get => Camera.IsFlipVertially;
        set => SetProperty(Camera.IsFlipVertially, value, (_) => Camera.IsFlipVertially = value);
    }

    [ObservableProperty]
    private int _resolutionIndex = 0;

    #endregion
}