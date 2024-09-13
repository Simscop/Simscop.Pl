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
        private double _percent = 0;

        partial void OnPercentChanged(double value)
        {
            if (value == 0) Title = "扫描存图";

            if (Percent > 1)
                Title = $"扫描存图 ({value:F2} %)({Rows}*{Cols})";
        }

        [ObservableProperty]
        private string _title = $"扫描存图";

        /// <summary>
        /// 列数
        /// 对应X-Weight
        /// </summary>
        [ObservableProperty]
        private int _cols = 0;

        /// <summary>
        /// 行数
        /// 对应Y-Height
        /// </summary>
        [ObservableProperty]
        private int _rows = 0;

        [ObservableProperty]
        private int _selectIndex = 0;

        partial void OnSelectIndexChanged(int value)
        {
            switch(value)
            {
                case 0:
                    XStep = 540000;
                    YStep = 420000;
                    break;
                //case 0:
                //    XStep = 170000;
                //    YStep = 100000;
                //    break;
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

            Rows = (int)Math.Ceiling(xSnap / XStep);
            Cols = (int)Math.Ceiling(ySnap / YStep);
            Rows = Rows == 0?1: Rows;
            Cols = Cols == 0?1: Cols;

            Percent = 0;
            int scanCount = Cols * Rows;
            double _per = (1 / (double)(scanCount)) * 100;//进度

            List<Task> saveTasks = new();
            MessageBoxResult result =MessageBox.Show($"{Cols}行（Y）* {Rows}列 (X)", "拼接行列", MessageBoxButton.OKCancel);
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
            double pointStartY = Math.Max(YStart, YEnd);

            int time1 = (int)(Math.Abs(XStart - XEnd) / VmManager.MotorViewModel.XSpeed);
            int time2 = (int)(Math.Abs(YStart - YEnd) / VmManager.MotorViewModel.YSpeed);
            int time0 = (time1 + time2) / 100 + 50;

            int time5 = time0 > 1200 ? time0 : 1200;

            //到达原始位置，z不做移动
            VmManager.MotorViewModel.SetAbsolutionPosition(new[] { true, true, false }, new double[] { pointStartX, pointStartY, 0 });

            Thread.Sleep(time5);

            double xPos = 0;
            double yPos = 0;
            double a = 0;
            double b = 0;
            string file=string.Empty;

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {              
                    if (cancellationToken.IsCancellationRequested)
                    {
                        IsXYStart = "开始扫描";
                        cancellationTokenSource = new CancellationTokenSource();
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
                        b = Cols - j - 1;
                    }

                    //xPos = pointStartX + b * XStep;
                    //yPos = pointStartY + a * YStep;
                    xPos = pointStartX + b * XStep;
                    yPos = pointStartY - a * YStep;

                    VmManager.MotorViewModel.SetAbsolutionPosition(new[] { true, true, false }, new double[] { xPos, yPos, 0 });

                    double expose = VmManager.CameraViewModel.Exposure;
                    double delay = 100;
                    int time = (int)(delay > expose * 3 ? delay : expose * 3);
                    Thread.Sleep(time);

                    Debug.WriteLine($"i-j__{i}-{j} x-y__{a+1}-{b+1} xpos_{xPos} y_pos_{yPos}");

                    var img = VmManager.CameraViewModel.Image?.Clone();
                    string filename = Path.Combine(rootFile, $"{a+1}_{b+1}.bmp");
                    var saveTask = Task.Run(() =>
                    {
                        img?.SaveImage(filename);
                        img?.Dispose();
                        Percent += _per;
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

        [RelayCommand]
        void StartPos()
        {
            VmManager.MotorViewModel.SetAbsolutionPosition(new[] { true, true, false }, new double[] { XStart, YStart, 0 });
        }

        [RelayCommand]
        void EndPos()
        {
            VmManager.MotorViewModel.SetAbsolutionPosition(new[] { true, true, false }, new double[] { XEnd,YEnd , 0 });
        }

    }
}
