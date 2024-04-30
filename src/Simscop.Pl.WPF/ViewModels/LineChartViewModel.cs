﻿using System.ComponentModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Simscop.Pl.Core.Models.Charts;
using Simscop.Pl.Core.Models.Charts.Constants;
using System.Drawing;

namespace Simscop.Pl.WPF.ViewModels;

public partial class LineChartViewModel : ObservableObject
{
    /// <summary>
    /// 
    /// </summary>
    public event Action<string?>? OnValueChanged;

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        OnValueChanged?.Invoke(e.PropertyName);
    }

    [ObservableProperty]
    private AxisModel _axisX = new();

    [ObservableProperty]
    private AxisModel _axisY = new();

    [ObservableProperty]
    private PanelModel _panel = new();

    [ObservableProperty]
    private AnnotationModel _annotation = new();

    [ObservableProperty]
    private List<(double X, double Y)>? _data;

    [ObservableProperty]
    private Color _color = Color.Black;

    [ObservableProperty]
    private LineStyle _lineStyle = LineStyle.Automatic;

    [ObservableProperty]
    private string _format = "{0}\n{1}: {2:##.##}\n{3}: {4:##.##}";

    [ObservableProperty]
    private double _margin = 8;

    [ObservableProperty]
    private double _fontSize = 12;

    [ObservableProperty]
    private double _fontWeight = 800;

    [ObservableProperty]
    private object? _serial;

    [ObservableProperty]
    private double _selected = double.NaN;

    partial void OnSelectedChanged(double value)
    {
        // todo 这里发送消息过去
    }
}