using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Media;
using Fake.Hardware;
using Lift.UI.Controls;
using Simscop.Pl.Core;
using Simscop.Pl.Hardware;
using Simscop.Pl.Hardware.Camera;
using Simscop.Pl.WPF.Views;
using Simscop.Pl.WPF.Views.MessageBox;
using Application = System.Windows.Application;

namespace Simscop.Pl.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {

        //DispatcherUnhandledException += App_DispatcherUnhandledException;
        //TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    void Fake()
    {
        HardwareManager.Motor = new FakeMortor();
        HardwareManager.Camera = new FakeCamera()
        {
            //SafeThreading = Application.Current.Dispatcher,
        };
        HardwareManager.Spectrometer = new FakeSpectrometer();
        HardwareManager.OmniDriver = new FakeOmniDriver();

        HardwareManager.IsCameraOk = true;
        HardwareManager.IsMotorOk = true;

    }

    void Initialize()
    {
        HardwareManager.Motor = new Hardware.Zaber();
        HardwareManager.Camera = new ToupTek();
        HardwareManager.OmniDriver = new OmniManager();
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

        var isFake = true;

        if (isFake)
        {
            Fake();
            var main = new MainWindow()
            {
                Background = Brushes.White,

            };
            main.Show();
        }
        else
        {
            Initialize();
            var main = new Splash();
            main.Show();
        }
    }
}
