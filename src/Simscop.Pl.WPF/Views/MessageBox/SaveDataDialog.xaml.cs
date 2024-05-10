using System.Windows;
using Simscop.Pl.Core;
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

        vm.Direcotry = WinformNative.ShowFolderDialog() ?? vm.Direcotry;
    }
}
