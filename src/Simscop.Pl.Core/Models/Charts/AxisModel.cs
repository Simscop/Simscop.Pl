using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Simscop.Pl.Core.Models.Charts;

public partial class AxisModel : ObservableObject
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
    private string? _title;

    [ObservableProperty]
    private string? _unit;

    [ObservableProperty]
    private bool _isVisible = true;

    [ObservableProperty]
    private bool _isZoom = true;

    [ObservableProperty]
    private bool _isPanning = true;

    [ObservableProperty]
    private TickStyle _tickStyle = TickStyle.None;

    [ObservableProperty]
    private double _viewMinimum = double.NaN;

    [ObservableProperty]
    private double _viewMaximum = double.NaN;
}

