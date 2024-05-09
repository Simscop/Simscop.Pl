using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Windows;
using System.Windows.Threading;
using OpenCvSharp;
using Simscop.Pl.Core.Services;
using Size = Simscop.Pl.Core.Size;

namespace Fake.Hardware;

public class FakeCamera : ICameraService
{
    private DispatcherTimer _timer;

    public FakeCamera()
    {
        _timer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _timer.Tick += (s, e) =>
        {
            SafeThreading?.BeginInvoke(() =>
            {
                var color = _colors[(_count++) % _colors.Count];

                var img = new Mat(new OpenCvSharp.Size(1024, 1024), MatType.CV_8UC4,
                    new Scalar(color.B, color.G, color.R, color.A));

                img = Cv2.ImRead(@"C:\Users\haeer\Desktop\icon-plus.tif");
                //ImageSize = new Size(img.Size().Width, img.Size().Height);

                OnCaptureChanged?.Invoke(img);
            });
        };
        _timer.Start();
    }

    public string? Model { get; set; } = "Fake";
    public string? SerialNumber { get; set; } = "v1.0";
    public string? Fireware { get; set; } = "v1.0";
    public string? HardwareVersion { get; set; } = "v1.0";
    public Dictionary<string, string>? Reserved { get; set; }
    public bool Valid() => true;

    public bool Initialize() => true;

    public bool DeInitialize() => true;

    public string? LastErrorMessage => null;

    private List<Color> _colors = new()
    {
        Color.Transparent,
        Color.AliceBlue,
        Color.AntiqueWhite,
        Color.Aqua,
        Color.Aquamarine,
        Color.Azure,
        Color.Beige,
        Color.Bisque,
        Color.Black,
        Color.BlanchedAlmond,
        Color.Blue,
        Color.BlueViolet,
        Color.Brown,
        Color.BurlyWood,
        Color.CadetBlue,
        Color.Chartreuse,
        Color.Chocolate,
        Color.Coral,
        Color.CornflowerBlue,
        Color.Cornsilk,
        Color.Crimson,
        Color.Cyan,
        Color.DarkBlue,
        Color.DarkCyan,
        Color.DarkGoldenrod,
        Color.DarkGray,
        Color.DarkGreen,
        Color.DarkKhaki,
        Color.DarkMagenta,
        Color.DarkOliveGreen,
        Color.DarkOrange,
        Color.DarkOrchid,
        Color.DarkRed,
        Color.DarkSalmon,
        Color.DarkSeaGreen,
        Color.DarkSlateBlue,
        Color.DarkSlateGray,
        Color.DarkTurquoise,
        Color.DarkViolet,
        Color.DeepPink,
        Color.DeepSkyBlue,
        Color.DimGray,
        Color.DodgerBlue,
        Color.Firebrick,
        Color.FloralWhite,
        Color.ForestGreen,
        Color.Fuchsia,
        Color.Gainsboro,
        Color.GhostWhite,
        Color.Gold,
        Color.Goldenrod,
        Color.Gray,
        Color.Green,
        Color.GreenYellow,
        Color.Honeydew,
        Color.HotPink,
        Color.IndianRed,
        Color.Indigo,
        Color.Ivory,
        Color.Khaki,
        Color.Lavender,
        Color.LavenderBlush,
        Color.LawnGreen,
        Color.LemonChiffon,
        Color.LightBlue,
        Color.LightCoral,
        Color.LightCyan,
        Color.LightGoldenrodYellow,
        Color.LightGray,
        Color.LightGreen,
        Color.LightPink,
        Color.LightSalmon,
        Color.LightSeaGreen,
        Color.LightSkyBlue,
        Color.LightSlateGray,
        Color.LightSteelBlue,
        Color.LightYellow,
        Color.Lime,
        Color.LimeGreen,
        Color.Linen,
        Color.Magenta,
        Color.Maroon,
        Color.MediumAquamarine,
        Color.MediumBlue,
        Color.MediumOrchid,
        Color.MediumPurple,
        Color.MediumSeaGreen,
        Color.MediumSlateBlue,
        Color.MediumSpringGreen,
        Color.MediumTurquoise,
        Color.MediumVioletRed,
        Color.MidnightBlue,
        Color.MintCream,
        Color.MistyRose,
        Color.Moccasin,
        Color.NavajoWhite,
        Color.Navy,
        Color.OldLace,
        Color.Olive,
        Color.OliveDrab,
        Color.Orange,
        Color.OrangeRed,
        Color.Orchid,
        Color.PaleGoldenrod,
        Color.PaleGreen,
        Color.PaleTurquoise,
        Color.PaleVioletRed,
        Color.PapayaWhip,
        Color.PeachPuff,
        Color.Peru,
        Color.Pink,
        Color.Plum,
        Color.PowderBlue,
        Color.Purple,
        Color.Red,
        Color.RosyBrown,
        Color.RoyalBlue,
        Color.SaddleBrown,
        Color.Salmon,
        Color.SandyBrown,
        Color.SeaGreen,
        Color.SeaShell,
        Color.Sienna,
        Color.Silver,
        Color.SkyBlue,
        Color.SlateBlue,
        Color.SlateGray,
        Color.Snow,
        Color.SpringGreen,
        Color.SteelBlue,
        Color.Tan,
        Color.Teal,
        Color.Thistle,
        Color.Tomato,
        Color.Turquoise,
        Color.Violet,
        Color.Wheat,
        Color.White,
        Color.WhiteSmoke,
        Color.Yellow,
        Color.YellowGreen,
        Color.RebeccaPurple,
    };

