using CommunityToolkit.Mvvm.ComponentModel;
using System.Drawing;
using CommunityToolkit.Mvvm.Messaging;
using Simscop.Pl.WPF.Managers;

namespace Simscop.Pl.WPF.ViewModels;

public partial class HeatmapViewModel : ObservableObject
{
    [ObservableProperty]
    private int _cols = 0;

    [ObservableProperty]
    private int _rows = 0;

    [ObservableProperty]
    private Point _selected = new(-1, -1);

    [ObservableProperty]
    private double[,]? _array;

    public HeatmapViewModel()
    {
        WeakReferenceMessenger.Default.Register<Line2MapMessage>(this, (obj, msg) =>
        {
            Cols = msg.Columns;
            Rows = msg.Rows;

            Array = msg.Array;
        });
    }

    partial void OnSelectedChanged(Point value)
        => WeakReferenceMessenger.Default.Send(new Map2LineMessage(value.Y, value.X));
}