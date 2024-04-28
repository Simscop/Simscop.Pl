using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Lift.UI.Tools;
using Lift.UI.Tools.Command;
using Lift.UI.Tools.Extension;
using OpenCvSharp.Flann;
using OxyPlot.Series;
using Simscop.Pl.Ui.ImageEx.ShapeEx;

namespace Simscop.Pl.Ui.ImageEx;

/// <summary>
/// 关于浏览相关的行为
/// </summary>
public class ImageExViewerBehavior : Behavior<ImageEx>
{
    protected override void OnAttached()
    {
        AssociatedObject.MainPanel!.MouseWheel += OnZoomChanged;
        AssociatedObject.Scroll!.MouseMove += OnMoveChanged;
        AssociatedObject.Scroll!.PreviewMouseDown += OnMoveStart;
        AssociatedObject.Scroll!.PreviewMouseUp += OnMoveStop;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.MainPanel!.MouseWheel -= OnZoomChanged;
        AssociatedObject.Scroll!.MouseMove -= OnMoveChanged;
        AssociatedObject.Scroll!.PreviewMouseDown -= OnMoveStart;
        AssociatedObject.Scroll!.PreviewMouseUp -= OnMoveStop;
    }

    private void OnZoomChanged(object sender, MouseWheelEventArgs e)
    {
        var oldScale = AssociatedObject.ImagePanelScale;

        // 放大倍数使用放大十倍以内为0.1，当大100倍以内为1，以此内推
        var scale = Math.Log10(AssociatedObject.ImagePanelScale / AssociatedObject.DefaultImagePanelScale);

        scale = (scale <= 0 ? 0.1 : Math.Pow(10, Math.Floor(scale))) * AssociatedObject.DefaultImagePanelScale;

        if (e.Delta <= 0)
        {
            AssociatedObject.ImagePanelScale -= AssociatedObject.DefaultImagePanelScale * scale;

            // 最小为缩小10倍
            if (AssociatedObject.ImagePanelScale <= AssociatedObject.DefaultImagePanelScale * 0.1)
                AssociatedObject.ImagePanelScale = AssociatedObject.DefaultImagePanelScale * 0.1;
        }
        else
            AssociatedObject.ImagePanelScale += AssociatedObject.DefaultImagePanelScale * scale;

        // update the offset
        if (AssociatedObject.ImagePanelScale <= AssociatedObject.DefaultImagePanelScale) return;

        var transform = AssociatedObject.ImagePanelScale / oldScale;
        var pos = e.GetPosition(AssociatedObject.Box);
        var target = new Point(pos.X * transform, pos.Y * transform);
        var offset = target - pos;

        AssociatedObject.Scroll!.ScrollToHorizontalOffset(AssociatedObject.Scroll.HorizontalOffset + offset.X);
        AssociatedObject.Scroll!.ScrollToVerticalOffset(AssociatedObject.Scroll.VerticalOffset + offset.Y);
    }

    private DateTime _flag = DateTime.Now;

    private Point _cursor = new(-1, -1);


    private void OnMoveStart(object sender, MouseButtonEventArgs e)
    {
        _cursor = e.GetPosition(AssociatedObject);
    }

    private void OnMoveStop(object sender, MouseButtonEventArgs e)
    {
        _cursor = new(-1, -1);
    }

    private void OnMoveChanged(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed) return;
        if (_cursor.X < 0 || _cursor.Y < 0) return;

        var pos = e.GetPosition(AssociatedObject);
        var offset = pos - _cursor;
        _cursor = pos;

        AssociatedObject.Scroll!.ScrollToHorizontalOffset(AssociatedObject.Scroll.HorizontalOffset - offset.X);
        AssociatedObject.Scroll.ScrollToVerticalOffset(AssociatedObject.Scroll.VerticalOffset - offset.Y);
    }
}

public class ImageExDrawBehavior : Behavior<ImageEx>
{
    protected override void OnAttached()
    {
        AssociatedObject.Loaded += OnLoaded;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.Canvas!.PreviewMouseMove -= OnCanvasPreviewMouseMove;
        AssociatedObject.Canvas!.PreviewMouseDown -= OnCanvasPreviewMouseDown;
        AssociatedObject.Canvas!.MouseLeave -= OnCanvasMouseLeave;
        AssociatedObject.MainPanel!.PreviewMouseUp -= OnCanvasPreviewMouseUp;


        AssociatedObject.Loaded -= OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        AssociatedObject.Canvas!.PreviewMouseMove += OnCanvasPreviewMouseMove;
        AssociatedObject.Canvas!.PreviewMouseDown += OnCanvasPreviewMouseDown;
        AssociatedObject.Canvas!.MouseLeave += OnCanvasMouseLeave;
        AssociatedObject.MainPanel!.PreviewMouseUp += OnCanvasPreviewMouseUp;
    }

    private bool _flag = false;

    private void OnCanvasMouseLeave(object sender, MouseEventArgs e) { }

    private void OnCanvasPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (!_flag) return;
        _flag = false;