    private int _count = 0;

    public bool Capture(out Mat? img)
    {
        if (_count != 0) Thread.Sleep((int)Exposure);

        var color = _colors[(_count++) % _colors.Count];

        //img = new Mat(new OpenCvSharp.Size(1024, 1024), MatType.CV_8UC4,
        //        new Scalar(color.B, color.G, color.R, color.A));

        img = Cv2.ImRead(@"C:\Users\haeer\Desktop\icon-plus.tif");


        ImageSize = new Size(img.Size().Width, img.Size().Height);
        return true;
    }

    public bool AutoWhiteBlanceOnce()
    {
        return true;
    }

    public bool StartCapture() => true;

    public bool StopCapture() => false;

    public (double Min, double Max) ExposureRange { get; } = (50, 10000);

    public double Exposure { get; set; } = 100;

    public (double Min, double Max) TemperatureRange { get; } = (0, 100);

    public double Temperature { get; set; } = 100;

    public (double Min, double Max) TintRange { get; } = (0, 100);

    public double Tint { get; set; } = 100;

    public double Gamma { get; set; } = 0;

    public double Contrast { get; set; } = 0;

    public double Brightness { get; set; } = 0;

    public int Gain { get; set; } = 0;

    public bool IsAutoLevel { get; set; } = false;

    public bool IsAutoExposure { get; set; } = false;

    public double Frame { get; } = 10;

    public int ClockwiseRotation { get; set; } = 0;

    public bool IsFlipHorizontally { get; set; } = false;

    public bool IsFlipVertially { get; set; } = false;

    public int ImageDetph { get; set; } = 8;

    public Size ImageSize { get; private set; } = new Size(0, 0);

    public (double Left, double Right) LevelRange { get; } = (0, 255);

    public (double Left, double Right) CurrentLevel { get; set; } = (0, 255);

    public bool SaveImage(string path)
    {
        throw new NotImplementedException();
    }

    public (uint Width, uint Height) Resolution { get; set; } = (1024, 1024);
    public List<(uint Width, uint Height)> Resolutions { get; set; } = new()
    {
        (512,512),
        (1024,1024),
        (2048,2048)
    };
    public event Action<Mat>? OnCaptureChanged;

    public Dispatcher? SafeThreading { get; set; } = Application.Current.Dispatcher;
}