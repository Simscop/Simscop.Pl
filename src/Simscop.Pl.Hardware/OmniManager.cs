using Simscop.Pl.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmniDriver;
using Simscop.Pl.Hardware.Spectrometer.OceanInsight;

namespace Simscop.Pl.Hardware;

public class OmniManager : IOmniDriverService
{
    public NETWrapper Wrapper { get; set; } = new NETWrapper();

    /// <inheritdoc />
    public string? Model { get; set; } = "none";

    /// <inheritdoc />
    public string? SerialNumber { get; set; } = "none";

    /// <inheritdoc />
    public string? Fireware { get; set; } = "none";

    /// <inheritdoc />
    public string? HardwareVersion { get; set; } = "none";

    /// <inheritdoc />
    public Dictionary<string, string>? Reserved { get; set; }

    /// <inheritdoc />
    public bool Valid()
    {
        HardwareVersion = Wrapper.getApiVersion();

        return false;
    }

    /// <inheritdoc />
    public bool Initialize() => Wrapper.openAllSpectrometers() != 0;

    /// <inheritdoc />
    public bool DeInitialize()
    {
        Wrapper.closeAllSpectrometers();
        return true;
    }

    public string? LastErrorMessage => null;

    public bool OpenSpectrometers(int index) => true;

    public bool OpenAllSpectrometers()
    {
        Wrapper.openAllSpectrometers();
        return true;
    }

    public void CloseSpectrometers(int index)
    {
        Wrapper.closeSpectrometer(index);
        return;
    }

    public void CloseAllSpectrometers() => Wrapper.closeAllSpectrometers();

    public ISpectrometerService[] GetAllSpectrometer()
    {
        var iss = new List<ISpectrometerService>();
        for (int i = 0; i < NumberOfSpectrometers; i++)
            iss.Add(new QePro(Wrapper)
            {
                //Model = Wrapper.getName(i),
                //SerialNumber = Wrapper.getSerialNumber(i),
                //Fireware = Wrapper.getFirmwareVersion(1),
                HardwareVersion = "none",
                Index = i,
            });

        return iss.ToArray();
    }

    public int NumberOfSpectrometers => Wrapper.getNumberOfSpectrometersFound();
}