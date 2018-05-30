using Newtonsoft.Json;
using NUnit.Framework;
using VinteR.Configuration;

namespace VinteR.Tests
{
    [TestFixture]
    public class TestVinterConfigurationService
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [Test]
        public void TestGetConfiguration()
        {
            var configurationService = new VinterConfigurationService();
            var configuration = configurationService.GetConfiguration();

            Assert.IsNotNull(configuration);
            Assert.AreEqual("C:\\VinteRData", configuration.HomeDir);
        }
    }
}