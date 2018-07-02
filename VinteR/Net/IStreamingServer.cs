using System.Net;
using VinteR.Model;

namespace VinteR.Net
{
    public interface IStreamingServer : IServer
    {
        void Send(MocapFrame mocapFrame);

        void AddReceiver(IPEndPoint receiverEndPoint);

        int Port { get; }
    }
}