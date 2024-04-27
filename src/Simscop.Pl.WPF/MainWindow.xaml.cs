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
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Lift.UI.Controls;
using Lift.UI.Tools;
using Lift.UI.Tools.Extension;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using OxyPlot;
using OxyPlot.Series;
using Simscop.Pl.Core;
using Simscop.Pl.Ui.Extensions;
using Simscop.Pl.WPF.Managers;
using Simscop.Pl.WPF.Views;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;
using WindowHelper = Simscop.Pl.WPF.Helpers.WindowHelper;

namespace Simscop.Pl.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        ImageViewer.IsVisibleChanged += MainChildVisibleChanged;
        Heatmap.IsVisibleChanged += MainChildVisibleChanged;
        Line.IsVisibleChanged += MainChildVisibleChanged;

        var size = 500;

        Heatmap.Cols = size;
        Heatmap.Rows = size;
        Heatmap.LabelFontSize = 0;
        var data = new double[size, size];

        var random = new Random();
        for (var i = 0; i < size; i++)
            for (var j = 0; j < size; j++)
                data[i, j] = random.NextDouble() * 100;

        Heatmap.Data = data;

        DataContext = VmManager.MainViewModel;

        Task.Run(() =>
        {
            while (true)
            {
                var mat = WeakReferenceMessenger.Default.Send<CaptureRequestMessage>().Response;
                if (mat is null) return;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ImageViewer.ImageSource = BitmapFrame.Create(mat.ToBitmapSource());
                });

            }
        });
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        Line.DataContext = VmManager.LineChartViewModel;

        VmManager.LineChartViewModel.OnValueChanged += (n) =>
        {
            if (n == nameof(VmManager.LineChartViewModel.Serial))
            {
                if (VmManager.LineChartViewModel.Serial is not LineSeries series) return;
                Line.ShowSerial(series, 0);
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

        var points = Enumerable.Range(0, 1000)
            .Select(item => (item * 1.0, Math.Cos(item * 0.01)))
            .ToList();

        VmManager.LineChartViewModel.Data = points;
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
}
