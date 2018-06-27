using System;
using System.Collections.Generic;
using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Shared;
using NLog;
using VinteR.Input;
using VinteR.Model;

namespace VinteR.OutputAdapter.Rest
{
    public class SessionsRouter : IRestRouter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IQueryService[] _queryServices;
        private readonly IHttpResponseWriter _responseWriter;

        public SessionsRouter(IQueryService[] queryServices, IHttpResponseWriter responseWriter)
        {
            _queryServices = queryServices;
            _responseWriter = responseWriter;
        }

        public void Register(IRouter router)
        {
            Register(HandleGetSessions, HttpMethod.GET, "/sessions", router);
        }

        private void Register(Func<IHttpContext, IHttpContext> func, HttpMethod method, string pathInfo, IRouter router)
        {
            router.Register(func, method, pathInfo);
            Logger.Info("Registered path {0,-15} to {1,15}.{2}#{3}", pathInfo, GetType().Name, func.Method.Name, method);
        }

        private IHttpContext HandleGetSessions(IHttpContext context)
        {
            IDictionary<string, IEnumerable<Session>> result = new Dictionary<string, IEnumerable<Session>>();
            foreach (var queryService in _queryServices)
            {
                result.Add(queryService.GetStorageName(), queryService.GetSessions());
            }
            _responseWriter.SendJsonResponse(result, context);
            return context;
        }
    }
}