using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Google.Protobuf.WellKnownTypes;
using Lift.UI.Controls;
using Simscop.Pl.Core;
using Simscop.Pl.WPF.Helpers;
using static SkiaSharp.HarfBuzz.SKShaper;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using MessageBox = Lift.UI.Controls.MessageBox;
using Window = Lift.UI.Controls.Window;

namespace Simscop.Pl.WPF;

/// <summary>
/// Interaction logic for Splash.xaml
/// </summary>
public partial class Splash : Window
{
    protected Dispatcher MainDispatcher = Application.Current.Dispatcher;

    public int AnimationDelay { get; set; } = 1000;

    public int DetectDelay { get; set; } = 500;

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
            Thread.Sleep(DetectDelay);
            OnValidMotor();
            Thread.Sleep(DetectDelay);
            OnValidSpectrometer();
            Thread.Sleep(DetectDelay);
            SafeRun(() => Laser.Text = "准备激光");
            Bottom2TopAnimation(Laser);
            Thread.Sleep(DetectDelay);
            SwitchTextAnimation(Laser, "激光方案暂停使用", fore: Brushes.SlateGray);
            Thread.Sleep(1000);

            var duration = 500;
            SafeRun(() =>
            {
                LoadingCircle.BeginAnimation(OpacityProperty, new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(duration)
                });
                Panel.BeginAnimation(OpacityProperty, new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(duration)
                });
            });
            Thread.Sleep(duration);

            SafeRun(() => GridMain.ColumnDefinitions.Clear());
            SafeRun(() => TbResult.Text = "硬件初始化完毕，进入程序中");

            SafeRun(() => TbResult.BeginAnimation(OpacityProperty, new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(duration * 2)
            }));

            Thread.Sleep(duration * 2);

            SafeRun(() => TbResult.BeginAnimation(OpacityProperty, new DoubleAnimation()
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(duration * 2)
            }));
            Thread.Sleep(duration * 2);

            SafeRun(Close);
        });
    }

    // todo 以后这个玩意要自己完成才行 
    void OnValidCamera()
    {
        SafeRun(() => Camera.Text = "准备相机");
        Bottom2TopAnimation(Camera);
        Thread.Sleep(AnimationDelay);

        if (HardwareManager.Camera is null)
        {
            SwitchTextAnimation(Camera, "没有检测到相机实现方案", fore: Brushes.Red);
            Thread.Sleep(AnimationDelay);
            HardwareManager.IsCameraOk = false;
            return;
        }

        SwitchTextAnimation(Camera, "相机环境验证");
        Thread.Sleep(AnimationDelay);

        if (!HardwareManager.Camera.Valid())
        {
            Thread.Sleep(AnimationDelay);
            SwitchTextAnimation(Camera, "相机环境不完整", fore: Brushes.Red);
            Thread.Sleep(AnimationDelay);
            HardwareManager.IsCameraOk = false;
            if (HardwareManager.Camera.LastErrorMessage is { } msg)
                MessageBox.Show(msg, "CameraError");
            return;
        }

        SwitchTextAnimation(Camera, "相机初始化");
        Thread.Sleep(AnimationDelay);

        if (!HardwareManager.Camera.Initialize())
        {
            Thread.Sleep(AnimationDelay);
            SwitchTextAnimation(Camera, "相机初始化失败", fore: Brushes.Red);
            Thread.Sleep(AnimationDelay);
            HardwareManager.IsCameraOk = false;
            return;
        }

        HardwareManager.IsCameraOk = true;
        SwitchTextAnimation(Camera, "相机连接成功", fore: Brushes.Green);
    }

    void OnValidSpectrometer()
    {
        SafeRun(() => Spectrometer.Text = "准备光谱仪");
        Bottom2TopAnimation(Spectrometer);
        Thread.Sleep(AnimationDelay);

        if (HardwareManager.OmniDriver is null)
        {
            SwitchTextAnimation(Spectrometer, "没有检测到光谱仪实现方案", fore: Brushes.Red);
            Thread.Sleep(AnimationDelay);
            HardwareManager.IsSpectrometerOk = false;
            return;
        }

        SwitchTextAnimation(Spectrometer, "检测到Omni方案");
        Thread.Sleep(AnimationDelay);

        SwitchTextAnimation(Spectrometer, "Omni环境验证");
        Thread.Sleep(AnimationDelay);

        if (!HardwareManager.OmniDriver.Valid())
        {
            Thread.Sleep(AnimationDelay);
            SwitchTextAnimation(Camera, "Omni环境不完整", fore: Brushes.Red);
            Thread.Sleep(AnimationDelay);
            HardwareManager.IsSpectrometerOk = false;
            return;
        }

        SwitchTextAnimation(Spectrometer, "Omni初始化");
        Thread.Sleep(AnimationDelay);

        if (!HardwareManager.OmniDriver.Initialize())
        {
            Thread.Sleep(AnimationDelay);
            SwitchTextAnimation(Spectrometer, "Omni初始化失败", fore: Brushes.Red);
            Thread.Sleep(AnimationDelay);
            HardwareManager.IsSpectrometerOk = false;
            return;
        }

        SwitchTextAnimation(Spectrometer, "查询光谱仪");
        Thread.Sleep(AnimationDelay);

        var spes = HardwareManager.OmniDriver.GetAllSpectrometer();

        if (spes.Length == 0)
        {
            Thread.Sleep(AnimationDelay);
            SwitchTextAnimation(Spectrometer, "没有检测到光谱仪设备，请检查", fore: Brushes.Red);
            Thread.Sleep(AnimationDelay);
            HardwareManager.IsSpectrometerOk = false;
            return;
        }

        HardwareManager.IsSpectrometerOk = true;
        HardwareManager.Spectrometer = spes[0];

        SwitchTextAnimation(Spectrometer, $"检查到{spes.Length}个设备", fore: Brushes.Green);

        Thread.Sleep(AnimationDelay);

    }

    void OnValidMotor()
    {
        SafeRun(() => Motor.Text = "准备电动台");
        Bottom2TopAnimation(Motor);
        Thread.Sleep(AnimationDelay);

        if (HardwareManager.Motor is null)
        {
            SwitchTextAnimation(Motor, "没有检测到电动台实现方案", fore: Brushes.Red);
            Thread.Sleep(AnimationDelay);
            HardwareManager.IsMotorOk = false;
            return;
        }

        SwitchTextAnimation(Motor, "电动台环境验证");
        Thread.Sleep(AnimationDelay);

        if (!HardwareManager.Motor.Valid())
        {
            Thread.Sleep(AnimationDelay);
            SwitchTextAnimation(Motor, "电动台环境不完整", fore: Brushes.Red);
            Thread.Sleep(AnimationDelay);
            HardwareManager.IsMotorOk = false;
            return;
        }

        SwitchTextAnimation(Motor, "电动台归位自检中");
        Thread.Sleep(AnimationDelay);

        if (!HardwareManager.Motor.Initialize())
        {
            Thread.Sleep(AnimationDelay);
            SwitchTextAnimation(Motor, "电动台自检失败", fore: Brushes.Red);
            Thread.Sleep(AnimationDelay);
            HardwareManager.IsMotorOk = false;
            return;
        }



        HardwareManager.IsMotorOk = true;
        SwitchTextAnimation(Motor, "检测到3个电动轴", fore: Brushes.Green);
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

    void SwitchTextAnimation(TextBlock obj, string text, uint duration = 300, Brush? fore = null)
    {
        Task.Run(() =>
        {
            SafeRun(() => obj.BeginAnimation(OpacityProperty, new DoubleAnimation()
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(duration)
            }));

            Thread.Sleep((int)duration);

            SafeRun(() =>
            {
                obj.Text = text;
                obj.BeginAnimation(OpacityProperty, new DoubleAnimation()
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(duration)
                });
            });
            if (fore is not null) SafeRun(() => obj.Foreground = fore);
        });
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (HardwareManager.IsCameraOk)
        {
            var main = new MainWindow()
            {
                Background = Brushes.White
            };
            main.Show();
        }

        base.OnClosing(e);
    }
}
