using System.Text;
using Grapevine.Interfaces.Server;
using Grapevine.Shared;

namespace VinteR.OutputAdapter.Rest.Sessions
{
    public class Get : IVinterRestRoute
    {
        public HttpMethod HttpMethod => HttpMethod.GET;
        public string PathInfo => "/sessions";

        public IHttpContext Handler(IHttpContext context)
        {
            var data = Encoding.UTF8.GetBytes("{\"some\": 123}");
            context.Response.ContentType = ContentType.JSON;
            context.Response.ContentLength64 = data.Length;
            context.Response.SendResponse(data);
            return context;
        }
    }
}