using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Simscop.Pl.WPF.Helpers;

public static class LogicalTreeHelperExtensions
{
    public static IEnumerable<T> FindLogicalChildren<T>(this DependencyObject? parent) where T : DependencyObject
    {
        if (parent == null) yield break;

        if (parent is T pt)
            yield return pt;


        foreach (object logicalChild in LogicalTreeHelper.GetChildren(parent))
        {
            if (logicalChild is not DependencyObject depObj) continue;

            foreach (T child in FindLogicalChildren<T>(depObj))
                yield return child;
            
        }
    }
}