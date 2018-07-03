using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Grapevine.Client;
using Ninject;
using NUnit.Framework;
using VinteR.Configuration;
using VinteR.Model;
using VinteR.OutputAdapter.Rest;
using HttpMethod = Grapevine.Shared.HttpMethod;

namespace VinteR.Tests
{
    [TestFixture]
    public class TestRestServer
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private VinterRestServer _restServer;
        private HttpClient _httpClient;

        [OneTimeSetUp]
        public void SetUpTestOnce()
        {
            var ninjectKernel = new StandardKernel(new VinterNinjectTestModule());
            var config = ninjectKernel.Get<IConfigurationService>();

            config.GetConfiguration().Rest.Port = 9001;
            var restRouters = ninjectKernel.GetAll<IRestRouter>().Select(r => r).ToArray();
            _restServer = new VinterRestServer(config, restRouters);
            _restServer.Start(new Session("testsession"));
            _httpClient = new HttpClient();
        }

        [OneTimeTearDown]
        public void TearDownOnce()
        {
            _restServer.Stop();
        }

        [Test]
        public void TestGetSession()
        {
            var uri = new Uri("http://localhost:8010/session?name=testsession&source=MongoDB");
            var bytes = _httpClient.GetByteArrayAsync(uri).Result;
            var session = Model.Gen.Session.Parser.ParseFrom(bytes);

            Assert.AreEqual(20, session.Meta.Duration);
            Assert.AreEqual(DateTime.MinValue.ToUniversalTime().ToBinary(), session.Meta.SessionStartMillis);
            Assert.AreEqual("optitrack", session.Frames[0].SourceId);
            Assert.AreEqual(new Model.Gen.MocapFrame.Types.Body.Types.Vector3() { X = 1, Y = 1, Z = 1 }, 
                session.Frames[0].Bodies[0].Centroid);
        }

        [Test]
        public void TestGetSessions()
        {
            var uri = new Uri("http://localhost:8010/sessions");
            var bytes = _httpClient.GetByteArrayAsync(uri).Result;
            var meta = Model.Gen.SessionsMetadata.Parser.ParseFrom(bytes);

            Assert.AreEqual(1, meta.InputSourceMeta.Count);
            Assert.AreEqual("MongoDB", meta.InputSourceMeta[0].SourceId);
            Assert.AreEqual(1, meta.InputSourceMeta[0].SessionMeta.Count);
        }
    }
}