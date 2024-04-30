using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Media;
using Fake.Hardware;
using Lift.UI.Controls;
using Simscop.Pl.Core;
using Simscop.Pl.WPF.Views;
using Simscop.Pl.WPF.Views.MessageBox;

namespace Simscop.Pl.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        HardwareManager.Motor = new FakeMortor();
        HardwareManager.Camera = new FakeCamera();
        HardwareManager.Spectrometer = new FakeSpectrometer();
        HardwareManager.OmniDriver = new FakeOmniDriver();
        //DispatcherUnhandledException += App_DispatcherUnhandledException;
        //TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        throw new NotImplementedException();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // todo 添加硬件初始化的功能

        var main = new Splash()
        {
            Background = Brushes.White,

        };
        main.Show();
    }
}
