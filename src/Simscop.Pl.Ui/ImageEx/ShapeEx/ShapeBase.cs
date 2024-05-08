using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Lift.UI.Tools.Extension;
using InkCanvas = System.Windows.Controls.InkCanvas;

namespace Simscop.Pl.Ui.ImageEx.ShapeEx;

// todo 线宽改成显示像素，而不是实际宽度

public abstract class ShapeBase : Shape
{
    /// <summary>
    /// 任意图像的起始点
    /// </summary>
    public Point PointStart { get; set; }

    /// <summary>
    /// 任意图形的终止点
    /// </summary>
    public Point PointEnd { get; set; }

    /// <summary>
    /// 绘制图像
    /// </summary>
    public abstract void Draw(InkCanvas canvas);

    /// <summary>
    /// 清除画板
    /// </summary>
    public virtual void Clear(InkCanvas canvas) => canvas.Children.Remove(this);

    /// <summary>
    /// 是否被选中
    /// </summary>
    public bool IsSelected { get; protected set; }

    /// <summary>
    /// 切换选中状态
    /// </summary>
    public virtual void SetSelected(InkCanvas? canvas)
    {
        if (canvas is null) return;

        foreach (var obj in canvas.Children)
        {
            if (obj is not ShapeBase shape) continue;

            shape.IsSelected = !shape.IsSelected;
            RefreshStrokeThickness();

            OnSelectedChanged?.Invoke(this);
        }
    }

    /// <summary>
    /// 切换选中状态
    /// </summary>
    public virtual void SetSelected() => SetSelected(Parent as InkCanvas);

    /// <summary>
    /// 当选中后
    /// </summary>
    public event Action<ShapeBase>? OnSelectedChanged;

    /// <summary>
    /// 常规状态宽度
    /// </summary>
    public double ThicknessNormal { get; set; } = 0;

    /// <summary>
    /// 鼠标放上去后线宽
    /// </summary>
    public double ThicknessMouseOver { get; set; } = 0;

    /// <summary>
    /// 选择后线宽
    /// </summary>
    public double ThicknessSelected { get; set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    public double ThicknessMouseOverAndSelected { get; set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    protected ShapeBase() : base() => InitComponent();

    /// <summary>
    /// initialize the components
    /// </summary>
    public void InitComponent()
    {
        ThicknessNormal = StrokeThickness;

        MouseEnter += (_, _) => RefreshStrokeThickness();
        MouseLeave += (_, _) => RefreshStrokeThickness();
        MouseDown += (_, e) =>
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                SetSelected();
        };
    }

    /// <summary>
    /// 刷新图案
    /// </summary>
    public virtual void Refresh()
    {
        var start = PointStart;
        var end = PointEnd;
        Width = Math.Abs(start.X - end.X);
        Height = Math.Abs(start.Y - end.Y);

        var position = new Point(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));
        InkCanvas.SetLeft(this, position.X);
        InkCanvas.SetTop(this, position.Y);
    }

    protected virtual void RefreshStrokeThickness()
    {
        Fill = IsSelected
            ? new SolidColorBrush() { Color = ((SolidColorBrush)Fill).Color, Opacity = 0.5 }
            : new SolidColorBrush() { Color = ((SolidColorBrush)Fill).Color, Opacity = 0.2 };

        Stroke = IsSelected
            ? new SolidColorBrush() { Color = ((SolidColorBrush)Stroke).Color, Opacity = 0.5 }
            : new SolidColorBrush() { Color = ((SolidColorBrush)Stroke).Color, Opacity = 0.2 };

        StrokeThickness = IsSelected
            ? IsMouseOver ? ThicknessMouseOverAndSelected : ThicknessSelected
            : IsMouseOver ? ThicknessMouseOver : ThicknessNormal;

    }

    internal abstract ShapeBase Clone();

    protected override Geometry DefiningGeometry
        => throw new NotImplementedException();

}

class TestShape : ShapeBase
{
    public override void Draw(InkCanvas canvas)
    {
        IsSelected = false;
        throw new NotImplementedException();
    }

    internal override ShapeBase Clone()
    {
        throw new NotImplementedException();
    }
}

public class RectangleShape : ShapeBase
{
    protected override Geometry DefiningGeometry =>
        new RectangleGeometry
        {
            Rect = new Rect(new Point(0, 0), new Size(Width, Height))
        };

    public override void Draw(InkCanvas canvas)
    {
        Refresh();
        canvas.Children.Add(this);
    }

    internal override RectangleShape Clone()
    {
        var clone = new RectangleShape();

        typeof(RectangleShape).GetProperties()
            ?.Where(prop => prop.CanWrite)
            ?.Where(prop => new List<string>()
            {
                "Width","Height","PointStart","PointEnd","Fill","Strokex"
            }.Contains(prop.Name))
            ?.Do(prop => prop.SetValue(clone, prop.GetValue(this)));

        return clone;
    }
}