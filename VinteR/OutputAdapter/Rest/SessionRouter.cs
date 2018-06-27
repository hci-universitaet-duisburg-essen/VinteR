using System;
using System.Linq;
using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Shared;
using NLog;
using VinteR.Input;

namespace VinteR.OutputAdapter.Rest
{
    public class SessionRouter : IRestRouter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IQueryService[] _queryServices;
        private readonly IHttpResponseWriter _responseWriter;

        public SessionRouter(IQueryService[] queryServices, IHttpResponseWriter responseWriter)
        {
            _queryServices = queryServices;
            _responseWriter = responseWriter;
        }

        public void Register(IRouter router)
        {
            Register(HandlePlaySession, HttpMethod.GET, "/session/play", router);
            Register(HandleGetSession, HttpMethod.GET, "/session", router);
        }

        private void Register(Func<IHttpContext, IHttpContext> func, HttpMethod method, string pathInfo, IRouter router)
        {
            router.Register(func, method, pathInfo);
            Logger.Info("Registered path {0,-15} to {1,15}.{2}#{3}", pathInfo, GetType().Name, func.Method.Name,
                method);
        }

        private IHttpContext HandlePlaySession(IHttpContext context)
        {
            return context;
        }

        private IHttpContext HandleGetSession(IHttpContext context)
        {
            // validate source parameter present
            var source = context.Request.QueryString["source"] ?? string.Empty;
            if (source == string.Empty)
                return _responseWriter.SendError(HttpStatusCode.BadRequest, "Parameter 'source' is missing", context);

            // validate source is on query services
            if (!_queryServices.Select(qs => qs.GetStorageName()).Contains(source))
                return _responseWriter.SendError(HttpStatusCode.NotFound, "Source " + source + " not found", context);

            // validate session name parameter present
            var sessionName = context.Request.QueryString["name"] ?? string.Empty;
            if (sessionName == string.Empty)
                return _responseWriter.SendError(HttpStatusCode.BadRequest, "Parameter 'name' is missing", context);

            // validate start time
            var startTime = context.Request.QueryString["start"] ?? "0";
            if (!int.TryParse(startTime, out var start))
                return _responseWriter.SendError(HttpStatusCode.BadRequest, "Parameter 'start' contains no number >= 0", context);

            // validate end time
            var endTime = context.Request.QueryString["end"] ?? "-1";
            if (!int.TryParse(endTime, out var end))
                return _responseWriter.SendError(HttpStatusCode.BadRequest, "Parameter 'end' contains no number >= -1", context);

            var queryService = _queryServices.Where(qs => qs.GetStorageName() == source)
                .Select(qs => qs)
                .First();
            var session = queryService.GetSession(sessionName, start, end);
            return _responseWriter.SendJsonResponse(session, context);
        }
    }
}