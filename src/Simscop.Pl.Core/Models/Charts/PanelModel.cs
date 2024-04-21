using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Simscop.Pl.Core.Models.Charts;

public partial class PanelModel : ObservableObject
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
    private Vector4d _margin = new Vector4d(-1);

    [ObservableProperty]
    private Vector4d _padding = new Vector4d(-1);

    [ObservableProperty]
    private Vector4d _axisMarginScale = new Vector4d(0);

    [ObservableProperty]
    private GridStyle _gridStyle = GridStyle.None;

    [ObservableProperty]
    private string? _title;

    [ObservableProperty]
    private string? _subtitle;

    [ObservableProperty]
    private double _titlePadding = 6;
}