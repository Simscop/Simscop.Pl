using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Simscop.Pl.Core.Services;
using Simscop.Pl.Hardware.Camera;
using Size = OpenCvSharp.Size;
using Window = System.Windows.Window;

namespace Example.Camera;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{

    ICameraService _camera = new ToupTek();

    private MainViewModel _vm = new MainViewModel();

    public MainWindow()
    {
        InitializeComponent();

        Closed += (_, _) => _camera.DeInitialize();

        Debug.WriteLine(_camera.Valid());
        Debug.WriteLine(_camera.Initialize());

        _camera.IsAutoExposure = false;
        _camera.Exposure = 0.1;
        
        _camera.Resolution = _camera.Resolutions[^1];
        Debug.WriteLine(_camera.Exposure);

        this.DataContext = _vm;

    }

    public static readonly DependencyProperty FrameProperty = DependencyProperty.Register(
        nameof(Frame), typeof(BitmapFrame), typeof(MainWindow), new PropertyMetadata(default(BitmapFrame)));

    public BitmapFrame Frame
    {
        get => (BitmapFrame)GetValue(FrameProperty);
        set => SetValue(FrameProperty, value);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        Task.Run(() =>
        {
            while (true)
            {
                _camera.Capture(out var mat);

                if (mat == null || mat.Rows == 0 || mat.Cols == 0) continue;

                Thread.Sleep(10);
                Dispatcher.BeginInvoke(() =>
                {
                    //Cv2.ImShow("show", mat.Resize(new Size(), 0.1, 0.1));
                    //Cv2.WaitKeyEx(1);
                    //var source = mat.ToBitmapSource();
                    //Debug.WriteLine("show");
                    var frame = mat.CvtColor(ColorConversionCodes.RGBA2BGR).ToBitmapSource();
                    _vm.Frame = BitmapFrame.Create(frame);

                    mat.Dispose();
                });
            }
        });
    }
}


public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private BitmapFrame _frame;

    partial void OnFrameChanged(BitmapFrame value)
    {
        //Debug.WriteLine("Chagned");
    }
}