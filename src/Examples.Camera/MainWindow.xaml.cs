﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenCvSharp.WpfExtensions;
using Simscop.Pl.Hardware.Camera;

namespace Examples.Camera;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        var camera = new ToupTek();

        camera.Valid();
        camera.Initialize();

        camera.OnCaptureChanged += mat =>
        {
            Viewer.ImageSource = BitmapFrame.Create(mat.ToBitmapSource());
        };
    }
}
