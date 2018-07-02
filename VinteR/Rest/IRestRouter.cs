using Grapevine.Server;

namespace VinteR.Rest
{
    /// <summary>
    /// A rest router is able to handle http rest requests. All registered routes
    /// must be given inside the dependency injection kernel, otherwise the
    /// rest server does not register them.
    /// </summary>
    public interface IRestRouter
    {
        /// <summary>
        /// Register all handler methods of this router to target router.
        /// </summary>
        /// <param name="router"></param>
        void Register(IRouter router);
    }
}