namespace Simscop.Pl.Core.Services;

// todo 默认不考虑多通道的情况，如果有再修订

public interface ISpectrometerService : IDeviceService
{
    /// <summary>
    /// 设备的编号
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// 滑动窗口宽度
    /// </summary>
    public int BoxcarWidth { get; set; }

    /// <summary>
    /// int the integration step size in microseconds
    /// </summary>
    public int IntegrationStepIncrement { get; }

    /// <summary>
    /// 积分时间，单位microseconds(μs)
    /// </summary>
    public int IntegrationTime { get; set; }

    /// <summary>
    /// 最小积分时间，单位microseconds(μs)
    /// </summary>
    public int MinimumIntegrationTime { get; }

    /// <summary>
    /// 最大积分时间，单位microseconds(μs)
    /// </summary>
    public int MaximumIntegrationTime { get; }

    /// <summary>
    /// CCD 像素的最大可能值
    /// </summary>
    public int MaximumIntensity { get; }

    /// <summary>
    /// 暗像素数
    /// </summary>
    public int NumberOfDarkPixel { get; }

    /// <summary>
    /// 像素数
    /// </summary>
    public int NumberOfPixels { get; }

    /// <summary>
    /// 平均结果
    /// </summary>
    public int ScansToAverage { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pixel"></param>
    /// <param name="channel"></param>
    /// <returns></returns>
    public double GetWaveLength(int pixel, int channel = 0);

    /// <summary>
    /// 单位nm
    /// </summary>
    /// <returns></returns>
    public double[] GetWavelengths();

    /// <summary>
    /// Acquire the next spectrum from the spectrometer.
    /// 单位是a.u.
    /// </summary>
    /// <returns></returns>
    public double[] GetSpectrum();

    /// <summary>
    /// EEPROM - Electrically Erasable Programmable Read-Only Memory
    ///
    /// <code>
    /// slot - 0-N the parameter position within the spectrometer's non-volatile memory we wish to set.
    /// Standard locations are as follows:
    /// CAUTION: not all spectrometers follow these conventions. Be sure to check the data sheets for your spectrometer.
    /// 0 - serial number (this slot may not be written by the customer)
    /// 1 - wavelength calibration coefficient - 0th order (aka "intercept")
    /// 2 - wavelength calibration coefficient - 1st order
    /// 3 - wavelength calibration coefficient - 2nd order
    /// 4 - wavelength calibration coefficient - 3rd order
    /// 5 - stray light constant
    /// 6 - non-linearity correction coefficient - 0th order
    /// 7 - non-linearity correction coefficient - 1st order
    /// 8 - non-linearity correction coefficient - 2nd order
    /// 9 - non-linearity correction coefficient - 3rd order
    /// 10 - non-linearity correction coefficient - 4th order
    /// 11 - non-linearity correction coefficient - 5th order
    /// 12 - non-linearity correction coefficient - 6th order
    /// 13 - non-linearity correction coefficient - 7th order
    /// 14 - polynomial order of non-linearity calibration
    /// 15 - optical bench configuration (not writable by the customer)
    ///     Format: gg fff sss
    ///     gg: grating number
    ///     fff: filter wavelength
    ///     sss: slit size
    /// 16 - USB2000 configuration (not writable by the customer)
    ///     Format: A W L V
    ///     A: array coating manufacturer
    ///     W: array wavelength (VIS, UV, OFLV)
    ///     L: L2 lens installed
    ///     V: CPLD version
    /// </code>
    /// </summary>
    /// <param name="slot"></param>
    /// <returns></returns>
    public string GetEepromInfo(int slot);
}
