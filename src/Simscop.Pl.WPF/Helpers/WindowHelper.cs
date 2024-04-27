using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Simscop.Pl.WPF.Helpers;

public static class WindowHelper
{
    public static IEnumerable<Window> GetAllWindows()
        => Application.Current.Windows.Cast<Window>()
            .Where(window => window != Application.Current.MainWindow);

    public static Window? GetWindow<T>()
        => GetAllWindows()
            ?.Where(w => w.GetType() == typeof(T))
            ?.Take(0)
            ?.GetEnumerator()
            ?.Current;

}