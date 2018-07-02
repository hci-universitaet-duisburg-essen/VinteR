using VinteR.Model;

namespace VinteR.MainApplication
{
    public interface IMainApplication
    {
        void Start();

        void StartRecord();

        void StartPlayback(Session session);

        void Stop();
    }
}
