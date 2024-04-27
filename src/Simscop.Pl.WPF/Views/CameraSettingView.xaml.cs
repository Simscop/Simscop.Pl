using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Simscop.Pl.WPF.Views;

/// <summary>
/// Interaction logic for CameraSettingView.xaml
/// </summary>
public partial class CameraSettingView
{
    public CameraSettingView()
    {
        InitializeComponent();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        DataContext = VmManager.CameraViewModel;
    }
}

