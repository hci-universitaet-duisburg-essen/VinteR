using System;
using System.Linq;
using System.Net.Http;
using Ninject;
using NUnit.Framework;
using VinteR.Configuration;
using VinteR.MainApplication;
using VinteR.Rest;

namespace VinteR.Tests
{
    [TestFixture]
    public class TestRestServer
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private const int RestPort = 8050;
        private HttpClient _httpClient;
        private IMainApplication _mainApplication;

        [OneTimeSetUp]
        public void SetUpTestOnce()
        {
            var ninjectKernel = new StandardKernel(new VinterNinjectTestModule());
            var config = ninjectKernel.Get<IConfigurationService>();
            config.GetConfiguration().Rest.Port = RestPort;
            config.GetConfiguration().StartMode = "playback";

            _mainApplication = ninjectKernel.Get<IMainApplication>();
            _mainApplication.Start();
            _httpClient = new HttpClient();
        }

        [OneTimeTearDown]
        public void TearDownOnce()
        {
            _mainApplication.Exit();
        }

        [Test]
        public void TestGetSession()
        {
            var url = $"http://localhost:{RestPort}/session?name=testsession&source=MongoDB";
            Logger.Info(url);
            try
            {
                var bytes = _httpClient.GetByteArrayAsync(url).Result;
                var session = Model.Gen.Session.Parser.ParseFrom(bytes);

                Assert.AreEqual(20, session.Meta.Duration);
                Assert.AreEqual(DateTime.MinValue.ToUniversalTime().ToBinary(), session.Meta.SessionStartMillis);
                Assert.AreEqual("optitrack", session.Frames[0].SourceId);
                Assert.AreEqual(new Model.Gen.MocapFrame.Types.Body.Types.Vector3() { X = 1, Y = 1, Z = 1 },
                    session.Frames[0].Bodies[0].Centroid);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
        }

        [Test]
        public void TestGetSessions()
        {
            var url = $"http://localhost:{RestPort}/sessions";
            Logger.Info(url);
            var uri = new Uri(url);
            var bytes = _httpClient.GetByteArrayAsync(uri).Result;
            var meta = Model.Gen.SessionsMetadata.Parser.ParseFrom(bytes);

            Assert.AreEqual(1, meta.InputSourceMeta.Count);
            Assert.AreEqual("MongoDB", meta.InputSourceMeta[0].SourceId);
            Assert.AreEqual(1, meta.InputSourceMeta[0].SessionMeta.Count);
        }
    }
}