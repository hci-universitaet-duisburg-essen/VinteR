using VinteR.Model;

namespace VinteR.MainApplication
{
    public interface IPlaybackService
    {
        void Start();

        void Start(Session session);

        void Pause();

        void Stop();

        void Jump(uint millis);
    }
}