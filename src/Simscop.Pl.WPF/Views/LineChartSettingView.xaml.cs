using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Simscop.Pl.Core;
using Simscop.Pl.WPF.UserControls;

namespace Simscop.Pl.WPF.Views;

/// <summary>
/// Interaction logic for LineChartSettingView.xaml
/// </summary>
public partial class LineChartSettingView
{
    public LineChartSettingView()
    {
        InitializeComponent();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        PanelView.DataContext = VmManager.LineChartViewModel.Panel;
        AxisXView.DataContext = VmManager.LineChartViewModel.AxisX;
        AxisYView.DataContext = VmManager.LineChartViewModel.AxisY;
        AnnotationView.DataContext = VmManager.LineChartViewModel.Annotation;
    }
}
