using System;
using Grapevine.Server;
using VinteR.Configuration;
using VinteR.Model;

namespace VinteR.OutputAdapter.Rest
{
    public class VinterRestServer : IOutputAdapter
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly Configuration.Rest _config;
        private readonly IVinterRestRoute[] _routes;
        private bool _isRunning;
        private RestServer _restServer;

        public VinterRestServer(IConfigurationService configurationService, IVinterRestRoute[] routes)
        {
            _config = configurationService.GetConfiguration().Rest;
            _routes = routes;
        }

        public void OnDataReceived(MocapFrame mocapFrame)
        {
        }

        public void Start(Model.Session session)
        {
            if (!_config.Enabled) return;

            try
            {
                _restServer = new RestServer
                {
                    Host = _config.Host,
                    Port = _config.Port.ToString()
                };

                IRouter router = new Router();
                foreach (var route in _routes)
                {
                    router.Register(route.Handler, route.HttpMethod, route.PathInfo);
                }

                _restServer.Router = router;
                _restServer.Start();
                _isRunning = true;
                Logger.Info("REST server running on {0}:{1}", _config.Host, _config.Port);
            }
            catch (Exception e)
            {
                var msg = $"Could not start rest server on {_config.Host}:{_config.Port}, cause: {e.Message}";
                throw new ApplicationException(msg, e);
            }
        }

        public void Stop()
        {
            if (!_isRunning) return;

            _restServer.Stop();
            Logger.Info("REST server stopped");
        }
    }
}