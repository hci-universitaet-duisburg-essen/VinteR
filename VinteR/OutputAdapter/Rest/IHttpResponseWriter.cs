using Grapevine.Interfaces.Server;
using Grapevine.Shared;

namespace VinteR.OutputAdapter.Rest
{
    /// <summary>
    /// An <code>IHttpResponseWriter</code> is able to send data
    /// to http context objects.
    /// See https://sukona.github.io/Grapevine/en/getting-started.html" for more info.
    /// </summary>
    public interface IHttpResponseWriter
    {
        /// <summary>
        /// Serializes given object into a json object that is returned to the client.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="context"></param>
        /// <returns>The context from the request</returns>
        IHttpContext SendJsonResponse(object obj, IHttpContext context);

        /// <summary>
        /// Sends given protobuf session using http content type application/vnd.google.protobuf
        /// </summary>
        /// <param name="session"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        IHttpContext SendSession(Model.Gen.Session session, IHttpContext context);

        /// <summary>
        /// Sends an json object with given message as value and given status code
        /// inside the http header.
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        IHttpContext SendError(HttpStatusCode statusCode, string message, IHttpContext context);
    }
}