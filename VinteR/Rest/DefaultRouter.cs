using System;
using System.Linq;
using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Shared;

namespace VinteR.Rest
{
    public class DefaultRouter : IRestRouter
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public event RecordCalledEventHandler OnRecordSessionCalled;
        public event RecordCalledEventHandler OnStopRecordCalled;
        public event SessionPlayEventHandler OnPlayCalled;
        public event EventHandler OnPausePlaybackCalled;
        public event EventHandler OnStopPlaybackCalled;
        public event EventHandler<uint> OnJumpPlaybackCalled;

        public void Register(IRouter router)
        {
            router.Register(HandleAll, HttpMethod.ALL);
        }

        private static IHttpContext HandleAll(IHttpContext context)
        {
            Logger.Info("Handling {0,5} on {1}", context.Request.HttpMethod, context.Request.PathInfo);

            if (!Logger.IsDebugEnabled) return context;
            if (!context.Request.QueryString.HasKeys()) return context;

            var maxKeyLength = context.Request.QueryString.AllKeys.Select(k => k.Length).Max();
            var msg = "{0," + maxKeyLength + "}: {1}";
            foreach (string key in context.Request.QueryString)
            {
                Logger.Debug(msg, key, context.Request.QueryString[key]);
            }
            return context;
        }
    }
}