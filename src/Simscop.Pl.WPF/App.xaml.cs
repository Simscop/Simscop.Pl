using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Media;

namespace Simscop.Pl.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var window = new MainWindow()
        {
            Background = Brushes.White,
        };
        window.Show();
    }
}
