using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
using Simscop.Pl.Core.Models.Charts.Constants;
using PlotCommands = OxyPlot.PlotCommands;

namespace Simscop.Pl.Ui
{
    /// <summary>
    /// Interaction logic for HeatmapChart.xaml
    /// </summary>
    public partial class HeatmapChart
    {
        private readonly DispatcherTimer _annotationTimer;

        private bool _showAnnotation;

        private Point _currentCursor;

        private Point _currentCoor;

        /// <summary>
        /// 
        /// </summary>
        public PlotModel PlotModel { get; } = new();

        /// <summary>
        /// 
        /// </summary>
        public LinearAxis AxisX { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LinearAxis AxisY { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LinearColorAxis ColorAxis { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public HeatMapSeries? Series { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public HeatmapChart()
        {
            InitializeComponent();

            var controller = new PlotController();
            controller.Unbind(PlotCommands.SnapTrack);
            controller.Unbind(PlotCommands.SnapTrackTouch);
            View.Controller = controller;

            BindingOperations.SetBinding(View, PlotViewBase.ModelProperty, new Binding() { Source = PlotModel });

            _annotationTimer = new(priority: DispatcherPriority.Render)
            {
                Interval = TimeSpan.FromMilliseconds(100),
            };
            _annotationTimer.Tick += (_, _) =>
            {
                _showAnnotation = true;
                _annotationTimer.Stop();
            };

            AxisX = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                MinimumPadding = 0,
                MaximumPadding = 0,
                IsZoomEnabled = false,
                IsPanEnabled = false
            };
            AxisY = new LinearAxis
            {
                Position = AxisPosition.Left,
                MinimumPadding = 0,
                MaximumPadding = 0,
                IsZoomEnabled = false,
                IsPanEnabled = false
            };
            ColorAxis = new LinearColorAxis
            {
                Position = AxisPosition.Right,
                Palette = OxyPalettes.Gray(256),
                HighColor = OxyColors.White,
                LowColor = OxyColors.Black
            };
            RefreshAxis();
        }

        void RefreshPlotModelBinding()
        {
            BindingOperations.ClearBinding(View, PlotViewBase.ModelProperty);
            BindingOperations.SetBinding(View, PlotViewBase.ModelProperty, new Binding() { Source = PlotModel });
        }

        void RefreshAxis()
        {
            PlotModel.Axes.Clear();

            PlotModel.Axes.Add(AxisX);
            PlotModel.Axes.Add(AxisY);
            PlotModel.Axes.Add(ColorAxis);
        }

        void RefreshSeries()
        {
            if (Series is null) return;

            PlotModel.Series.Clear();
            PlotModel.Series.Add(Series);
        }

        public static readonly DependencyProperty ColsProperty = DependencyProperty.Register(
            nameof(Cols), typeof(int), typeof(HeatmapChart), new PropertyMetadata(default(int)));

        public int Cols
        {
            get => (int)GetValue(ColsProperty);
            set => SetValue(ColsProperty, value);
        }

        public static readonly DependencyProperty RowsProperty = DependencyProperty.Register(
            nameof(Rows), typeof(int), typeof(HeatmapChart), new PropertyMetadata(default(int)));

        public int Rows
        {
            get => (int)GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }

        public static readonly DependencyProperty LabelFontSizeProperty = DependencyProperty.Register(
            nameof(LabelFontSize), typeof(double), typeof(HeatmapChart), new PropertyMetadata(0.3, OnDataChanged));

        public double LabelFontSize
        {
            get => (double)GetValue(LabelFontSizeProperty);
            set => SetValue(LabelFontSizeProperty, value);
        }

        public static readonly DependencyProperty PaletteProperty = DependencyProperty.Register(
            nameof(Palette), typeof(Palette), typeof(HeatmapChart), new PropertyMetadata(Palette.Gray, (s, d) =>
            {
                if (s is not HeatmapChart heatmap || d.NewValue is not Palette palette) return;

                heatmap.ColorAxis.Palette = palette switch
                {
                    Palette.BlueWhiteRed31 => OxyPalettes.BlueWhiteRed31,
                    Palette.Hot64 => OxyPalettes.Hot64,
                    Palette.Hue64 => OxyPalettes.Hue64,
                    Palette.BlackWhiteRed => OxyPalettes.BlackWhiteRed(256),
                    Palette.BlueWhiteRed => OxyPalettes.BlueWhiteRed(256),
                    Palette.Cool => OxyPalettes.Cool(256),
                    Palette.Gray => OxyPalettes.Gray(256),
                    Palette.Hot => OxyPalettes.Hot(256),
                    Palette.Hue => OxyPalettes.Hue(256),
                    Palette.HueDistinct => OxyPalettes.HueDistinct(256),
                    Palette.Jet => OxyPalettes.Jet(256),
                    Palette.Rainbow => OxyPalettes.Rainbow(256),
                    Palette.Cividis => OxyPalettes.Cividis(),
                    Palette.Inferno => OxyPalettes.Inferno(),
                    Palette.Magma => OxyPalettes.Magma(),
                    Palette.Plasma => OxyPalettes.Plasma(),
                    Palette.Viridis => OxyPalettes.Viridis(),
                    _ => throw new ArgumentOutOfRangeException()
                };
                heatmap.RefreshAxis();
                heatmap.RefreshPlotModelBinding();
            }));

        public Palette Palette
        {
            get => (Palette)GetValue(PaletteProperty);
            set => SetValue(PaletteProperty, value);
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            nameof(Data), typeof(double[,]), typeof(HeatmapChart), new PropertyMetadata(default(double[,]), OnDataChanged));

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not HeatmapChart heatmap
                || e.NewValue is not double[,] n
                || n.Length == 0
                || n.GetLength(0) != heatmap.Cols
                || n.GetLength(1) != heatmap.Rows) return;

            heatmap.Series = new HeatMapSeries
            {
                X0 = 0.5,
                X1 = heatmap.Cols - 0.5,
                Y0 = 0.5,
                Y1 = heatmap.Rows - 0.5,
                Data = n,
                Interpolate = false,
                RenderMethod = HeatMapRenderMethod.Bitmap,
                LabelFontSize = heatmap.Rows > 50 || heatmap.Cols > 50 ? 0 : heatmap.LabelFontSize,
            };

            heatmap.RefreshSeries();
            heatmap.UpdateTextAnnotation();
            heatmap.RefreshPlotModelBinding();
        }

        public double[,] Data
        {
            get => (double[,])GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        public static readonly DependencyProperty IsInterpolateProperty = DependencyProperty.Register(
            nameof(IsInterpolate), typeof(bool), typeof(HeatmapChart), new PropertyMetadata(default(bool)));

        public bool IsInterpolate
        {
            get => (bool)GetValue(IsInterpolateProperty);
            set => SetValue(IsInterpolateProperty, value);
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);

            _showAnnotation = false;
            _annotationTimer.Stop();

            PlotModel.Annotations.Clear();
            RefreshPlotModelBinding();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            _annotationTimer.Start();
        }

        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(
            nameof(SelectedIndex), typeof(System.Drawing.Point), typeof(HeatmapChart), new PropertyMetadata(default(System.Drawing.Point)));

        public System.Drawing.Point SelectedIndex
        {
            get => (System.Drawing.Point)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            var pos = e.GetPosition(this);
            var area = PlotModel.PlotArea;

            if (e.LeftButton != MouseButtonState.Pressed
                || !area.Contains(pos.X, pos.Y)) return;

            if (Series is null || Series.Data.Length == 0) return;

            var x = AxisX.InverseTransform(pos.X);
            var y = AxisY.InverseTransform(pos.Y);
            var coorX = (int)Math.Floor(x);
            var coorY = (int)Math.Floor(y);

            SelectedIndex = new System.Drawing.Point(coorX, coorY);
        }

        void UpdateTextAnnotation()
        {
            if (!_showAnnotation || Series?.Data is null) return;

            var pos = _currentCursor;

            var x = _currentCoor.X;
            var y = _currentCoor.Y;

            var coorX = (int)Math.Floor(x);
            var coorY = (int)Math.Floor(y);

            var array = Series.Data;
            if (array == null) return;

            var v = array[coorX, coorY];

            PlotModel.Annotations.Clear();

            var annotation = new TextAnnotation()
            {
                TextPosition = new DataPoint(x, y),
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                Text = $"{v}",
                StrokeThickness = 1,
                Padding = new OxyThickness(10),
                Background = OxyColors.White,
                TextColor = OxyColors.Black,
                FontSize = 14,
                FontWeight = 500,
            };
            PlotModel.Annotations.Add(annotation);
            RefreshPlotModelBinding();
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {

            var pos = e.GetPosition(this);
            var area = PlotModel.PlotArea;

            if (e.LeftButton != MouseButtonState.Pressed
                || !area.Contains(pos.X, pos.Y)) return;

            if (Series is null) return;
            if (Series.Data.Length == 0) return;

            var x = AxisX.InverseTransform(pos.X);
            var y = AxisY.InverseTransform(pos.Y);

            _currentCursor = pos;
            _currentCoor = new Point(x, y);

            UpdateTextAnnotation();
        }


    }
}
