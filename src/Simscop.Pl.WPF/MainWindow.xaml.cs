﻿using System.Diagnostics;
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
using Simscop.Pl.Ui;
using Simscop.Pl.Ui.Extensions;
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
}
