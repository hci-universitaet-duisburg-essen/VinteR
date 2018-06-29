using Google.Protobuf;
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
        /// Sends given protobuf message using http content type application/vnd.google.protobuf
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        IHttpContext SendProtobufMessage(IMessage message, IHttpContext context);

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