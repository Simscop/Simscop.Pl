using System.Windows.Media;
using Lift.UI.Controls;

namespace Simscop.Pl.WPF.Views;

/// <summary>
/// Interaction logic for CameraSettingView.xaml
/// </summary>
public partial class CameraSettingView : Window
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

