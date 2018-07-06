using VinteR.Model;
using VinteR.Streaming;

namespace VinteR.MainApplication
{
    public interface IPlaybackService
    {
        event PlayMocapFrameEventHandler FrameAvailable;

        Session Session { get; }

        Session Play(string source, string sessionName, uint start, int end);

        bool IsPlaying();

        bool ContainsSession(string source, string sessionName, uint start, int end);

        void Pause();

        void Stop();

        void Jump(uint millis);
    }
}