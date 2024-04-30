using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Lift.UI.Controls;
using Simscop.Pl.Core;
using Simscop.Pl.WPF.Helpers;

namespace Simscop.Pl.WPF;

/// <summary>
/// Interaction logic for Splash.xaml
/// </summary>
public partial class Splash
{
    protected Dispatcher MainDispatcher = Application.Current.Dispatcher;

    private bool _init = true;

    public Splash()
    {
        InitializeComponent();

    }


    protected override void OnRender(DrawingContext drawingContext)
    {
        if (!_init) return;
        _init = false;

        base.OnRender(drawingContext);
        Background = Brushes.White;

        Task.Run(() =>
        {

            OnValidCamera();
            Thread.Sleep(500);
            OnValidMotor();
            Thread.Sleep(500);
            OnValidSpectrometer();
            Thread.Sleep(500);
            SafeRun(() => Laser.Text = "准备激光");
            Bottom2TopAnimation(Laser);
            Thread.Sleep(1000);

            SwitchTextAnimation(Laser, "激光方案暂停使用");
            SafeRun(() => Laser.Foreground = Brushes.SlateGray);
        });
    }

    // todo 以后这个玩意要自己完成才行 
    void OnValidCamera()
    {
        SafeRun(() => Camera.Text = "准备相机");
        Bottom2TopAnimation(Camera);
        Thread.Sleep(1000);

        if (HardwareManager.Camera is null)
        {
            SafeRun(() => { Camera.Foreground = Brushes.Red; });
            SwitchTextAnimation(Camera, "没有检测到相机实现方案");
            Thread.Sleep(1000);
            HardwareManager.IsCameraOk = false;
            return;
        }

        SwitchTextAnimation(Camera, "相机环境验证");
        Thread.Sleep(1000);

        if (!HardwareManager.Camera.Valid())
        {
            Thread.Sleep(1000);
            SafeRun(() => { Camera.Foreground = Brushes.Red; });
            SwitchTextAnimation(Camera, "相机环境不完整");
            Thread.Sleep(1000);
            HardwareManager.IsCameraOk = false;
            return;
        }

        SwitchTextAnimation(Camera, "相机初始化");
        Thread.Sleep(1000);

        if (!HardwareManager.Camera.Initialize())
        {
            Thread.Sleep(1000);
            SafeRun(() => { Camera.Foreground = Brushes.Red; });
            SwitchTextAnimation(Camera, "相机初始化失败");
            Thread.Sleep(1000);
            HardwareManager.IsCameraOk = false;
            return;
        }

        HardwareManager.IsCameraOk = true;
        SwitchTextAnimation(Camera, "相机连接成功");
        SafeRun(() => { Camera.Foreground = Brushes.Green; });
    }

    void OnValidSpectrometer()
    {
        SafeRun(() => Spectrometer.Text = "准备光谱仪");
        Bottom2TopAnimation(Spectrometer);
        Thread.Sleep(1000);

        if (HardwareManager.OmniDriver is null)
        {
            SafeRun(() => { Spectrometer.Foreground = Brushes.Red; });
            SwitchTextAnimation(Spectrometer, "没有检测到光谱仪实现方案");
            Thread.Sleep(1000);
            HardwareManager.IsSpectrometerOk = false;
            return;
        }

        SwitchTextAnimation(Spectrometer, "检测到Omni方案");
        Thread.Sleep(1000);

        SwitchTextAnimation(Spectrometer, "Omni环境验证");
        Thread.Sleep(1000);

        if (!HardwareManager.OmniDriver.Valid())
        {
            Thread.Sleep(1000);
            SafeRun(() => { Spectrometer.Foreground = Brushes.Red; });
            SwitchTextAnimation(Camera, "Omni环境不完整");
            Thread.Sleep(1000);
            HardwareManager.IsSpectrometerOk = false;
            return;
        }

        SwitchTextAnimation(Spectrometer, "Omni初始化");
        Thread.Sleep(1000);

        if (!HardwareManager.OmniDriver.Initialize())
        {
            Thread.Sleep(1000);
            SafeRun(() => { Spectrometer.Foreground = Brushes.Red; });
            SwitchTextAnimation(Spectrometer, "Omni初始化失败");
            Thread.Sleep(1000);
            HardwareManager.IsSpectrometerOk = false;
            return;
        }

        SwitchTextAnimation(Spectrometer, "查询光谱仪");
        Thread.Sleep(1000);

        var spes = HardwareManager.OmniDriver.GetAllSpectrometer();

        if (spes.Length == 0)
        {
            Thread.Sleep(1000);
            SafeRun(() => { Spectrometer.Foreground = Brushes.Red; });
            SwitchTextAnimation(Spectrometer, "没有检测到光谱仪设备，请检查");
            Thread.Sleep(1000);
            HardwareManager.IsSpectrometerOk = false;
            return;
        }

        HardwareManager.IsSpectrometerOk = true;
        SwitchTextAnimation(Spectrometer, $"检查到{spes.Length}个设备");
        SafeRun(() => { Spectrometer.Foreground = Brushes.Green; });
    }

