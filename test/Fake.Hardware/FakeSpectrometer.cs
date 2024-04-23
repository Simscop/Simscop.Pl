using Simscop.Pl.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenCvSharp.LineIterator;

namespace Fake.Hardware;

public class FakeSpectrometer : ISpectrometerService
{
    public string? Model { get; set; } = "FakeSpectrometer";
    public string? SerialNumber { get; set; } = "v1.0";
    public string? Fireware { get; set; } = "v1.0";
    public string? HardwareVersion { get; set; } = "v1.0";
    public Dictionary<string, string>? Reserved { get; set; }
    public bool Valid() => true;

    public bool Initialize() => true;

    public bool DeInitialize() => true;

    public int Index { get; set; } = 0;
    public int BoxcarWidth { get; set; } = 0;
    public int IntegrationStepIncrement { get; } = 0;
    public int IntegrationTime { get; set; } = 10;
    public int MinimumIntegrationTime { get; } = 0;
    public int MaximumIntegrationTime { get; } = int.MaxValue;
    public int MaximumIntensity { get; } = 0;
    public int NumberOfDarkPixel { get; } = 0;
    public int NumberOfPixels { get; } = 1024;
    public int ScansToAverage { get; set; } = 0;

    private readonly Random _random = new();

    public double GetWaveLength(int pixel, int channel = 0) => 0.1 * pixel;

    public double[] GetWavelengths() => Enumerable.Range(0, NumberOfPixels).Select(item => item * 0.1).ToArray();

    double Gaussian(double x, double mean, double stdDev)
        => 1 / (stdDev * Math.Sqrt(2 * Math.PI)) * Math.Exp(-Math.Pow(x - mean, 2) / (2 * Math.Pow(stdDev, 2)));

    double BimodalCurve(double x, double mean1, double stdDev1, double mean2, double stdDev2, double weight)
    {
        var gaussian1 = Gaussian(x, mean1, stdDev1);
        var gaussian2 = Gaussian(x, mean2, stdDev2);
        return weight * gaussian1 + (1 - weight) * gaussian2;
    }

    public double[] GetSpectrum() => Enumerable.Range(0, NumberOfPixels).Select(item =>
    {
        var noise = _random.NextDouble();
        var x = (double)item / NumberOfPixels * Math.PI * 2;

        return BimodalCurve(x * 15, 15, 20, 70, 15, 0.5) * 1000 + noise * (1.0 / (BoxcarWidth + 1));
    }).ToArray();

    public string GetEepromInfo(int slot) => "none";
}