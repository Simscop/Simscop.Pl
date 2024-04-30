using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lift.Core.Share.Extensions;
using Simscop.Pl.Core.Services;

namespace Fake.Hardware;

public class FakeMortor : IMotorService
{
    public string? Model { get; set; } = "";
    public string? SerialNumber { get; set; } = "";
    public string? Fireware { get; set; } = "";
    public string? HardwareVersion { get; set; } = "";
    public Dictionary<string, string>? Reserved { get; set; }
    public bool Valid() => true;

    public bool Initialize() => true;

    public bool DeInitialize() => true;

    public string? LastErrorMessage => null;

    public string Unit { get; } = "um";

    public (double X, double Y, double Z) Xyz { get; } = (1, 2, 3);
    public double X { get; private set; } = 0;
    public double Y { get; private set; } = 0;
    public double Z { get; private set; } = 0;
    public void SetRelativePosition(bool[] index, double[] pos)
    {
        if (index.Length != 3 || pos.Length != 3) throw new Exception();

        if (index[0]) X = pos[0];
        if (index[1]) Y = pos[1];
        if (index[2]) Z = pos[2];
    }

    public void SetAbsolutePosition(bool[] index, double[] pos)
    {
        if (index.Length != 3 || pos.Length != 3) throw new Exception();

        if (index[0]) X = pos[0];
        if (index[1]) Y = pos[1];
        if (index[2]) Z = pos[2];

    }
}