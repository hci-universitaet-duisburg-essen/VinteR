using System;
using VinteR.Model;

namespace VinteR.Rest
{
    public interface IRestServer
    {
        void Start();

        void Stop();
    }
}