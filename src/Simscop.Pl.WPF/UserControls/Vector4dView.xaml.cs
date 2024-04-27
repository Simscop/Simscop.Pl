using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lift.UI.Data;
using Simscop.Pl.Core;

namespace Simscop.Pl.WPF.UserControls;

/// <summary>
/// Interaction logic for Vector4dView.xaml
/// </summary>
public partial class Vector4dView
{
    public Vector4dView()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty VectorProperty = DependencyProperty.Register(
        nameof(Vector), typeof(Vector4d), typeof(Vector4dView), new PropertyMetadata(default(Vector4d), OnVectorChanged));

    private static void OnVectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Vector4dView view) return;

        view._flag = true;

        view.V1.Value = view.Vector.V1;
        view.V2.Value = view.Vector.V2;
        view.V3.Value = view.Vector.V3;
        view.V4.Value = view.Vector.V4;

        view._flag = false;
    }

    /// <summary>
    /// 为真时，value的改变不更新到Vector
    /// </summary>
    private bool _flag = true;

    private void ValueChanged(object? sender, FunctionEventArgs<double> e)
    {
        if (_flag) return;
        Vector = new Vector4d(V1.Value, V2.Value, V3.Value, V4.Value);
    }

    public Vector4d Vector
    {
        get => (Vector4d)GetValue(VectorProperty);
        set => SetValue(VectorProperty, value);
    }

    public static readonly DependencyProperty MinProperty = DependencyProperty.Register(
        nameof(Min), typeof(double), typeof(Vector4dView), new PropertyMetadata((double)-1, OnMinChanged));

    private static void OnMinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Vector4dView view || e.NewValue is not double v) return;

        view.V1.Minimum = v;
        view.V2.Minimum = v;
        view.V3.Minimum = v;
        view.V4.Minimum = v;
    }

    public double Min
    {
        get => (double)GetValue(MinProperty);
        set => SetValue(MinProperty, value);
    }

    public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(
        nameof(Max), typeof(double), typeof(Vector4dView), new PropertyMetadata(double.MaxValue, OnMaxChanged));

    private static void OnMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Vector4dView view || e.NewValue is not double v) return;

        view.V1.Maximum = v;
        view.V2.Maximum = v;
        view.V3.Maximum = v;
        view.V4.Maximum = v;
    }

    public double Max
    {
        get => (double)GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }

    public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register(
        nameof(Increment), typeof(double), typeof(Vector4dView), new PropertyMetadata((double)1, OnIncrementChanged));

    private static void OnIncrementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Vector4dView view || e.NewValue is not double v) return;

        view.V1.Increment = v;
        view.V2.Increment = v;
        view.V3.Increment = v;
        view.V4.Increment = v;
    }

    public double Increment
    {
        get => (double)GetValue(IncrementProperty);
        set => SetValue(IncrementProperty, value);
    }
}
