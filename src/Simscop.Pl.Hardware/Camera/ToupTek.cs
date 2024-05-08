using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;
using OpenCvSharp;
using Simscop.Pl.Core.Services;
using Size = Simscop.Pl.Core.Size;

namespace Simscop.Pl.Hardware.Camera;

// todo 后面的优化为借助DelegateOnEventCallback来优化某些属性和设置方式
// todo 另外可能接口里面要多加一些事件才行

public class ToupTek : ICameraService
{
    private Toupcam? _camera = null;

    private bool _passCapture = true;

    private bool _isCapture = false;

    public string Model { get; set; } = "none";
    public string SerialNumber { get; set; } = "none";
    public string Fireware { get; set; } = "none";
    public string HardwareVersion { get; set; } = "none";
    public Dictionary<string, string>? Reserved { get; set; }

    public bool Valid()
    {
        try
        {
            if (_camera is not null) return false;

            var array = Toupcam.EnumV2();

            // the camera not found
            if (array.Length == 0) return false;

            // todo 这里当前也只考虑一个相机的情况

            Model = array[0].displayname;
            SerialNumber = array[0].id;

            Resolutions = array[0].model.res.Select(item => (item.width, item.height)).ToList();

            return true;
        }
        catch (Exception e)
        {
            LastErrorMessage = e.Message;
            return false;
        }
    }

    /// <inheritdoc/>
    public bool Initialize()
    {
        _camera = Toupcam.Open(SerialNumber);

        if (_camera is null) return false;

        // note 当前只考虑多色模式
        var mode = _camera.MonoMode;
        //Exposure = 1000;

        // 获取范围
        _camera.get_ExpTimeRange(out var min, out var max, out _);
        ExposureRange = (min / 1000.0, max / 1000.0);



        return _camera.StartPullModeWithCallback(new(DelegateOnEventCallback));
    }

    private void DelegateOnEventCallback(Toupcam.eEVENT evt)
    {
        /* this run in the UI thread */
        if (_camera == null) return;

        // 图片到达
        if (evt == Toupcam.eEVENT.EVENT_IMAGE)
        {
            if (SafeThreading is not null)
                SafeThreading?.BeginInvoke(ImageEvent);
            else ImageEvent();
        }
    }

    void ImageEvent()
    {
        /* this run in the UI thread */
        if (_camera == null) return;

        var size = ImageSize;

        var mat = new Mat();
        try
        {
            mat = new Mat(size.Height, size.Width, MatType.CV_8UC4);
            var rec = _camera.PullImageV3(mat.Data, 0, 32, (int)mat.Step(), out var info);

            if (!rec) return;
            _currentImage = mat.Clone();
            OnCaptureChanged?.Invoke(_currentImage);

        }
        catch (Exception ex)
        {
            LastErrorMessage = ex.Message;
        }
        finally
        {
            mat.Dispose();
        }
    }

    /// <inheritdoc/>
    public bool DeInitialize()
    {
        _camera?.Close();
        _camera = null;
        return true;
    }

    public string? LastErrorMessage { get; private set; }

    private Mat? _currentImage = new();

    public bool Capture(out Mat? img)
    {
        var size = ImageSize;
        img = null;

        if (size.Width == 0 || size.Height == 0) return false;

        //_currentImage = null;
        //_passCapture = false;

        //var exp = Exposure / 2;
        //var count = 0;
        //while (_currentImage is null || count++ < 4)
        //    Thread.Sleep((int)exp);

        img = _currentImage.Clone();
        GC.Collect();

        _passCapture = true;

        return true;
    }

    public (double Min, double Max) ExposureRange { get; private set; }

    public double Exposure
    {
        get
        {
            if (_camera is null
                || !_camera.get_ExpoTime(out var exp)) return -1;

            return exp / 1000.0;
        }
        set
        {
            SafeThreading?.BeginInvoke(() =>
            {
                if (_camera is null
                    || value > ExposureRange.Max
                    || value < ExposureRange.Min) return;

                var exp = (uint)Math.Floor(value * 1000);
                if (!_camera.put_ExpoTime(exp))
                    throw new Exception();
            });
        }
    }

    public (double Min, double Max) TemperatureRange => (2000, 15000);

