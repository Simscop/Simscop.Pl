using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot.Wpf;
using Simscop.Pl.Core.Models.Charts;
using Simscop.Pl.Ui.Charts;
using TickStyle = OxyPlot.Axes.TickStyle;

namespace Simscop.Pl.Ui;

/// <summary>
/// Interaction logic for LineChart.xaml
/// </summary>
public partial class LineChart : UserControl
{
    public LineChart()
    {
        InitializeComponent();

        var line = new LineSeries()
        {
            Title = "line1",
            Tag = "tag",
            CanTrackerInterpolatePoints = true,
            MarkerType = MarkerType.None,
            MarkerSize = 2,
        };
        for (var i = 0; i < 100; i++)
            line.Points.Add(new DataPoint(i * 0.1, Math.Cos(i * 0.1) * 10));

        //line.LineLegendPosition = LineLegendPosition.Start;
        //line.LabelFormatString = "start";
        line.TrackerFormatString = "x: {2:0.########}\ny: {4:0.########}";
        line.Decimator = Decimator.Decimate;

        PlotModel.Series.Add(line);

        line.MouseDown += (s, e) =>
        {
            //Debug.WriteLine(line.GetNearestPoint(e.Position, true).DataPoint);
        };

        this.MouseDown += (s, e) =>
        {
            var pos = e.GetPosition(this);
            Debug.WriteLine(line.GetNearestPoint(new ScreenPoint(pos.X, pos.Y), true).DataPoint);

        };



        //var legend1 = new Legend();
        //legend1.LegendSymbolLength = 24;
        //PlotModel.Legends.Add(legend1);

        BindingOperations.SetBinding(View, PlotViewBase.ModelProperty, new Binding() { Source = PlotModel });
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        // axis
        OnXAxisChanged();
        OnYAxisChanged();
        UpdateAxis();

        // panel
        OnPanelChanged();
    }

    void UpdateBindingPlotModel()
    {
        BindingOperations.ClearBinding(View, PlotViewBase.ModelProperty);
        BindingOperations.SetBinding(View, PlotViewBase.ModelProperty, new Binding() { Source = PlotModel });
    }


    void UpdatePanelGrid()
    {
        if ((Panel.GridStyle & GridStyle.Vertical) == GridStyle.Vertical)
        {
            XAxis.MajorGridlineStyle = LineStyle.Solid;
            XAxis.MinorGridlineStyle = LineStyle.Dot;
        }
        else
        {
            XAxis.MajorGridlineStyle = LineStyle.None;
            XAxis.MinorGridlineStyle = LineStyle.None;
        }

        if ((Panel.GridStyle & GridStyle.Horizontal) == GridStyle.Horizontal)
        {
            YAxis.MajorGridlineStyle = LineStyle.Solid;
            YAxis.MinorGridlineStyle = LineStyle.Dot;
        }
        else
        {
            YAxis.MajorGridlineStyle = LineStyle.None;
            YAxis.MinorGridlineStyle = LineStyle.None;
        }
    }

    void UpdateAxis()
    {
        PlotModel.Axes.Clear();
        PlotModel.Axes.Add(XAxis);
        PlotModel.Axes.Add(YAxis);

        UpdatePanelGrid();
        UpdateBindingPlotModel();
    }


    /// <summary>
    /// 
    /// </summary>
    public PlotModel PlotModel { get; } = new() { };

    /// <summary>
    /// 
    /// </summary>
    protected LinearAxis XAxis { get; set; } = new()
    {
        Position = AxisPosition.Bottom
    };

    /// <summary>
    /// 
    /// </summary>
    protected LinearAxis YAxis { get; set; } = new()
    {
        Position = AxisPosition.Left
    };


    #region XAxis

