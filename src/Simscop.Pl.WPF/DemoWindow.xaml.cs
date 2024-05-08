using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Simscop.Pl.Hardware.Camera;
using Simscop.Pl.Ui.ImageEx;

namespace Simscop.Pl.WPF
{
    /// <summary>
    /// Interaction logic for DemoWindow.xaml
    /// </summary>
    public partial class DemoWindow
    {
        public DemoWindow()
        {
            InitializeComponent();


            var camera = new ToupTek();

            Debug.WriteLine($"{camera.Valid() && camera.Initialize()}");

            camera.Exposure = 1000;

            camera.OnCaptureChanged += mat =>
            {

                Image.ImageSource = mat.ToWriteableBitmap(0, 0, PixelFormats.Bgr32, null);

            };

            
        }


    }
}
