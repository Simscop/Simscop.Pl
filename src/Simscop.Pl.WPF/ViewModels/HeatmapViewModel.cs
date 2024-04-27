using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simscop.Pl.WPF.ViewModels;

public partial class HeatmapViewModel : ObservableObject
{
    [ObservableProperty]
    private (int Row, int Column) _selected = new(-1, -1);

    partial void OnSelectedChanged((int Row, int Column) value)
    {
        // todo 发送消息修改曲线
    }
}