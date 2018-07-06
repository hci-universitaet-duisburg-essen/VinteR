using System;
using Grapevine.Server;
using VinteR.Model;

namespace VinteR.Rest
{
    public delegate Session RecordCalledEventHandler();
    public delegate Session GetSessionEventHandler(string source, string sessionName, uint start, int end);
    public delegate Session SessionPlayEventHandler(string source, string sessionName, uint start, int end);

    /// <summary>
    /// A rest router is able to handle http rest requests. All registered routes
    /// must be given inside the dependency injection kernel, otherwise the
    /// rest server does not register them.
    /// </summary>
    public interface IRestRouter
    {
        // sessions event handler
        event RecordCalledEventHandler OnRecordSessionCalled;
        event RecordCalledEventHandler OnStopRecordCalled;

        // session event handler
        event GetSessionEventHandler OnGetSessionCalled;
        event SessionPlayEventHandler OnPlayCalled;
        event EventHandler OnPausePlaybackCalled;
        event EventHandler OnStopPlaybackCalled;
        event EventHandler<uint> OnJumpPlaybackCalled;

        /// <summary>
        /// Register all handler methods of this router to target router.
        /// </summary>
        /// <param name="router"></param>
        void Register(IRouter router);
    }
}