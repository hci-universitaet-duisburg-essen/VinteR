using System.Text;
using Google.Protobuf;
using Grapevine.Interfaces.Server;
using Grapevine.Shared;
using Newtonsoft.Json;

namespace VinteR.Rest
{
    public class HttpResponseWriter : IHttpResponseWriter
    {
        public IHttpContext SendProtobufMessage(IMessage message, IHttpContext context)
        {
            var bytes = message.ToByteArray();
            SendResponse(bytes, context, ContentType.GoogleProtoBuf);
            return context;
        }

        public IHttpContext SendError(HttpStatusCode statusCode, string message, IHttpContext context)
        {
            context.Response.StatusCode = statusCode;
            var response = new ErrorMessage() {Error = message};
            SendJsonResponse(response, context);
            return context;
        }

        public IHttpContext SendJsonResponse(object obj, IHttpContext context)
        {
            var bytes = Serialize(obj);
            context.Response.ContentEncoding = Encoding.UTF8;
            SendResponse(bytes, context);
            return context;
        }

        private static byte[] Serialize(object obj)
        {
            var serialized = JsonConvert.SerializeObject(obj);
            var bytes = Encoding.UTF8.GetBytes(serialized);
            return bytes;
        }

        private static void SendResponse(byte[] data, IHttpContext context,
            ContentType contentType = ContentType.JSON)
        {
            context.Response.ContentType = contentType;
            context.Response.ContentLength64 = data.Length;
            context.Response.SendResponse(data);
        }

        private class ErrorMessage
        {
            public string Error { get; set; }
        }
    }
}