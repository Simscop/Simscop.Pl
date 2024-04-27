using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Simscop.Pl.Core.Models.Charts.Constants;

namespace Simscop.Pl.Core.Models.Charts;

public partial class AnnotationModel : ObservableObject
{
    /// <summary>
    /// 
    /// </summary>
    public event Action? OnValueChanged;

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        OnValueChanged?.Invoke();
    }

    [ObservableProperty]
    private double _fontSize = 14;

    [ObservableProperty]
    private double _fontWeight = 500;

    [ObservableProperty]
    private string _format = "{4}";

    [ObservableProperty]
    private Color _textColor = Color.Red;

    [ObservableProperty]
    private Color _pointColor = Color.Red;

    [ObservableProperty]
    private double _pointSize;

    [ObservableProperty]
    private LineStyle _lineSytle = LineStyle.Automatic;

    [ObservableProperty]
    private double _thickness = 2;

    [ObservableProperty]
    private Color _lineColor = Color.Red;
}