    public double Temperature
    {
        get
        {
            if (_camera is null
                || !_camera.get_TempTint(out var temp, out _)) return -1;
            return temp;
        }
        set
        {
            SafeThreading?.BeginInvoke(() =>
            {
                if (_camera is null
                    || value > TemperatureRange.Max
                    || value < TemperatureRange.Min) return;
                _camera.put_TempTint((int)value, (int)Tint);
            });
        }
    }

    public (double Min, double Max) TintRange => (200, 2500);

    public double Tint
    {
        get
        {
            if (_camera is null
                || !_camera.get_TempTint(out _, out var tint)) return -1;
            return tint;
        }
        set
        {
            SafeThreading?.BeginInvoke(() =>
            {
                if (_camera is null
                || value > TintRange.Max
                || value < TintRange.Min) return;
                _camera.put_TempTint((int)Temperature, (int)value);
            });
        }
    }
    public double Gamma
    {
        get
        {
            if (_camera is null
                || !_camera.get_Gamma(out var gamma)) return -1;
            return (gamma - 100) / 80.0;
        }
        set
        {
            SafeThreading?.BeginInvoke(() =>
            {
                if (_camera is null) return;
                if (value is < -1 or > 1) value = 0;

                // 20~180 default:100
                _camera.put_Gamma((int)Math.Floor(value * 80.0 + 100));
            });
        }
    }

    public double Contrast
    {
        get
        {
            if (_camera is null
                || !_camera.get_Contrast(out var contrast)) return -1;
            return contrast / 100.0;
        }
        set
        {
            SafeThreading?.BeginInvoke(() =>
            {
                if (_camera is null) return;
                if (value is < -1 or > 1) value = 0;

                // -100~100 default:0
                _camera.put_Contrast((int)Math.Floor(value * 100));
            });
        }
    }

    public double Brightness
    {
        get
        {
            if (_camera is null
                || !_camera.get_Brightness(out var brightness)) return -1;
            return brightness / 64.0;
        }
        set
        {
            SafeThreading?.BeginInvoke(() =>
            {
                if (_camera is null) return;
                if (value is < -1 or > 1) value = 0;

                // -64~64 default:0
                _camera.put_Brightness((int)Math.Floor(value * 64));
            });
        }
    }

    public int Gain { get; set; }

    public bool IsAutoLevel { get; set; }

    public bool IsAutoExposure
    {
        get => _camera is not null
               && _camera.get_AutoExpoEnable(out bool auto)
               && auto;

        set => _camera?.put_AutoExpoEnable(value);
    }
    public double Frame { get; set; }
    public int ClockwiseRotation { get; set; }
    public bool IsFlipHorizontally { get; set; }
    public bool IsFlipVertially { get; set; }
    public int ImageDetph { get; set; }

    public Size ImageSize
        => (_camera is null
            || !_camera.get_Size(out var width, out var height))
            ? new Size(0, 0)
            : new Size(width, height);

    public (double Left, double Right) LevelRange => (0, 255);
    public (double Left, double Right) CurrentLevel { get; set; }
    public bool SaveImage(string path)
    {
        throw new NotImplementedException();
    }

    // note 修改这个会暂停相机
    public (uint Width, uint Height) Resolution
    {
        get => (_camera is null
                || !_camera.get_eSize(out var size)
                || size > Resolutions.Count)
            ? (0, 0)
            : Resolutions[(int)size];

        set
        {
            SafeThreading?.BeginInvoke(() =>
            {
                if (_camera is null
                    || (value.Width == Resolution.Width
                        && value.Height == Resolution.Height)) return;
                var index = Resolutions.FindIndex(item => item.Width == value.Width
                                                          && item.Height == value.Height);

                if (index < 0 || !_camera.Stop()) return;

                _camera.Stop();
                _camera.put_eSize((uint)index);

                _camera.StartPullModeWithCallback(new(DelegateOnEventCallback));
            });
        }
    }

    public List<(uint Width, uint Height)> Resolutions { get; set; } = new();

    public event Action<Mat>? OnCaptureChanged;

    public Dispatcher? SafeThreading { get; set; } = Application.Current.Dispatcher;
}