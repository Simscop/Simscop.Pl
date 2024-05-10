using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Simscop.Pl.Core.Models.Charts;
using Simscop.Pl.Core.Models.Charts.Constants;
using System.Drawing;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Simscop.Pl.Core;
using Simscop.Pl.Core.Services;
using Simscop.Pl.WPF.Managers;
using Simscop.Pl.Core.Models;

namespace Simscop.Pl.WPF.ViewModels;

public partial class LineChartViewModel : ObservableObject
{
    protected ISpectrometerService Spectrometer = HardwareManager.Spectrometer!;

    /// <summary>
    /// 
    /// </summary>
    public event Action<string?>? OnValueChanged;

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        OnValueChanged?.Invoke(e.PropertyName);
    }

    public LineChartViewModel()
    {
        WeakReferenceMessenger.Default.Register<LineChangedMessage>(this, (obj, msg) =>
        {
            Application.Current?.Dispatcher.BeginInvoke(() => { Data = msg.Points; });
        });
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
        WeakReferenceMessenger.Default.Send(new SelectedWaveChangedMessage(value));
    }
}