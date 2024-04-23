using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Fake.Hardware;
using OxyPlot;
using OxyPlot.Series;
using Simscop.Pl.Hardware;
using Simscop.Pl.Hardware.Spectrometer.OceanInsight;
using Simscop.Pl.Ui;

namespace Example.Spectrometer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        var manager = new FakeOmniDriver();
        manager.OpenAllSpectrometers();
        var spe = manager.GetAllSpectrometer()[0];
        spe.BoxcarWidth = 0;

        Task.Run(() =>
        {
            while (true)
            {
                var x = spe.GetWavelengths();
                var y = spe.GetSpectrum();

                var count = spe.NumberOfPixels;

                if (x.Length != count || y.Length != count) throw new Exception();

                var line = new LineSeries();
                var line2 = new LineSeries();

                line.Points.Clear();
                line2.Points.Clear();
                for (var i = 0; i < count; i++)
                {
                    line.Points.Add(new DataPoint(x[i], y[i]));
                    line2.Points.Add(new DataPoint(x[i], y[i] / 2));
                }


                Thread.Sleep(10);
                try
                {
                    Application.Current?.Dispatcher.Invoke(() => LineChart.ShowSerial(line, 0));
                }
                catch (Exception e) { }
            }
        });
    }
}
