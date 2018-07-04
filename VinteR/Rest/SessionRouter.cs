using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Shared;
using NLog;
using VinteR.Input;
using VinteR.Model.Gen;
using VinteR.Serialization;
using VinteR.Streaming;
using HttpStatusCode = Grapevine.Shared.HttpStatusCode;
using Session = VinteR.Model.Session;

namespace VinteR.Rest
{
    public class SessionRouter : IRestRouter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const string ParamSource = "source";
        private const string ParamSessionName = "name";
        private const string ParamStart = "start";
        private const string ParamEnd = "end";
        private const string ParamHost = "host";
        private const string ParamPort = "port";

        public event RecordCalledEventHandler OnRecordSessionCalled;
        public event RecordCalledEventHandler OnStopRecordCalled;
        public event SessionPlayEventHandler OnPlayCalled;
        public event EventHandler OnPausePlaybackCalled;
        public event EventHandler OnStopPlaybackCalled;
        public event EventHandler<uint> OnJumpPlaybackCalled;

        private readonly IQueryService[] _queryServices;
        private readonly IHttpResponseWriter _responseWriter;
        private readonly ISerializer _serializer;
        private readonly IStreamingServer _streamingServer;

        public SessionRouter(IQueryService[] queryServices, IHttpResponseWriter responseWriter, ISerializer serializer,
            IStreamingServer streamingServer)
        {
            _queryServices = queryServices;
            _responseWriter = responseWriter;
            _serializer = serializer;
            _streamingServer = streamingServer;
        }

        public void Register(IRouter router)
        {
            Register(HandlePlaySession, HttpMethod.GET, "/session/play", router);
            Register(HandlePauseSession, HttpMethod.GET, "/session/pause", router);
            Register(HandleStopSession, HttpMethod.GET, "/session/stop", router);
            Register(HandleJumpSession, HttpMethod.GET, "/session/jump", router);
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
                var source = GetParam(context, ParamSource);
                var sessionName = GetParam(context, ParamSessionName);
                var session = OnPlayCalled?.Invoke(source, sessionName);
                context.Response.StatusCode = HttpStatusCode.Accepted;

                var hostParam = GetParam(context, ParamHost);
                var portParam = GetParam(context, ParamPort);
                if (hostParam != string.Empty && portParam != string.Empty)
                {
                    var ipAddress = IPAddress.Parse(hostParam);
                    if (int.TryParse(portParam, out var port))
                    {
                        _streamingServer.AddReceiver(new IPEndPoint(ipAddress, port));
                        _serializer.ToProtoBuf(session, out SessionMetadata meta);
                        _responseWriter.SendProtobufMessage(meta, context);
                    }
                    else
                    {
                        _responseWriter.SendError(HttpStatusCode.BadRequest,
                            $"Invalid receiver configuration ${hostParam}:${portParam}", context);
                    }
                }
                else
                {
                    _responseWriter.SendError(HttpStatusCode.BadRequest, "host and port must be set", context);
                }
            }
            catch (InvalidArgumentException e)
            {
                _responseWriter.SendError(HttpStatusCode.BadRequest, e.Message, context);
            }

            return context;
        }

        private IHttpContext HandleStopSession(IHttpContext context)
        {
            try
            {
                OnStopPlaybackCalled?.Invoke(this, EventArgs.Empty);
                context.Response.StatusCode = HttpStatusCode.Accepted;
                var response = ToDict("msg", "Session playback stopped");
                _responseWriter.SendJsonResponse(response, context);
            }
            catch (Exception e)
            {
                _responseWriter.SendError(HttpStatusCode.InternalServerError,
                    "Could not stop playback; cause: " + e.Message, context);
            }

            return context;
        }

        private IHttpContext HandlePauseSession(IHttpContext context)
        {
            try
            {
                OnPausePlaybackCalled?.Invoke(this, EventArgs.Empty);
                context.Response.StatusCode = HttpStatusCode.Accepted;
                var response = ToDict("msg", "Session playback paused");
                _responseWriter.SendJsonResponse(response, context);
            }
            catch (Exception e)
            {
                _responseWriter.SendError(HttpStatusCode.InternalServerError,
                    "Could not pause playback; cause: " + e.Message, context);
            }

            return context;
        }

        private IHttpContext HandleJumpSession(IHttpContext context)
        {
            try
            {
                var jumpPointParameter = context.Request.QueryString["milliseconds"] ?? string.Empty;
                if (uint.TryParse(jumpPointParameter, out var millis))
                {
                    OnJumpPlaybackCalled?.Invoke(this, millis);
                    context.Response.StatusCode = HttpStatusCode.Accepted;
                    var response = ToDict("msg", "Session playback jumped to " + millis);
                    _responseWriter.SendJsonResponse(response, context);
                }
                else
                {
                    _responseWriter.SendError(HttpStatusCode.BadRequest, "Parameter 'milliseconds' is not uint",
                        context);
                }
            }
            catch (Exception e)
            {
                _responseWriter.SendError(HttpStatusCode.InternalServerError,
                    "Could not pause playback; cause: " + e.Message, context);
            }

            return context;
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
            var source = GetParam(context, ParamSource);
            if (source == string.Empty)
                throw new InvalidArgumentException(HttpStatusCode.BadRequest, "Parameter '" + ParamSource + "' is missing");

            // validate source is on query services
            if (!_queryServices.Select(qs => qs.GetStorageName()).Contains(source))
                throw new InvalidArgumentException(HttpStatusCode.NotFound, "Source " + source + " not found");

            // validate session name parameter present
            var sessionName = GetParam(context, ParamSessionName);
            if (sessionName == string.Empty)
                throw new InvalidArgumentException(HttpStatusCode.BadRequest, "Parameter '" + ParamSessionName + "' is missing");

            // validate start time
            var startTime = GetParam(context, ParamStart, "0");
            if (!uint.TryParse(startTime, out var start))
                throw new InvalidArgumentException(HttpStatusCode.BadRequest,
                    "Parameter '" + ParamStart + "' contains no number >= 0");

            // validate end time
            var endTime = GetParam(context, ParamEnd, "-1");
            if (!int.TryParse(endTime, out var end))
                throw new InvalidArgumentException(HttpStatusCode.BadRequest,
                    "Parameter '" + ParamEnd + "' contains no number >= -1");

            var queryService = _queryServices.Where(qs => qs.GetStorageName() == source)
                .Select(qs => qs)
                .First();
            var session = queryService.GetSession(sessionName, start, end);
            return session;
        }

        private static string GetParam(IHttpContext context, string key, string defaultValue = "")
        {
            return context.Request.QueryString[key] ?? defaultValue;
        }

        private static IDictionary<string, string> ToDict(string key, object value)
        {
            return new Dictionary<string, string> {{key, value.ToString()}};
        }
    }
}