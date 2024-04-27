using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Lift.UI.Tools.Extension;
using Simscop.Pl.Core.Models.Charts;
using Simscop.Pl.WPF.Helpers;

namespace Simscop.Pl.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for AxisView.xaml
    /// </summary>
    public partial class AxisView : UserControl
    {
        public AxisView()
        {
            InitializeComponent();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }
    }
}
