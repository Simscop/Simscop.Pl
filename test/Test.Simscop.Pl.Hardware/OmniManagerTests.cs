using Simscop.Pl.Hardware;
using System;
using Simscop.Pl.Core.Services;
using Xunit;

namespace Test.Simscop.Pl.Hardware
{
    public class OmniManagerTests
    {
        [Fact]
        public void TestValid()
        {
            var manager = new OmniManager();
            Assert.True(manager.Valid());

            var version = manager.HardwareVersion;
            
        }
    }
}
