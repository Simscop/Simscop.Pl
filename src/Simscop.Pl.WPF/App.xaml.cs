using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Media;
using Simscop.Pl.WPF.Views;

namespace Simscop.Pl.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // todo 添加硬件初始化的功能

        var main = new DemoWindow()
        {
            Background = Brushes.White,

        };
        main.Show();
    }
}
