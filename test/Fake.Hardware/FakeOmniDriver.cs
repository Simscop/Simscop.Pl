using Simscop.Pl.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fake.Hardware;

public class FakeOmniDriver : IOmniDriverService
{

    public string? Model { get; set; } = "FakeOmniDriver";
    public string? SerialNumber { get; set; } = "v1.0";
    public string? Fireware { get; set; } = "v1.0";
    public string? HardwareVersion { get; set; } = "v1.0";
    public Dictionary<string, string>? Reserved { get; set; }
    public bool Valid() => true;

    public bool Initialize() => true;

    public bool DeInitialize() => true;

    public string? LastErrorMessage => null;

    public bool OpenSpectrometers(int index) => index == 0;

    public bool OpenAllSpectrometers() => true;

    public void CloseSpectrometers(int index) { }

    public void CloseAllSpectrometers() { }

    public ISpectrometerService[] GetAllSpectrometer()
    {
        return new ISpectrometerService[]
        {
            new FakeSpectrometer()
        };
    }

    public int NumberOfSpectrometers { get; } = 1;
}