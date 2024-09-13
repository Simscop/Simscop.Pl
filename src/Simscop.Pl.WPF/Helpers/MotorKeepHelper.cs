using Simscop.Pl.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simscop.Pl.WPF.Helpers;

public static class MotorKeepHelper
{
    const string FileName = "motor.txt";

    public static void Restore()
    {
        if (!File.Exists(FileName)) return;

        var str = File.ReadAllText(FileName);
        var posAll = str.Split(new char[] { ',' });

        if (posAll.Length != 3) return;

        double.TryParse(posAll[0], out var x);
        double.TryParse(posAll[1], out var y);
        double.TryParse(posAll[2], out var z);

        if (!HardwareManager.IsMotorOk) return;

        HardwareManager.Motor!.AsyncSetAbsolutePosition(new bool[] { true, true, true }, new double[] { x, y, z }).Wait();
    }

    public static void Keep()
    {
        if (!HardwareManager.IsMotorOk) return;
        var xyz = HardwareManager.Motor!.Xyz;

        File.WriteAllText(FileName, $"{xyz.X},{xyz.Y},{xyz.Z}");
    }
}