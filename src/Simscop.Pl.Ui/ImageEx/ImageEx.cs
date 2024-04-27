using System.CodeDom;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Accessibility;
using Lift.Core.ImageArray.Helper;

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

    private void OnZoomChanged(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
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
        if (AssociatedObject.Scroll is null) return;

        var pos = e.GetPosition(AssociatedObject);
        var offset = pos - _cursor;
        _cursor = pos;

        AssociatedObject.Scroll.ScrollToHorizontalOffset(AssociatedObject.Scroll.HorizontalOffset - offset.X);
        AssociatedObject.Scroll.ScrollToVerticalOffset(AssociatedObject.Scroll.VerticalOffset - offset.Y);
    }
}


[TemplatePart(Name = NamePartMainPanel, Type = typeof(Panel))]
[TemplatePart(Name = NamePartScrollView, Type = typeof(ScrollViewer))]
[TemplatePart(Name = NamePartViewBox, Type = typeof(Viewbox))]
public class ImageEx : ContentControl
{
    #region Name

    public const string NamePartMainPanel = "PART_MAIN_PANEL";

    public const string NamePartScrollView = "PART_SCROLL";

    public const string NamePartViewBox = "PART_BOX";

    #endregion

    #region Part

    internal Panel? MainPanel;

    internal ScrollViewer? Scroll;

    internal Viewbox? Box;

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
    }


    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        MainPanel = Template.FindName(NamePartMainPanel, this) as Panel;
        Scroll = Template.FindName(NamePartScrollView, this) as ScrollViewer;
        Box = Template.FindName(NamePartViewBox, this) as Viewbox;

        _behaviors.ForEach(b => b.Attach(this));
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
    void UpdateImage(bool isTile = true)
    {
        if (isTile)
            ImagePanelScale = DefaultImagePanelScale;
    }

    void UpdateImageInfo()
    {
        if (ImageSource is null) return;

        DefaultImagePanelScale = Math.Min(ActualWidth / ImageSource.Width,
            ActualHeight / ImageSource.Height);

        DefaultImageSize = (ImageSource.Width, ImageSource.Height);
    }
}