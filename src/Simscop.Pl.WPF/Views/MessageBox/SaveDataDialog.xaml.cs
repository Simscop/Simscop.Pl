using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;
using Simscop.Pl.WPF.ViewModels;

namespace Simscop.Pl.WPF.Views.MessageBox;

/// <summary>
/// Interaction logic for SaveDataDialog.xaml
/// </summary>
public partial class SaveDataDialog : Window
{
    public SaveDataDialog()
    {
        InitializeComponent();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm) return;

        var openFileDialog = new CommonOpenFileDialog();
        if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            vm.Direcotry = openFileDialog.FileName;
    }
}
