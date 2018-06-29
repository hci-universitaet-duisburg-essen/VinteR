using VinteR.Model;

namespace VinteR.SessionPlayer
{
    /// <summary>
    /// A session player is able to replay a recorded session by the times
    /// given inside a mocap frame.
    /// </summary>
    public interface ISessionPlayer
    {
        /// <summary>
        /// Event handler that is called when a frame is available
        /// by its time given in <see cref="MocapFrame.ElapsedMillis"/>
        /// </summary>
        event PlayMocapFrameEventHandler FrameAvailable;


        Session Session { get; set; }

        void Play();

        void Pause();

        void Stop();

        void Jump(uint millis);
    }
}