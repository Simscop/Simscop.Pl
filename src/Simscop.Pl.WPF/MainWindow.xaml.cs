using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Win32;
using OpenCvSharp.WpfExtensions;
using OxyPlot;
using OxyPlot.Series;
using Simscop.Pl.Core;
using Simscop.Pl.Hardware;
using Simscop.Pl.Ui;
using Simscop.Pl.Ui.Extensions;
using Simscop.Pl.WPF.Helpers;
using Simscop.Pl.WPF.Managers;
using Simscop.Pl.WPF.Views;
using Simscop.Pl.WPF.Views.MessageBox;
using Window = Lift.UI.Controls.Window;

namespace Simscop.Pl.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Rect _markRect = new();

    private DispatcherTimer _cameraTimer;

    private int _frame = 0;

    private bool _isRender = false;

    private ScanView _scanView;

    public MainWindow()
    {
        InitializeComponent();

        ImageViewer.IsVisibleChanged += MainChildVisibleChanged;
        Heatmap.IsVisibleChanged += MainChildVisibleChanged;
        Line.IsVisibleChanged += MainChildVisibleChanged;

        DataContext = VmManager.MainViewModel;

        _cameraTimer = new DispatcherTimer(priority: DispatcherPriority.Render)
        {
            Interval = TimeSpan.FromSeconds(3)
        };

        RegisterInvoke();
        RegisterMessage();
        RegisterViewModel();

         _scanView = new ScanView();
    }

    private void RegisterViewModel()
    {
        MotorBar.DataContext = VmManager.MotorViewModel;
        MotorSettingBar.DataContext = VmManager.MotorViewModel;
        VmManager.MotorViewModel.StartTimer();
    }

    private void RegisterInvoke()
    {
        ImageViewer.OnMarkderChanged += (rect) =>
        {
            _markRect = rect;
            VmManager.CameraViewModel.UpdateMarkImg(rect);
        };
    }

    private void RegisterMessage()
    {
        WeakReferenceMessenger.Default.Register<AcquireRamanDataMessage>(this, (_, _) =>
        {
            AcquireBox.ShowAsSingleton();
        });

        WeakReferenceMessenger.Default.Register<MarkderInfoRequestMessage>(this, (obj, msg) =>
        {
            msg.Reply(_markRect);
        });

        new List<string>()
        {
            ToastMessage.ToastSucess,
            ToastMessage.ToastInfo,
            ToastMessage.ToastWarning,
            ToastMessage.ToastError,
            ToastMessage.ToastFatal
        }.ForEach(item =>
        {
            WeakReferenceMessenger.Default.Register<string, string>(this, item, (obj, msg) =>
            {
                ToastManager.RunAsString(item, msg);
            });
        });
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        if (_isRender) return;
        _isRender = true;

        Heatmap.DataContext = VmManager.HeatmapViewModel;
        Line.DataContext = VmManager.LineChartViewModel;

        VmManager.LineChartViewModel.OnValueChanged += (n) =>
        {
            if (n == nameof(VmManager.LineChartViewModel.Serial))
            {
                if (VmManager.LineChartViewModel.Serial is not LineSeries series) return;
                Line.ShowSerial(series, 0);

                Line.UpdateAnnotation();
            }

            if (n is nameof(VmManager.LineChartViewModel.AxisX)
                or nameof(VmManager.LineChartViewModel.AxisY)
                or nameof(VmManager.LineChartViewModel.Annotation)
                or nameof(VmManager.LineChartViewModel.Serial)) return;

            var line = new LineSeries()
            {
                Color = VmManager.LineChartViewModel.Color.ToOxyColor(),
                LineStyle = (LineStyle)VmManager.LineChartViewModel.LineStyle,
                TrackerFormatString = VmManager.LineChartViewModel.Format,
                LabelMargin = VmManager.LineChartViewModel.Margin,
                FontSize = VmManager.LineChartViewModel.FontSize,
                FontWeight = VmManager.LineChartViewModel.FontWeight
            };

            line.Points.Clear();
            VmManager.LineChartViewModel.Data?.ForEach(item => line.Points.Add(new DataPoint(item.X, item.Y)));
            VmManager.LineChartViewModel.Serial = line;
        };

        Line.SetBinding(LineChart.SelectedXProperty, new Binding("Selected") { Source = VmManager.LineChartViewModel, Mode = BindingMode.TwoWay });

        //HardwareManager.Camera!.IsAutoExposure = false;

        if (HardwareManager.IsCameraOk)
        {
            HardwareManager.Camera!.IsAutoExposure = false;
            HardwareManager.Camera.Resolution = HardwareManager.Camera.Resolutions[2];

            _cameraTimer.Tick += (_, _) =>
            {
                VmManager.CameraViewModel.FirstInit();

                VmManager.CameraViewModel.Frame = _frame / 3.0;

                _frame = 0;
            };

            _cameraTimer.Start();

            HardwareManager.Camera!.OnCaptureChanged += img =>
            {
                VmManager.CameraViewModel.Image = img.Clone();

                //Debug.WriteLine($"**  {VmManager.CameraViewModel.Image.Size()}");

                var source = img.ToWriteableBitmap(0, 0, PixelFormats.Bgr32, null);
                //var source = img.ToWriteableBitmap();
                ImageViewer.ImageSource = source;
                _frame++;
            };
        }
    }

    private void MainChildVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        var flag = 2;

        if (Heatmap.Visibility == Visibility.Collapsed) flag--;
        if (Line.Visibility == Visibility.Collapsed) flag--;

        Main.ColumnDefinitions[0].Width = ImageViewer.Visibility == Visibility.Collapsed
            ? new GridLength(0)
            : new GridLength(1, GridUnitType.Star);

        Main.ColumnDefinitions[1].Width = flag switch
        {
            0 => new GridLength(0),
            1 => new GridLength(1, GridUnitType.Star),
            2 => new GridLength(1, GridUnitType.Star),
            _ => throw new NotImplementedException()
        };

        Chart.Columns = ImageViewer.Visibility switch
        {
            Visibility.Visible => 1,
            Visibility.Hidden => throw new ArgumentOutOfRangeException(),
            Visibility.Collapsed => flag,
            _ => throw new ArgumentOutOfRangeException()
        };

        Chart.Rows = ImageViewer.Visibility switch
        {
            Visibility.Visible => flag,
            Visibility.Hidden => throw new ArgumentOutOfRangeException(),
            Visibility.Collapsed => 1,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void OnLineChartSettingViewClicked(object sender, RoutedEventArgs e)
    {
        var view = new LineChartSettingView()
        {
            Background = Brushes.White,
        };
        view.Show();
    }


    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        Application.Current.Shutdown();
    }

    private void OnCameraSettingViewClicked(object sender, RoutedEventArgs e)
    {
        var view = new CameraSettingView()
        {
            Background = Brushes.White,
        };
        view.Show();
    }

    private void SaveMenuClicked(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveDataDialog()
        {
            DataContext = DataContext
        };
        dialog.Left = Left + (Width - dialog.Width) / 2;
        dialog.Top = Top + (Height - dialog.Height) / 2;
        dialog.ShowDialog();
    }

    private void OnSaveImageClicked(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "bmp (*.bmp)|*.bmp",
            Title = "存储图片"
        };

        if (dialog.ShowDialog() is true)
            VmManager.CameraViewModel.Image?.SaveImage(dialog.FileName);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        MotorKeepHelper.Keep();

        base.OnClosing(e);

    }

    private bool _isFlag = false;

    private void Scan_Click(object sender, RoutedEventArgs e)
    {

        VmManager.MotorViewModel.SetAbsolutionPosition(new[] { true, true, false }, new double[] { point1.x, point1.y, 15590561.00 });
        Thread.Sleep(1000);
        var xPos = HardwareManager.Motor.X;
        var yPos = HardwareManager.Motor.Y;
        _isFlag = !_isFlag;
        Debug.WriteLine($"{xPos} {yPos}");
       Stopwatch sw = Stopwatch.StartNew();
        Task.Run(() =>
        {
            sw.Start();

            //double xTran = 170000 * 12;//nm
            //double yTran = 100000 * 25;
            //int count = 5;//次数
            //double xcount = count;
            //double ycount = count;

            double xTran = 170000 * 3;//nm
            double yTran = 100000 * 3;
            int xcount = 20;
            int ycount = 20;

            for (int i = 0; i < xcount; i++)
            {
                for (int j = 0; j < ycount; j++)
                {
                    if (!_isFlag) return;
                    double x = 0;
                    double y = 0;
                    if (i % 2 == 0)
                    {
                        x = i;
                        y = j;
                    }
                    else if (i % 2 == 1)
                    {
                        x = i;
                        y = ycount - j - 1;
                    }

                    VmManager.MotorViewModel.SetAbsolutionPosition(new[] { true, true, false }, new double[] { xPos + y * xTran, yPos + x * yTran, 0 });
                    Debug.WriteLine($"{i} {j} ");
                    Thread.Sleep(50);

                    var img = VmManager.CameraViewModel.Image;
                    Task.Run(() =>
                    {
                        img?.SaveImage(@"C:\\Users\\Simsc\\Desktop\\ZZJ\\" + $"{x + 1}_{y + 1}.bmp");
                        img?.Dispose();
                    });

                }
            }

            //76578626.00
            //45238948.00
            //15258689.00
            sw.Stop();
            Debug.WriteLine($"time: {sw.ElapsedMilliseconds} ms");
            MessageBox.Show("complete!");
        });
    }

    static (double x, double y) point1;
    static (double x, double y) point2;

    private void SavePoint1_Click(object sender, RoutedEventArgs e)
    {
        point1.x = HardwareManager.Motor.X;
        point1.y = HardwareManager.Motor.Y;
        MessageBox.Show($"point1  x_{point1.x}  y_{point1.y}");
    }

    private void SavePoint2_Click(object sender, RoutedEventArgs e)
    {
        point2.x = HardwareManager.Motor.X;
        point2.y = HardwareManager.Motor.Y;
        MessageBox.Show($"point2  x_{point2.x}  y_{point2.y}");
    }


    private void ScanView_Click(object sender, RoutedEventArgs e)
    {
      
        if (_scanView.WindowState == WindowState.Minimized)
        {
            _scanView.WindowState = WindowState.Normal;
        }
        else if (!_scanView.IsVisible)
        {
            _scanView.Show();
        }
        else
        {
            _scanView.Activate();
        }
    }
}
