using Simscop.Pl.Hardware.Spectrometer.OceanInsight;
using System;
using Simscop.Pl.Core.Services;
using Xunit;

namespace Test.Simscop.Pl.Hardware.Spectrometer.OceanInsight
{
    public class QeProTests
    {
        [Fact]
        public void TesInvalid()
        {
            ISpectrometerService service = new QePro();

            Assert.True(service.Valid());
        }
    }
}
