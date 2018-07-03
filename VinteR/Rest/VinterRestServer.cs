using System;
using Grapevine.Server;
using VinteR.Configuration;

namespace VinteR.Rest
{
    public class VinterRestServer : IRestServer
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        private readonly Configuration.Rest _config;
        private readonly IRestRouter[] _routers;

        private bool _isRunning;
        private RestServer _restServer;

        public VinterRestServer(IConfigurationService configurationService, IRestRouter[] routers)
        {
            _config = configurationService.GetConfiguration().Rest;
            _routers = routers;
        }

        public void Start()
        {
            // do not start if not enabled
            if (!_config.Enabled) return;

            if (_isRunning)
            {
                Logger.Warn("Ignoring start(). REST server already running");
                return;
            }

            try
            {
                // create server instance
                _restServer = new RestServer
                {
                    Host = _config.Host,
                    Port = _config.Port.ToString()
                };

                // bind all registered routes to router (see constructor for bindings)
                IRouter router = new Router();
                foreach (var route in _routers)
                {
                    route.Register(router);
                }
                _restServer.Router = router;

                // start the server
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