    public static readonly DependencyProperty XAxisModelProperty = DependencyProperty.Register(
        nameof(XAxisModel), typeof(AxisModel), typeof(LineChart), new PropertyMetadata(new AxisModel(), OnXAxisChanged));

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
        XAxis = new LinearAxis
        {
            Position = AxisPosition.Bottom,
            Title = XAxisModel.Title,
            Unit = XAxisModel.Unit,
            IsAxisVisible = XAxisModel.IsVisible,
            IsZoomEnabled = XAxisModel.IsZoom,
            IsPanEnabled = XAxisModel.IsPanning,
            TickStyle = (TickStyle)XAxisModel.TickStyle,
            AbsoluteMinimum = XAxisModel.ViewMinimum,
            AbsoluteMaximum = XAxisModel.ViewMaximum
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
        YAxis = new LinearAxis()
        {
            Position = AxisPosition.Left,
            Title = YAxisModel.Title,
            Unit = YAxisModel.Unit,
            IsAxisVisible = YAxisModel.IsVisible,
            IsZoomEnabled = YAxisModel.IsZoom,
            IsPanEnabled = YAxisModel.IsPanning,
            TickStyle = (TickStyle)YAxisModel.TickStyle,
            AbsoluteMinimum = YAxisModel.ViewMinimum,
            AbsoluteMaximum = YAxisModel.ViewMaximum
        };

        //YAxis.MaximumRange=20;


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
        PlotModel.PlotMargins = Panel.Margin.IsNegative()
            ? new OxyThickness(double.NaN)
            : new OxyThickness(Panel.Margin.V1, Panel.Margin.V2, Panel.Margin.V3, Panel.Margin.V4);
        PlotModel.Padding = Panel.Padding.IsNegative()
            ? new OxyThickness(8)
            : new OxyThickness(Panel.Padding.V1, Panel.Padding.V2, Panel.Padding.V3, Panel.Padding.V4);

        XAxis.MinimumPadding = Panel.AxisMarginScale.V1;
        XAxis.MaximumPadding = Panel.AxisMarginScale.V3;
        YAxis.MaximumPadding = Panel.AxisMarginScale.V2;
        YAxis.MinimumPadding = Panel.AxisMarginScale.V4;

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

    #region toolkits

    bool TransformFromPoint(MouseEventArgs e, out Point coordinate) => TransformFromPoint(e.GetPosition(this), out coordinate);

    bool TransformFromPoint(Point point, out Point coordinate)
    {
        coordinate = new Point(double.NaN, double.NaN);

        var aera = PlotModel.PlotArea;
        if (!aera.Contains(point.X, point.Y)) return false;

        var xRange = XAxis.ActualMaximum - XAxis.ActualMinimum;
        var yRange = YAxis.ActualMaximum - YAxis.ActualMinimum;

        var x = XAxis.ActualMinimum + xRange * (point.X - aera.Left) / aera.Width;
        var y = YAxis.ActualMinimum + yRange * (aera.Height - (point.Y - aera.Top)) / aera.Height;


        coordinate = new Point(x, y);
        return true;
    }

    #endregion

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        //Debug.WriteLine(e.GetPosition(this));

        //if (!TransformFromPoint(e, out var coordinate)) return;

        //var line = PlotModel.Series[0];
        //Debug.WriteLine(line.GetNearestPoint(new ScreenPoint(e.GetPosition(this).X, e.GetPosition(this).Y), true));


    }


    protected override void OnPreviewMouseDoubleClick(MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left && TransformFromPoint(e, out var point))
        {

            PlotModel.Annotations.Clear();
            PlotModel.Annotations.Add(new TextAnnotation()
            {
                Text = $"{point.X:##.###} - {point.Y::##.###}",
                TextPosition = new DataPoint(point.X, point.Y),
            });
            PlotModel.Annotations.Add(new LineAnnotation()
            {
                X = point.X,
                Type = LineAnnotationType.Vertical,
                MinimumY = YAxis.ActualMinimum,
                StrokeThickness = 2,
                MaximumY = point.Y,
            });

            Debug.WriteLine("ok");
            UpdateAxis();
        }
    }
}