    void OnValidMotor()
    {
        SafeRun(() => Motor.Text = "准备电动台");
        Bottom2TopAnimation(Motor);
        Thread.Sleep(1000);

        if (HardwareManager.Motor is null)
        {
            SafeRun(() => { Motor.Foreground = Brushes.Red; });
            SwitchTextAnimation(Motor, "没有检测到电动台实现方案");
            Thread.Sleep(1000);
            HardwareManager.IsMotorOk = false;
            return;
        }

        SwitchTextAnimation(Motor, "电动台环境验证");
        Thread.Sleep(1000);

        if (!HardwareManager.Motor.Valid())
        {
            Thread.Sleep(1000);
            SafeRun(() => { Motor.Foreground = Brushes.Red; });
            SwitchTextAnimation(Motor, "电动台环境不完整");
            Thread.Sleep(1000);
            HardwareManager.IsMotorOk = false;
            return;
        }

        SwitchTextAnimation(Motor, "电动台初始化");
        Thread.Sleep(1000);

        if (!HardwareManager.Motor.Initialize())
        {
            Thread.Sleep(1000);
            SafeRun(() => { Motor.Foreground = Brushes.Red; });
            SwitchTextAnimation(Motor, "电动台初始化失败");
            Thread.Sleep(1000);
            HardwareManager.IsMotorOk = false;
            return;
        }



        HardwareManager.IsMotorOk = true;
        SwitchTextAnimation(Motor, "检测到3个电动轴");
        SafeRun(() => { Motor.Foreground = Brushes.Green; });
    }

    void SafeRun(Action action) => MainDispatcher.BeginInvoke(action);

    void Bottom2TopAnimation(TextBlock obj)
    {
        SafeRun(() =>
        {
            var opacityAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.3)
            };

            var translateYAnimation = new DoubleAnimation
            {
                From = 50,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.3)
            };

            var storyboard = new Storyboard();
            storyboard.Children.Add(opacityAnimation);
            storyboard.Children.Add(translateYAnimation);

            Storyboard.SetTarget(opacityAnimation, obj);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));

            Storyboard.SetTarget(translateYAnimation, obj);
            Storyboard.SetTargetProperty(translateYAnimation, new PropertyPath("(TextBlock.RenderTransform).(TranslateTransform.Y)"));


            obj.Visibility = Visibility.Visible;
            storyboard.Begin();
        });
    }

    void SwitchTextAnimation(TextBlock obj, string text, uint duration = 300)
    {
        SafeRun(() =>
        {
            var opacityAnimation1 = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(duration)
            };

            var opacityAnimation2 = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(duration)
            };

            var storyboard = new Storyboard();
            storyboard.Children.Add(opacityAnimation1);

            Storyboard.SetTarget(opacityAnimation1, obj);
            Storyboard.SetTargetProperty(opacityAnimation1, new PropertyPath(OpacityProperty));

            SafeRun(() => obj.Text = text);
            storyboard.Begin();

            storyboard.Children.Clear();
            storyboard.Children.Add(opacityAnimation2);
            Storyboard.SetTarget(opacityAnimation2, obj);
            Storyboard.SetTargetProperty(opacityAnimation2, new PropertyPath(OpacityProperty));
            storyboard.Begin();
        });
    }
}
