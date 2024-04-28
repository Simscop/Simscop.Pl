using System.Diagnostics;
using System.Windows.Media;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
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

            var mat = Cv2.ImRead(@"C:\Users\haeer\Desktop\icon-plus.tif", ImreadModes.Unchanged);
            Image.ImageSource = mat.ToBitmapSource();

            Debug.WriteLine($"{mat.Channels()} - {mat.Depth()}");

            Image.OnMarkderChanged += (r) =>
            {
                Debug.WriteLine(r);
            };

            Image.GetImageColorFromPosition = (x, y) =>
            {
                if (x < 0 || y < 0) return Colors.Transparent;

                var vec = mat.At<Vec3b>(y, x);
                var color = Color.FromArgb(255, vec.Item2, vec.Item1, vec.Item0);

                Image.Background = new SolidColorBrush(color);

                return color;
            };

        
        }
    }
}
