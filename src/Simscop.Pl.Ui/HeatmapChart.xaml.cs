using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Simscop.Pl.Ui
{
    /// <summary>
    /// Interaction logic for HeatmapChart.xaml
    /// </summary>
    public partial class HeatmapChart : UserControl
    {
        private DispatcherTimer _timer;

        public HeatmapChart()
        {
            InitializeComponent();

            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private void _timer_Tick(object? sender, EventArgs e)
        {
            var data = new double[20, 20];

            double k = Math.Pow(2, 0.1);

            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    data[i, j] = Math.Pow(k, (double)i) * (double)j / 4.0;
                }
            }

            var model = new PlotModel { };
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
            model.Axes.Add(new LinearColorAxis
            {
                Position = AxisPosition.Right,
                Palette = OxyPalettes.Gray(100),
                HighColor = OxyColors.White,
                LowColor = OxyColors.Black
            });

            var hms = new HeatMapSeries
            {
                X0 = 0,
                X1 = 20,
                Y0 = 0,
                Y1 = 20,
                Data = data,
                Interpolate = false,
                RenderMethod = HeatMapRenderMethod.Bitmap,
                LabelFontSize = 0
            };

            model.Series.Add(hms);

            View.Model = model;

            Debug.WriteLine("Render");
        }
    }
}
