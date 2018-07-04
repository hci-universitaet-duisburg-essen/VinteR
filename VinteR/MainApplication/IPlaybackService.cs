using VinteR.Model;
using VinteR.Streaming;

namespace VinteR.MainApplication
{
    public interface IPlaybackService
    {
        event PlayMocapFrameEventHandler FrameAvailable;

        bool IsPlaying { get; }

        Session Play(string source, string sessionName);

        void Pause();

        void Stop();

        void Jump(uint millis);
    }
}