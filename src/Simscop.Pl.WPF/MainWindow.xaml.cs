using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Data;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging;
using OpenCvSharp.WpfExtensions;
using OxyPlot;
using OxyPlot.Series;
using Simscop.Pl.Core;
using Simscop.Pl.Ui;
using Simscop.Pl.Ui.Extensions;
using Simscop.Pl.WPF.Managers;
using Simscop.Pl.WPF.Views;
using Simscop.Pl.WPF.Views.MessageBox;

namespace Simscop.Pl.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{

    private System.Windows.Rect _markRect = new();

    public MainWindow()
    {
        InitializeComponent();

        ImageViewer.IsVisibleChanged += MainChildVisibleChanged;
        Heatmap.IsVisibleChanged += MainChildVisibleChanged;
        Line.IsVisibleChanged += MainChildVisibleChanged;

        DataContext = VmManager.MainViewModel;

        //Task.Run(() =>
        //{
        //    while (true)
        //    {
        //        var mat = WeakReferenceMessenger.Default.Send<CaptureRequestMessage>().Response;

        //        if (mat is not null)
        //            Application.Current.Dispatcher.BeginInvoke(() =>
        //            {
        //                ImageViewer.ImageSource = mat.ToBitmapSource();
        //            });

        //        Thread.Sleep(10);
        //    }
        //});

        RegisterInvoke();
        RegisterMessage();
    }

    private void RegisterInvoke()
    {
        ImageViewer.OnMarkderChanged += (rect) => _markRect = rect;
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
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

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
      
        HardwareManager.Camera!.OnCaptureChanged += img =>
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    var source = img.ToBitmapSource();
                    ImageViewer.ImageSource = source;

                    Debug.WriteLine($"{source.Width} - {source.Height}");
                    //GC.Collect();
                });

            });

          
            Debug.WriteLine($"{HardwareManager.Camera.Exposure}");
        };
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
}
