using NUnit.Framework;
using VinteR.Configuration;

namespace VinteR.Tests
{
    [TestFixture]
    public class TestVinterConfigurationService
    {
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
