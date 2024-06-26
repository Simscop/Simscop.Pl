﻿using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Lift.UI.Tools.Extension;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
using Simscop.Pl.Core.Models.Charts;
using Simscop.Pl.Ui.Charts;
using Simscop.Pl.Ui.Extensions;
using PlotCommands = OxyPlot.PlotCommands;
using TickStyle = OxyPlot.Axes.TickStyle;

namespace Simscop.Pl.Ui;

/// <summary>
/// Interaction logic for LineChart.xaml
/// </summary>
public partial class LineChart : UserControl
{
    private DispatcherTimer _resizeTimer;

    public LineChart()
    {
        InitializeComponent();

        var controller = new PlotController();
        controller.Unbind(PlotCommands.SnapTrack);
        controller.Unbind(PlotCommands.SnapTrackTouch);
        View.Controller = controller;

        BindingOperations.SetBinding(View, PlotViewBase.ModelProperty, new Binding() { Source = PlotModel });

        _resizeTimer = new DispatcherTimer();
        _resizeTimer.Interval = TimeSpan.FromSeconds(0.5); // 设置延迟执行的时间间隔
        _resizeTimer.Tick += _resizeTimer_Tick; ;
    }

    private void _resizeTimer_Tick(object? sender, EventArgs e)
    {
        _resizeTimer.Stop();

        PlotModel.Annotations.ForEach(item =>
        {
            if (item is not TextAnnotationExt ta) return;

            ta.Update();
        });
        UpdateAxis();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        // axis
        OnXAxisChanged();
        OnYAxisChanged();

        // panel
        OnPanelChanged();
    }

    void RefreshPlotModelBinding()
    {
        BindingOperations.ClearBinding(View, PlotViewBase.ModelProperty);
        BindingOperations.SetBinding(View, PlotViewBase.ModelProperty, new Binding() { Source = PlotModel });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="series"></param>
    /// <param name="index"></param>
    public void ShowSerial(Series series, int index)
    {
        if (PlotModel.Series.Count == index)
            PlotModel.Series.Add(series);

        if (PlotModel.Series.Count <= index)
            throw new Exception();
        else
            PlotModel.Series[index] = series;

        UpdateAxis();
    }

    void UpdatePanelGrid()
    {
        if ((Panel.GridStyle & GridStyle.Vertical) == GridStyle.Vertical)
        {
            AxisX.MajorGridlineStyle = LineStyle.Solid;
            AxisX.MinorGridlineStyle = LineStyle.Dot;
        }
        else
        {
            AxisX.MajorGridlineStyle = LineStyle.None;
            AxisX.MinorGridlineStyle = LineStyle.None;
        }

        if ((Panel.GridStyle & GridStyle.Horizontal) == GridStyle.Horizontal)
        {
            AxisY.MajorGridlineStyle = LineStyle.Solid;
            AxisY.MinorGridlineStyle = LineStyle.Dot;
        }
        else
        {
            AxisY.MajorGridlineStyle = LineStyle.None;
            AxisY.MinorGridlineStyle = LineStyle.None;
        }
    }

    void UpdateAxis()
    {
        PlotModel.Axes.Clear();
        PlotModel.Axes.Add(AxisX);
        PlotModel.Axes.Add(AxisY);

        UpdatePanelGrid();
        RefreshPlotModelBinding();
    }


    /// <summary>
    /// 
    /// </summary>
    public PlotModel PlotModel { get; } = new();

    /// <summary>
    /// 
    /// </summary>
    protected LinearAxis AxisX { get; set; } = new()
    {
        Position = AxisPosition.Bottom,
        MinimumPadding = 0,
        MaximumPadding = 0,
        IsZoomEnabled = false,
        IsPanEnabled = false
    };

    /// <summary>
    /// 
    /// </summary>
    protected LinearAxis AxisY { get; set; } = new()
    {
        Position = AxisPosition.Left,
        MinimumPadding = 0,
        MaximumPadding = 0,
        IsZoomEnabled = false,
        IsPanEnabled = false
    };

    #region XAxis

    public static readonly DependencyProperty XAxisModelProperty = DependencyProperty.Register(
        nameof(XAxisModel), typeof(AxisModel), typeof(LineChart), new PropertyMetadata(new AxisModel() { }, OnXAxisChanged));

    private static void OnXAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not LineChart lineChart) return;

