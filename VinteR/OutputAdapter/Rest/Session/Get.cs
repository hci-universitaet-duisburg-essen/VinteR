using Grapevine.Interfaces.Server;
using Grapevine.Shared;

namespace VinteR.OutputAdapter.Rest.Session
{
    public class Get : IVinterRestRoute
    {

        public HttpMethod HttpMethod => HttpMethod.GET;
        public string PathInfo => "/session";

        public IHttpContext Handler(IHttpContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}