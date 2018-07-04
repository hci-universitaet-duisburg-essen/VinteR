using System;
using System.Collections.Generic;
using System.Linq;
using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Shared;
using NLog;
using VinteR.Input;
using VinteR.Model.Gen;
using VinteR.Serialization;
using Session = VinteR.Model.Session;

namespace VinteR.Rest
{

    public class SessionsRouter : IRestRouter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public event RecordCalledEventHandler OnRecordSessionCalled;
        public event RecordCalledEventHandler OnStopRecordCalled;

        // ignored as they are threaded in the SessionRouter
        public event SessionPlayEventHandler OnPlayCalled;
        public event EventHandler OnPausePlaybackCalled;
        public event EventHandler OnStopPlaybackCalled;
        public event EventHandler<uint> OnJumpPlaybackCalled;

        private readonly IQueryService[] _queryServices;
        private readonly IHttpResponseWriter _responseWriter;
        private readonly ISerializer _serializer;

        public SessionsRouter(IQueryService[] queryServices, IHttpResponseWriter responseWriter, ISerializer serializer)
        {
            _queryServices = queryServices;
            _responseWriter = responseWriter;
            _serializer = serializer;
        }

        public void Register(IRouter router)
        {
            Register(HandleGetSessions, HttpMethod.GET, "/sessions", router);
            Register(HandleRecordSessions, HttpMethod.POST, "/sessions/record", router);
            Register(HandleStopRecord, HttpMethod.GET, "/sessions/record/stop", router);
        }

        private void Register(Func<IHttpContext, IHttpContext> func, HttpMethod method, string pathInfo, IRouter router)
        {
            router.Register(func, method, pathInfo);
            Logger.Info("Registered path {0,-15} to {1,15}.{2}#{3}", pathInfo, GetType().Name, func.Method.Name,
                method);
        }

        private IHttpContext HandleGetSessions(IHttpContext context)
        {
            var sessionsMetadata = new SessionsMetadata();
            foreach (var queryService in _queryServices)
            {
                var inputSourceMetadata =
                    new SessionsMetadata.Types.InputSourceMetadata {SourceId = queryService.GetStorageName()};

                var sessions = queryService.GetSessions().Select(s =>
                {
                    _serializer.ToProtoBuf(s, out SessionMetadata meta);
                    var sessionMetadata = meta;
                    return sessionMetadata;
                });
                inputSourceMetadata.SessionMeta.AddRange(sessions);
                sessionsMetadata.InputSourceMeta.Add(inputSourceMetadata);
            }

            _responseWriter.SendProtobufMessage(sessionsMetadata, context);
            return context;
        }

        private IHttpContext HandleRecordSessions(IHttpContext context)
        {
            var session = OnRecordSessionCalled?.Invoke();
            _serializer.ToProtoBuf(session, out SessionMetadata meta);
            _responseWriter.SendProtobufMessage(meta, context);
            return context;
        }

        private IHttpContext HandleStopRecord(IHttpContext context)
        {
            var session = OnStopRecordCalled?.Invoke();
            if (session == null)
            {
                context.Response.StatusCode = HttpStatusCode.NotModified;
                var msg = new Dictionary<string, string> {{"msg", "No record to stop"}};
                _responseWriter.SendJsonResponse(msg, context);
            }
            else
            {
                context.Response.StatusCode = HttpStatusCode.Accepted;
                _serializer.ToProtoBuf(session, out SessionMetadata meta);
                _responseWriter.SendProtobufMessage(meta, context);
            }
            return context;
        }
    }
}