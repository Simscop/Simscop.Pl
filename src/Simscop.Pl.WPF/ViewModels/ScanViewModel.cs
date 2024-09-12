using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lift.UI.Controls;
using Simscop.Pl.Core;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
using MessageBox = System.Windows.MessageBox;

namespace Simscop.Pl.WPF.ViewModels
{
    public partial class ScanViewModel : ObservableObject
    {
        public ScanViewModel()
        {
            if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            SelectIndex = 1;
        }

        [ObservableProperty]
        private int _selectIndex = 0;

        partial void OnSelectIndexChanged(int value)
        {
            switch(value)
            {

                case 0:
                    XStep = 170000;
                    YStep = 100000;
                    break;
                case 1:
                    XStep = 140000;
                    YStep = 105000;
                    break;
                case 2:
                    XStep = 95000;
                    YStep = 65000;
                    break;
            }
        }

        [ObservableProperty]
        public List<string> _selectLite = new List<string>() { "10x", "40x", "60x" };

        [ObservableProperty]
        private double _xStart = 0;

        [ObservableProperty]
        private double _xEnd = 0;

        [ObservableProperty]
        private double _yStart = 0;

        [ObservableProperty]
        private double _yEnd = 0;

        [ObservableProperty]
        private double _xStep = 0;

        [ObservableProperty]
        private double _yStep = 0;

        [ObservableProperty]
        private String _isXYStart = "开始扫描";

        [RelayCommand]
        void SetXYStartPoint()
        {
            XStart = HardwareManager.Motor!.X;
            YStart = HardwareManager.Motor!.Y;
        }

        [RelayCommand]
        void SetXYEndPoint()
        {
            XEnd = HardwareManager.Motor!.X;
            YEnd = HardwareManager.Motor!.Y;
        }

        private CancellationTokenSource cancellationTokenSource;

        [RelayCommand]
        void StartScan()
        {
            if (IsXYStart == "开始扫描")
            {
                Task.Run(() =>
                {
                    Start(cancellationTokenSource.Token);
                });
            }
            else
            {
                cancellationTokenSource.Cancel();
            }
        }

        void Start(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(Root))
            {
                MessageBox.Show("请先设置存储路径");
                return;
            }
            if (!string.IsNullOrEmpty(Root) && !Directory.Exists(Root)) Directory.CreateDirectory(Root);

            //string rootFile = System.IO.Path.Combine(Root, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff"));
            //if (!string.IsNullOrEmpty(rootFile) && !Directory.Exists(rootFile)) Directory.CreateDirectory(rootFile);
            string rootFile=Root;

            if (XStart == YStart)
            {
                MessageBox.Show("请先设置起始点");
            }

            if (XEnd == YEnd)
            {
                MessageBox.Show("请先设置终止点");
            }

            if (XStep == 0 || YStep == 0)
            {
               MessageBox.Show("请正确设置步长");
                return;
            }

            double xSnap = Math.Abs(XStart - XEnd);
            double ySnap = Math.Abs(YStart - YEnd);

            int rows = (int)Math.Ceiling(xSnap / XStep);
            int cols = (int)Math.Ceiling(ySnap / YStep);
            rows=rows==0?1:rows;
            cols=cols==0?1:cols;

            List<Task> saveTasks = new();

            MessageBoxResult result =MessageBox.Show($"{rows}行*{cols}列", "拼接行列", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                IsXYStart = "停止扫描";
            }
            else
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource = new CancellationTokenSource();
                Debug.WriteLine("扫描取消");
                IsXYStart = "开始扫描";
                return;
            }

            //拼接图实际位置左下角为起始点
            double pointStartX = Math.Min(XStart, XEnd);
            double pointStartY= Math.Max(YStart, YEnd);

            int time1 = (int)(Math.Abs(XStart - XEnd) / VmManager.MotorViewModel.XSpeed / 1000);
            int time2 = (int)(Math.Abs(YStart - YEnd) / VmManager.MotorViewModel.YSpeed / 1000);

            //到达原始位置，z不做移动
            VmManager.MotorViewModel.SetAbsolutionPosition(new[] { true, true, false }, new double[] { pointStartX, pointStartY, 0 });

            double xPos = 0;
            double yPos = 0;
            double a = 0;
            double b = 0;
            string file=string.Empty;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        IsXYStart = "开始扫描";
                        Debug.WriteLine("Stitcher return");
                        return;
                    }

                    if (i % 2 == 0)
                    {
                        a = i;
                        b = j;
                    }
                    else if (i % 2 == 1)
                    {
                        a = i;
                        b = cols - j - 1;
                    }

                    xPos = pointStartX + b * XStep;
                    yPos = pointStartY + a * YStep;
                    VmManager.MotorViewModel.SetAbsolutionPosition(new[] { true, true, false }, new double[] { xPos, yPos, 0 });

                    double expose = VmManager.CameraViewModel.Exposure;
                    Thread.Sleep((int)(expose * 2));

                    Debug.WriteLine($"i-j__{i}-{j} x-y__{a+1}-{b+1} xpos_{xPos} y_pos_{yPos}");

                    var img = VmManager.CameraViewModel.Image?.Clone();
                    string filename = Path.Combine(rootFile, $"{a+1}_{b+1}.bmp");
                    var saveTask = Task.Run(() =>
                    {
                        img?.SaveImage(filename);
                        img?.Dispose();
                    });
                    saveTasks.Add(saveTask);

                }
            }

            Task.WhenAll(saveTasks);
            saveTasks.Clear();

            IsXYStart = "开始扫描";

            MessageBox.Show("COMPLETE!!!");
        }

        [ObservableProperty]
        private string _root = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ScanResult");

        [RelayCommand]
        void SelectRoot()
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
                Root = dialog.SelectedPath;
        }

        [RelayCommand]
        void OpenFolder() => OpenFolderAndSelectFile(Root);


        private void OpenFolderAndSelectFile(string fileFullName)
        {
            ProcessStartInfo psi = new ProcessStartInfo("Explorer.exe");
            psi.Arguments = "/e,/select," + fileFullName;
            Process.Start(psi);
        }

    }
}
