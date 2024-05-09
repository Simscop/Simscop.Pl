using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lift.UI.Controls;
using Lift.UI.Data;
using Color = System.Windows.Media.Color;
using UserControl = System.Windows.Controls.UserControl;

namespace Simscop.Pl.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for AnnotationView.xaml
    /// </summary>
    public partial class AnnotationView : UserControl
    {
        public AnnotationView()
        {
            InitializeComponent();
        }

        private void TextConfirm(object? sender, FunctionEventArgs<Color> e)
        {
            if (sender is not ColorPicker { Parent: Popup popup }) return;
            
            popup.IsOpen = false;
        }

    }
}
