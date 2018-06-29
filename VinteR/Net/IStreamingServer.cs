using VinteR.Model;

namespace VinteR.Net
{
    public interface IStreamingServer : IServer
    {
        void Send(MocapFrame mocapFrame);
    }
}