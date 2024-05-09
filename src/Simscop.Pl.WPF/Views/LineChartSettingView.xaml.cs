using System.Windows.Media;
using Lift.UI.Controls;

namespace Simscop.Pl.WPF.Views;

/// <summary>
/// Interaction logic for LineChartSettingView.xaml
/// </summary>
public partial class LineChartSettingView : Window
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
