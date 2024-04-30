using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Simscop.Pl.WPF.Views.MessageBox;

/// <summary>
/// Interaction logic for AcquireBox.xaml
/// </summary>
public partial class AcquireBox
{
    public AcquireBox()
    {
        InitializeComponent();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        Background = Brushes.White;
    }

    private static AcquireBox? _box;

    public static void ShowAsSingleton()
    {
        _box ??= new AcquireBox()
        {
            DataContext = VmManager.MainViewModel
        };

        var main = Application.Current.MainWindow ?? _box;

        // 获取主窗口的位置和尺寸
        var mainWindowLeft = main.Left;
        var mainWindowTop = main.Top;
        var mainWindowWidth = main.Width;
        var mainWindowHeight = main.Height;

        // 计算新窗口的位置
        var newWindowLeft = mainWindowLeft + (mainWindowWidth - _box.Width) / 2;
        var newWindowTop = mainWindowTop + (mainWindowHeight - _box.Height) / 2;

        // 设置新窗口的位置
        _box.Left = newWindowLeft;
        _box.Top = newWindowTop;

        _box.ShowDialog();

    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (this != _box)
        {
            base.OnClosing(e);
            return;
        }

        e.Cancel = true;
        _box.Hide();
    }
}
