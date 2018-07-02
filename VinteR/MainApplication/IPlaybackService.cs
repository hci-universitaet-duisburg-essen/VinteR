using VinteR.Model;

namespace VinteR.MainApplication
{
    public interface IPlaybackService
    {
        void Start(Session session);

        void Stop();
    }
}