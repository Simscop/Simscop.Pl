﻿using System.Windows.Media;

namespace Simscop.Pl.WPF.Views;

/// <summary>
/// Interaction logic for CameraSettingView.xaml
/// </summary>
public partial class CameraSettingView
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

