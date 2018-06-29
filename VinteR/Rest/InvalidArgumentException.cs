using System;
using Grapevine.Shared;

namespace VinteR.Rest
{
    public class InvalidArgumentException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public InvalidArgumentException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}