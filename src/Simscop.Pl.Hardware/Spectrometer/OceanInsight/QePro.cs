using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simscop.Pl.Core.Services;
using OmniDriver;
namespace Simscop.Pl.Hardware.Spectrometer.OceanInsight;

/// <summary>
/// 
/// </summary>
public class QePro : ISpectrometerService
{
    public NETWrapper Wrapper { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public QePro()
    {
        if (Wrapper is null)
            throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="wrapper"></param>
    public QePro(NETWrapper wrapper) => Wrapper = wrapper;

    /// <inheritdoc/>
    public string? Model { get; set; }

    /// <inheritdoc/>
    public string? SerialNumber { get; set; }

    /// <inheritdoc/>
    public string? Fireware { get; set; }

    /// <inheritdoc/>
    public string? HardwareVersion { get; set; }

    /// <inheritdoc/>
    public Dictionary<string, string>? Reserved { get; set; }

    /// <summary>
    /// <inheritdoc />
    /// <code>
    /// 1. 验证环境变量，x86还是x64，这里为了方便直接跳过
    /// 2. 验证当前目录是否有需要的dll，方便直接跳过
    /// 3. 验证设备是否连接，这里为了方便直接跳过
    /// </code>
    /// </summary>
    /// <returns></returns>
    public bool Valid() => true;

    /// <summary>
    /// <inheritdoc />
    /// <code>
    /// 1. 检测设备并连接
    /// 2. 分配结果
    /// </code>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool Initialize() => true;

    /// <inheritdoc />
    public bool DeInitialize() => true;

    public string? LastErrorMessage => null;

    /// <inheritdoc />
    public int Index { get; set; } = 0;

    /// <inheritdoc />
    public int BoxcarWidth
    {
        get => Wrapper.getBoxcarWidth(Index);
        set => Wrapper.setBoxcarWidth(Index, value);
    }

    /// <inheritdoc />
    public int IntegrationStepIncrement => Wrapper.getIntegrationStepIncrement(Index);

    /// <inheritdoc />
    public int IntegrationTime
    {
        get => Wrapper.getIntegrationTime(Index);
        set => Wrapper.setIntegrationTime(Index, value);
    }

    /// <inheritdoc />
    public int MinimumIntegrationTime => Wrapper.getMinimumIntegrationTime(Index);

    /// <inheritdoc />
    public int MaximumIntegrationTime => Wrapper.getMaximumIntegrationTime(Index);

    /// <inheritdoc />
    public int MaximumIntensity => Wrapper.getMaximumIntensity(Index);

    /// <inheritdoc />
    public int NumberOfDarkPixel => Wrapper.getNumberOfDarkPixels(Index);

    /// <inheritdoc />
    public int NumberOfPixels => Wrapper.getNumberOfPixels(Index);

    /// <inheritdoc />
    public int ScansToAverage
    {
        get => Wrapper.getScansToAverage(Index);
        set => Wrapper.setScansToAverage(Index, value);
    }

    /// <inheritdoc />
    public double GetWaveLength(int pixel, int channel = 0)
        => Wrapper.getWavelength(Index, channel, pixel);

    /// <inheritdoc />
    public double[] GetWavelengths()
        => Wrapper.getWavelengths(Index);

    /// <inheritdoc />
    public double[] GetSpectrum() => Wrapper.getSpectrum(Index);

    /// <inheritdoc />
    public string GetEepromInfo(int slot) => Wrapper.getEEPromInfo(Index, slot);
}