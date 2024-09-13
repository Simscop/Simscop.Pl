using System.ComponentModel;

namespace Simscop.Pl.WPF
{
    /// <summary>
    /// ScanView.xaml 的交互逻辑
    /// </summary>
    public partial class ScanView : Lift.UI.Controls.Window
    {
        public ScanView()
        {
            InitializeComponent();

            DataContext = VmManager.ScanViewModel;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }


    }
}