        AssociatedObject.ShapePreviewer!.Visibility = Visibility.Collapsed;
        AssociatedObject.Canvas!.Cursor = Cursors.Arrow;

        // valid location
        if (!ValidLocation(e)) return;

        // valid size
        var min = Math.Min(AssociatedObject.ShapePreviewer!.Height, AssociatedObject.ShapePreviewer!.Width);
        var threshold = Math.Min(AssociatedObject.ImageSource!.Height, AssociatedObject.ImageSource!.Width) * 0.02;

        if (threshold > min) return;

        // render
        AssociatedObject.ShapePreviewer!.PointEnd = e.GetPosition(AssociatedObject.Canvas);

        // add shape
        //var rec = AssociatedObject.ShapePreviewer!.Clone();
        //AssociatedObject.ShapeCollection.Add(rec);
        AssociatedObject.ShapeMarker!.PointStart = AssociatedObject.ShapePreviewer!.PointStart;
        AssociatedObject.ShapeMarker!.PointEnd = AssociatedObject.ShapePreviewer!.PointEnd;

        AssociatedObject.ShapeMarker!.Refresh();

        var marker = AssociatedObject.ShapeMarker;
        var start = marker.PointStart;
        var end = marker.PointEnd;

        var location = new Point(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));

        AssociatedObject.OnMarkderChanged?.Invoke(new Rect(location.X, location.Y, marker.Width, marker.Height));
    }

    private void OnCanvasPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed
            || e.RightButton != MouseButtonState.Pressed) return;

        _flag = true;

        AssociatedObject.Canvas!.Cursor = Cursors.Cross;
        AssociatedObject.ShapePreviewer!.PointStart = e.GetPosition(AssociatedObject.Canvas);
    }

    private void OnCanvasPreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed
            || e.RightButton != MouseButtonState.Pressed) return;

        AssociatedObject.ShapePreviewer!.PointEnd = e.GetPosition(AssociatedObject.Canvas);

        AssociatedObject.ShapePreviewer!.Visibility = Visibility.Visible;
        AssociatedObject.ShapePreviewer!.Refresh();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <returns>
    /// true - 合法
    /// </returns>
    bool ValidLocation(MouseEventArgs e)
    {
        // valid location
        var pos = e.GetPosition(AssociatedObject.Canvas);
        var width = AssociatedObject.Canvas!.ActualWidth;
        var height = AssociatedObject.Canvas!.ActualHeight;

        return !(pos.X < 0 || pos.Y < 0 || pos.X > width || pos.Y > height);
    }
}

[TemplatePart(Name = NamePartMainPanel, Type = typeof(Panel))]
[TemplatePart(Name = NamePartScrollView, Type = typeof(ScrollViewer))]
[TemplatePart(Name = NamePartViewBox, Type = typeof(Viewbox))]
[TemplatePart(Name = NamePartCanvas, Type = typeof(InkCanvas))]
[TemplatePart(Name = NamePartShapePreviewer, Type = typeof(ShapeBase))]
[TemplatePart(Name = NamePartShapeMarkder, Type = typeof(ShapeBase))]
public class ImageEx : ContentControl
{
    #region Name

    public const string NamePartMainPanel = "PART_MAIN_PANEL";

    public const string NamePartScrollView = "PART_SCROLL";

    public const string NamePartViewBox = "PART_BOX";

    public const string NamePartCanvas = "PART_CANVAS";

    public const string NamePartShapePreviewer = "PART_SHAPE_PREVIEWER";

    public const string NamePartShapeMarkder = "PART_SHAPE_MARKER";

    #endregion

    #region Part

    internal Panel? MainPanel;

    internal ScrollViewer? Scroll;

    internal Viewbox? Box;

    internal InkCanvas? Canvas;

    internal ShapeBase? ShapePreviewer;

    internal ShapeBase? ShapeMarker;

    #endregion

    #region Commands

    /// <summary>
    /// 删除当前标记
    /// </summary>
    public const string Delete = "Delete";

    /// <summary>
    /// 放缩到当前标记
    /// </summary>
    public const string ZoomScale = "ZoomScale";

    /// <summary>
    /// Markder相关路由
    /// </summary>
    public static readonly RoutedUICommand MarkerCommand = new RoutedUICommand();

    #endregion

    #region Events

    /// <summary>
    /// 返回相对Image的坐标
    /// </summary>
    public Action<Rect>? OnMarkderChanged;

    public Action<(int X, int Y, Color C)>? OnCursorChanged;

    public Func<int, int, Color>? GetImageColorFromPosition;

    #endregion

    private readonly List<Behavior> _behaviors = new();