        if (e.OldValue is AxisModel mo)
            mo.OnValueChanged -= lineChart.OnXAxisChanged;
        if (e.NewValue is AxisModel mn)
            mn.OnValueChanged += lineChart.OnXAxisChanged;
    }

    private void OnXAxisChanged()
    {
        AxisX = new LinearAxis
        {
            Position = AxisPosition.Bottom,
            Title = XAxisModel.Title,
            Unit = XAxisModel.Unit,
            IsAxisVisible = XAxisModel.IsVisible,
            IsZoomEnabled = XAxisModel.IsZoom,
            IsPanEnabled = XAxisModel.IsPanning,
            TickStyle = (TickStyle)XAxisModel.TickStyle,
            //Minimum = XAxisModel.ViewMinimum,
            //AbsoluteMinimum = XAxisModel.ViewMinimum,
            //Maximum = XAxisModel.ViewMaximum,
            //AbsoluteMaximum = XAxisModel.ViewMaximum,
        };


        UpdateAxis();
    }

    public AxisModel XAxisModel
    {
        get => (AxisModel)GetValue(XAxisModelProperty);
        set => SetValue(XAxisModelProperty, value);
    }

    #endregion

    #region YAxis

    public static readonly DependencyProperty YAxisModelProperty = DependencyProperty.Register(
        nameof(YAxisModel), typeof(AxisModel), typeof(LineChart), new PropertyMetadata(new AxisModel(), OnYAxisChanged));

    private static void OnYAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not LineChart lineChart) return;

        if (e.OldValue is AxisModel mo)
            mo.OnValueChanged -= lineChart.OnYAxisChanged;
        if (e.NewValue is AxisModel mn)
            mn.OnValueChanged += lineChart.OnYAxisChanged;
    }

    private void OnYAxisChanged()
    {
        AxisY = new LinearAxis()
        {
            Position = AxisPosition.Left,
            Title = YAxisModel.Title,
            Unit = YAxisModel.Unit,
            IsAxisVisible = YAxisModel.IsVisible,
            IsZoomEnabled = YAxisModel.IsZoom,
            IsPanEnabled = YAxisModel.IsPanning,
            TickStyle = (TickStyle)YAxisModel.TickStyle,
            //Minimum = YAxisModel.ViewMinimum,
            //AbsoluteMinimum = YAxisModel.ViewMinimum,
            //Maximum = YAxisModel.ViewMaximum,
            //AbsoluteMaximum = YAxisModel.ViewMaximum,
        };

        //AxisY.MaximumRange=20;
        UpdateAxis();
    }


    public AxisModel YAxisModel
    {
        get => (AxisModel)GetValue(YAxisModelProperty);
        set => SetValue(YAxisModelProperty, value);
    }


    #endregion

    #region Panel

    public static readonly DependencyProperty PanelProperty = DependencyProperty.Register(
        nameof(Panel), typeof(PanelModel), typeof(LineChart), new PropertyMetadata(new PanelModel(), OnPanelChanged));

    private static void OnPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not LineChart lineChart) return;


        if (e.OldValue is PanelModel mo)
            mo.OnValueChanged -= lineChart.OnPanelChanged;
        if (e.NewValue is PanelModel mn)
            mn.OnValueChanged += lineChart.OnPanelChanged;
    }

    private void OnPanelChanged()
    {
        PlotModel.PlotMargins = new OxyThickness(
            Panel.Margin.V1 <= 0 ? double.NaN : Panel.Margin.V1,
            Panel.Margin.V2 <= 0 ? double.NaN : Panel.Margin.V2,
            Panel.Margin.V3 <= 0 ? double.NaN : Panel.Margin.V3,
            Panel.Margin.V4 <= 0 ? double.NaN : Panel.Margin.V4);

        PlotModel.Padding = new OxyThickness(
            Panel.Padding.V1 <= 0 ? 6 : Panel.Margin.V1,
            Panel.Padding.V2 <= 0 ? 6 : Panel.Margin.V2,
            Panel.Padding.V3 <= 0 ? 6 : Panel.Margin.V3,
            Panel.Padding.V4 <= 0 ? 6 : Panel.Margin.V4);

        AxisX.MinimumPadding = Panel.AxisMarginScale.V1 <= 0 ? 0 : Panel.AxisMarginScale.V1;
        AxisX.MaximumPadding = Panel.AxisMarginScale.V3 <= 0 ? 0 : Panel.AxisMarginScale.V3;
        AxisY.MaximumPadding = Panel.AxisMarginScale.V2 <= 0 ? 0 : Panel.AxisMarginScale.V2;
        AxisY.MinimumPadding = Panel.AxisMarginScale.V4 <= 0 ? 0 : Panel.AxisMarginScale.V4;

        PlotModel.Title = Panel.Title;
        PlotModel.Subtitle = Panel.Subtitle;
        PlotModel.TitlePadding = Panel.TitlePadding;

        UpdateAxis();
    }

    public PanelModel Panel
    {
        get => (PanelModel)GetValue(PanelProperty);
        set => SetValue(PanelProperty, value);
    }

    #endregion

    #region Annotation

    private readonly List<Annotation> _annotations = new();

    private LineAnnotation _realLineAnnotation = new();

    public static readonly DependencyProperty AnnotationProperty = DependencyProperty.Register(
        nameof(Annotation), typeof(AnnotationModel), typeof(LineChart), new PropertyMetadata(new AnnotationModel(), OnAnnotationChanged));

    private static void OnAnnotationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not LineChart lineChart) return;


        if (e.OldValue is AnnotationModel mo)
            mo.OnValueChanged -= lineChart.OnAnnotationChanged;
        if (e.NewValue is AnnotationModel mn)
            mn.OnValueChanged += lineChart.OnAnnotationChanged;
    }

    private void OnAnnotationChanged()
    {
        _annotations.ForEach(item =>
        {
            try
            {
                switch (item)
                {
                    case TextAnnotationExt ta:
                        ta.FontSize = Annotation.FontSize;
                        ta.FontWeight = Annotation.FontWeight;
                        ta.FormatString = Annotation.Format;
                        ta.TextColor = Annotation.TextColor.ToOxyColor();
                        ta.StrokeThickness = 0;
                        ta.Update();
                        break;
                    case LineAnnotation la:
                        la.StrokeThickness = Annotation.Thickness;
                        la.LineStyle = (LineStyle)Annotation.LineSytle;
                        la.Color = Annotation.LineColor.ToOxyColor();
                        break;
                    case PointAnnotation pa:
                        pa.Fill = Annotation.PointColor.ToOxyColor();
                        pa.Size = Annotation.PointSize;
                        break;
                    default:
                        return;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        });

        UpdateAxis();
    }

    public AnnotationModel Annotation
    {
        get => (AnnotationModel)GetValue(AnnotationProperty);
        set => SetValue(AnnotationProperty, value);
    }

    #endregion

    #region toolkits

    bool TransformFromPoint(MouseEventArgs e, out Point coordinate) => TransformFromPoint(e.GetPosition(this), out coordinate);

    bool TransformFromPoint(Point point, out Point coordinate)
    {
        coordinate = new Point(double.NaN, double.NaN);

        var aera = PlotModel.PlotArea;
        if (!aera.Contains(point.X, point.Y)) return false;

        var xRange = AxisX.ActualMaximum - AxisX.ActualMinimum;
        var yRange = AxisY.ActualMaximum - AxisY.ActualMinimum;

        var x = AxisX.ActualMinimum + xRange * (point.X - aera.Left) / aera.Width;
        var y = AxisY.ActualMinimum + yRange * (aera.Height - (point.Y - aera.Top)) / aera.Height;


        coordinate = new Point(x, y);
        return true;
    }

    /// <summary>
    /// 实际的左边长度对应的控件的像素长度
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns></returns>
    double Pixel2XCoordinate(double coordinate)
    {
        var xRange = AxisX.ActualMaximum - AxisX.ActualMinimum;
        var aera = PlotModel.PlotArea;

        var scale = xRange / aera.Width;
        return coordinate * scale;
    }


    double Pixel2YCoordinate(double coordinate)
    {
        var aera = PlotModel.PlotArea;
        var yRange = AxisY.ActualMaximum - AxisY.ActualMinimum;

        var scale = yRange / aera.Width;
        return coordinate * scale;
    }
    #endregion


    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);

        _resizeTimer.Stop();
        _resizeTimer.Start();
    }

    void RemoveUselessAnnotations()
    {
        PlotModel.Annotations.Clear();
        _annotations.ForEach(item => PlotModel.Annotations.Add(item));
    }

    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseLeftButtonUp(e);

        RemoveUselessAnnotations();
        UpdateAxis();
    }

    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
        //base.OnPreviewMouseMove(e);

        var pos = e.GetPosition(this);

        var area = PlotModel.PlotArea;

        if (e.LeftButton == MouseButtonState.Pressed && area.Contains(pos.X, pos.Y))
        {
            //return;
            TransformFromPoint(e, out var coor);
            RemoveUselessAnnotations();
            PlotModel.Series.ForEach(item =>
            {
                if (item is not LineSeries series || string.IsNullOrWhiteSpace(series.TrackerFormatString)) return;


                var closeY = series.Points.OrderBy(p => Math.Abs(p.X - coor.X)).FirstOrDefault().Y;
                var close = item.GetNearestPoint(new ScreenPoint(e.GetPosition(this).X, AxisY.Transform(closeY)), true)
                    .DataPoint;

                PlotModel.Annotations.Add(new LineAnnotation()
                {
                    X = coor.X,
                    Type = LineAnnotationType.Vertical,
                    Color = OxyColors.Black,
                    StrokeThickness = 1,
                    LineStyle = LineStyle.Dot
                });

                PlotModel.Annotations.Add(new LineAnnotation()
                {
                    Y = close.Y,
                    Type = LineAnnotationType.Horizontal,
                    Color = OxyColors.Black,
                    StrokeThickness = 1,
                    LineStyle = LineStyle.Dot
                });

                var annotation = new TextAnnotationExt()
                {
                    TransformX = Pixel2XCoordinate,
                    TransformY = Pixel2YCoordinate,
                    TargetX = close.X,
                    TargetY = close.Y,
                    OffsetX = series.LabelMargin,
                    OffsetY = series.LabelMargin,
                    TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                    TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                    Text = series.TrackerFormatString.Format(series.Title, AxisX.Title, close.X, AxisY.Title, close.Y),
                    StrokeThickness = 0,
                    TextColor = OxyColors.Black,
                    FontSize = series.FontSize,
                    FontWeight = series.FontWeight,
                };
                annotation.Update();
                PlotModel.Annotations.Add(annotation);
            });

            UpdateAxis();
        }

        e.Handled = false;
    }

    public static readonly DependencyProperty SelectedXProperty = DependencyProperty.Register(
        nameof(SelectedX), typeof(double), typeof(LineChart), new PropertyMetadata(default(double)));

    public double SelectedX
    {
        get => (double)GetValue(SelectedXProperty);
        set => SetValue(SelectedXProperty, value);
    }

    private Point _doublePositionPoint = new();
    private Point _doubleTransformPoint = new();

    private bool _doubleFlag = false;

    protected override void OnPreviewMouseDoubleClick(MouseButtonEventArgs e)
    {
        if (!(e.ChangedButton == MouseButton.Left && TransformFromPoint(e, out _doubleTransformPoint)))
        {
            e.Handled = true;
            return;
        }

        _doublePositionPoint = e.GetPosition(this);

        _doubleFlag = true;

        // 仅允许一个x的annotation
        UpdateAnnotation();
    }

    public void UpdateAnnotation()
    {
        if (!_doubleFlag) return;

        var render = _doublePositionPoint;
        var coor = _doubleTransformPoint;

        _realLineAnnotation = new LineAnnotation() { MaximumY = double.NaN };
        _annotations.Clear();
        PlotModel.Annotations.Clear();

        var series = PlotModel.Series.FirstOrDefault(item => item is LineSeries) as LineSeries;
        if (series is null) return;

        var y = series.Points.OrderBy(p => Math.Abs(p.X - coor.X)).FirstOrDefault().Y;
        var select = series.GetNearestPoint(new ScreenPoint(render.X, AxisY.Transform(y)), true).DataPoint;

        SelectedX = select.X;

        var text = new TextAnnotationExt()
        {
            TargetX = select.X,
            TargetY = select.Y,
            SerialTitle = series.Title,
            TitleX = AxisX.Title,
            TitleY = AxisY.Title,
            FormatString = Annotation.Format,
            TransformX = Pixel2XCoordinate,
            TransformY = Pixel2YCoordinate,

            TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
            TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
        };
        text.Update();
        PlotModel.Annotations.Add(text);
        _annotations.Add(text);

        if (_realLineAnnotation.MaximumY.IsNaN()
            || _realLineAnnotation.MaximumY < select.Y)
        {
            var realLineAnnotation = new LineAnnotation()
            {
                X = coor.X,
                Type = LineAnnotationType.Vertical,
                StrokeThickness = 2,
                MaximumY = select.Y,
                LineStyle = LineStyle.Automatic,
                Color = OxyColors.Red,
            };

            PlotModel.Annotations.Add(realLineAnnotation);
            _annotations.Add(realLineAnnotation);

            if (!_realLineAnnotation.MaximumY.IsNaN())
            {
                PlotModel.Annotations.Remove(_realLineAnnotation);
                _annotations.Remove(_realLineAnnotation);
            }

            _realLineAnnotation = realLineAnnotation;
        }

        var dot = new PointAnnotation()
        {
            X = select.X,
            Y = select.Y,
            Fill = OxyColors.Red,
        };

        PlotModel.Annotations.Add(dot);
        _annotations.Add(dot);


        OnAnnotationChanged();


        //}
        //catch (InvalidOperationException _) { }
    }
}
