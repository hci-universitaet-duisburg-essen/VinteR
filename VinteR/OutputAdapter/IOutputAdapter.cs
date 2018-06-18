using VinteR.Model;

namespace VinteR.OutputAdapter
{
    public interface IOutputAdapter
    {

        // receive the notification from Output manager.
        void OnDataReceived(MocapFrame mocapFrame);

        void Start();
        void Stop();
    }
}
