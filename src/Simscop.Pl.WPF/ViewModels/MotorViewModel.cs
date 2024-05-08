using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Simscop.Pl.Core;
using Simscop.Pl.Core.Services;

namespace Simscop.Pl.WPF.ViewModels;

public enum MotorAxis
{
    X,
    Y,
    Z
}

public partial class MotorViewModel : ObservableObject
{
    internal IMotorService Motor;

    private readonly DispatcherTimer _motorTimer = new()
    {
        Interval = TimeSpan.FromSeconds(1)
    };

    public MotorViewModel()
    {
        if (HardwareManager.Motor is null) throw new Exception();
        Motor = HardwareManager.Motor;
    }

    [ObservableProperty]
    private double _x;

    [ObservableProperty]
    private double _y;

    [ObservableProperty]
    private double _z;

    [ObservableProperty]
    private double _intervalX;

    [ObservableProperty]
    private double _intervalY;

    [ObservableProperty]
    private double _intervalZ;

    [ObservableProperty]
    private MotorAxis _axis = MotorAxis.X;

    [RelayCommand]
    private async Task AsyncMoveX(bool positive = true) =>
        await Motor.AsyncSetRelativePosition(new[] { true, false, false },
            new[] { positive ? IntervalX : -IntervalX, positive ? IntervalY : -IntervalY, positive ? IntervalZ : -IntervalZ });

    [RelayCommand]
    private async Task AsyncMoveY(bool positive = true) =>
        await Motor.AsyncSetRelativePosition(new[] { false, true, false }, 
            new[] { positive ? IntervalX : -IntervalX, positive ? IntervalY : -IntervalY, positive ? IntervalZ : -IntervalZ });

    [RelayCommand]
    private async Task AsyncMoveZ(bool positive = true) =>
        await Motor.AsyncSetRelativePosition(new[] { false, false, true }, 
            new[] { positive ? IntervalX : -IntervalX, positive ? IntervalY : -IntervalY, positive ? IntervalZ : -IntervalZ });

    public void StartTimer()
    {
        if (!HardwareManager.IsMotorOk) return;

        _motorTimer.Tick += (s, e) =>
        {
            X = Motor?.X ?? 0;
            Y = Motor?.Y ?? 0;
            Z = Motor?.Z ?? 0;
        };
        _motorTimer.Start();
    }
}