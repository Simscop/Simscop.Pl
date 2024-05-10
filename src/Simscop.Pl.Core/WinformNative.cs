using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simscop.Pl.Core;

public static class WinformNative
{
    public static string? ShowFolderDialog()
    {
        var dialog = new FolderBrowserDialog();

        return dialog.ShowDialog() is DialogResult.OK ? dialog.SelectedPath : null;
    }
}