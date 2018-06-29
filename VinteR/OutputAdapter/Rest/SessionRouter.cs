using System;
using System.Linq;
using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Shared;
using NLog;
using VinteR.Input;
using VinteR.Model;
using VinteR.Serialization;

namespace VinteR.OutputAdapter.Rest
{
    public class SessionRouter : IRestRouter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IQueryService[] _queryServices;
        private readonly IHttpResponseWriter _responseWriter;
        private readonly ISerializer _serializer;

        public SessionRouter(IQueryService[] queryServices, IHttpResponseWriter responseWriter, ISerializer serializer)
        {
            _queryServices = queryServices;
            _responseWriter = responseWriter;
            _serializer = serializer;
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
            try
            {
                var session = GetSession(context);
                return null;
            }
            catch (InvalidArgumentException e)
            {
                return _responseWriter.SendError(e.StatusCode, e.Message, context);
            }
        }

        private IHttpContext HandleGetSession(IHttpContext context)
        {
            try
            {
                var session = GetSession(context);
                _serializer.ToProtoBuf(session, out Model.Gen.Session protoSession);
                return _responseWriter.SendProtobufMessage(protoSession, context);
            }
            catch (InvalidArgumentException e)
            {
                return _responseWriter.SendError(e.StatusCode, e.Message, context);
            }
        }

        private Session GetSession(IHttpContext context)
        {
            // validate source parameter present
            var source = context.Request.QueryString["source"] ?? string.Empty;
            if (source == string.Empty)
                throw new InvalidArgumentException(HttpStatusCode.BadRequest, "Parameter 'source' is missing");

            // validate source is on query services
            if (!_queryServices.Select(qs => qs.GetStorageName()).Contains(source))
                throw new InvalidArgumentException(HttpStatusCode.NotFound, "Source " + source + " not found");

            // validate session name parameter present
            var sessionName = context.Request.QueryString["name"] ?? string.Empty;
            if (sessionName == string.Empty)
                throw new InvalidArgumentException(HttpStatusCode.BadRequest, "Parameter 'name' is missing");

            // validate start time
            var startTime = context.Request.QueryString["start"] ?? "0";
            if (!int.TryParse(startTime, out var start))
                throw new InvalidArgumentException(HttpStatusCode.BadRequest, "Parameter 'start' contains no number >= 0");

            // validate end time
            var endTime = context.Request.QueryString["end"] ?? "-1";
            if (!int.TryParse(endTime, out var end))
                throw new InvalidArgumentException(HttpStatusCode.BadRequest, "Parameter 'end' contains no number >= -1");

            var queryService = _queryServices.Where(qs => qs.GetStorageName() == source)
                .Select(qs => qs)
                .First();
            var session = queryService.GetSession(sessionName, start, end);
            return session;
        }
    }
}