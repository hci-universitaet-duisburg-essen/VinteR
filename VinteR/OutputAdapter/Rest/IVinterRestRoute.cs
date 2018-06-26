using System;
using Grapevine.Interfaces.Server;
using Grapevine.Shared;

namespace VinteR.OutputAdapter.Rest
{
    public interface IVinterRestRoute
    {
        IHttpContext Handler(IHttpContext context);
        HttpMethod HttpMethod { get; }
        string PathInfo { get; }
    }
}