    static ImageEx()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageEx)
            , new FrameworkPropertyMetadata(typeof(ImageEx)));
    }

    public ImageEx()
    {
        _behaviors.Add(new ImageExViewerBehavior());
        _behaviors.Add(new ImageExDrawBehavior());

        //ShapeCollection.CollectionChanged += (_, _) =>
        //{
        //    if (Canvas is null) return;

        //    // append
        //    ShapeCollection.SkipWhile(item => Canvas.Children.Contains(item)).ForEach(item => item.Draw(Canvas));
        //};

        CommandBindings.Add(new CommandBinding(MarkerCommand, (obj, args) =>
        {
            if (args.Parameter is not string command || ShapeMarker is null) return;

            if (command == Delete) ShapeMarker.Visibility = Visibility.Collapsed;
            else if (command == ZoomScale) return;
            else throw new NotImplementedException();
        }));
    }


    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        MainPanel = Template.FindName(NamePartMainPanel, this) as Panel;
        Scroll = Template.FindName(NamePartScrollView, this) as ScrollViewer;
        Box = Template.FindName(NamePartViewBox, this) as Viewbox;
        Canvas = Template.FindName(NamePartCanvas, this) as InkCanvas;
        ShapePreviewer = Template.FindName(NamePartShapePreviewer, this) as ShapeBase;
        ShapeMarker = Template.FindName(NamePartShapeMarkder, this) as ShapeBase;

        _behaviors.ForEach(b => b.Attach(this));

        Canvas!.PreviewMouseMove += OnCanvasCursorChagned;
    }

    private void OnCanvasCursorChagned(object sender, MouseEventArgs e)
    {
        base.OnPreviewMouseMove(e);

        if (ImageSource is null || GetImageColorFromPosition is null) return;

        var pos = e.GetPosition(Canvas);
        var x = (int)Math.Floor(pos.X);
        var y = (int)Math.Floor(pos.Y);

        var c = GetImageColorFromPosition(x, y);
        OnCursorChanged?.Invoke((x, y, c));
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        UpdateImageInfo();
        UpdateImage();
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);

        UpdateImageInfo();
        UpdateImage();
    }

    public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
        nameof(ImageSource), typeof(ImageSource), typeof(ImageEx), new PropertyMetadata(null,
            (o, p) =>
        {
            if (o is not ImageEx ex) return;
            if (p.NewValue is not BitmapSource s2) return;

            ex.UpdateImageInfo();

            if (p.OldValue is BitmapSource s1
                && Math.Abs(s1.Width - s2.Width) < 0.001
                && Math.Abs(s1.Height - s2.Height) < 0.001)
                ex.UpdateImage(false);

            ex.UpdateImage();
        }));

    public ImageSource? ImageSource
    {
        get => (ImageSource?)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public static readonly DependencyProperty ShapeCollectionProperty = DependencyProperty.Register(
        nameof(ShapeCollection), typeof(ObservableCollection<ShapeBase>), typeof(ImageEx), new PropertyMetadata(new ObservableCollection<ShapeBase>(), OnShapeChanged));

    private static void OnShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageEx ex || ex.Canvas is null) return;

        // do some func for update the render
        if (e.OldValue is not ObservableCollection<ShapeBase> oVal ||
            e.NewValue is not ObservableCollection<ShapeBase> nVal) return;

        // update
        oVal.ForEach(item => item?.Clear(ex.Canvas));
        nVal.ForEach(item => item?.Draw(ex.Canvas));
    }

    public ObservableCollection<ShapeBase> ShapeCollection
    {
        get => (ObservableCollection<ShapeBase>)GetValue(ShapeCollectionProperty);
        set => SetValue(ShapeCollectionProperty, value);
    }

    public static readonly DependencyProperty MarkerMenuProperty = DependencyProperty.Register(
        nameof(MarkerMenu), typeof(ContextMenu), typeof(ImageEx), new PropertyMetadata(default(ContextMenu)));

    public ContextMenu MarkerMenu
    {
        get => (ContextMenu)GetValue(MarkerMenuProperty);
        set => SetValue(MarkerMenuProperty, value);
    }

    #region Render Size Info

    internal double DefaultImagePanelScale = 0;

    internal (double Width, double Height) DefaultImageSize;

    public static readonly DependencyProperty ImagePanelScaleProperty = DependencyProperty.Register(
        nameof(ImagePanelScale), typeof(double), typeof(ImageEx), new PropertyMetadata((double)-1, OnImagePanelScale));

    private static void OnImagePanelScale(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageEx ex || e.NewValue is not double n || n <= 0) return;
        if (ex.Box is null) return;

        ex.Box.Height = ex.DefaultImageSize.Height * ex.ImagePanelScale;
        ex.Box.Height = ex.DefaultImageSize.Height * ex.ImagePanelScale;
    }

    [Browsable(true)]
    [Category("SizeInfo")]
    [ReadOnly(true)]
    public double ImagePanelScale
    {
        get => (double)GetValue(ImagePanelScaleProperty);
        set => SetValue(ImagePanelScaleProperty, value);
    }

    #endregion

    /// <summary>
    /// 更新图像
    /// </summary>
    /// <param name="isTile">是否平铺图像</param>
    private void UpdateImage(bool isTile = true)
    {
        if (isTile)
            ImagePanelScale = DefaultImagePanelScale;
    }

    private void UpdateImageInfo()
    {
        if (ImageSource is null) return;

        DefaultImagePanelScale = Math.Min(ActualWidth / ImageSource.Width,
            ActualHeight / ImageSource.Height);

        DefaultImageSize = (ImageSource.Width, ImageSource.Height);
    }
}