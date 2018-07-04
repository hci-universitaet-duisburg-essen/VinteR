using VinteR.Model;
using VinteR.Streaming;

namespace VinteR.MainApplication
{
    public interface IRecordService
    {
        event PlayMocapFrameEventHandler FrameAvailable;

        Session Session { get; }

        Session Start();

        void Stop();
    }
}