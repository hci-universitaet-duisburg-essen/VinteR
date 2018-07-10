using System.Linq;
using VinteR.Input;
using VinteR.Model;
using VinteR.Streaming;

namespace VinteR.MainApplication
{
    public class PlaybackService : IPlaybackService
    {
        public event PlayMocapFrameEventHandler FrameAvailable;

        private readonly ISessionPlayer _player;
        private readonly IQueryService[] _queryServices;

        public Session Session => _player.Session;

        public string SessionSource { get; private set; }

        public uint Start { get; private set; }

        public int End { get; private set; }

        private string _currentSessionName;

        public PlaybackService(ISessionPlayer player, IQueryService[] queryServices)
        {
            _queryServices = queryServices;
            _player = player;
            _player.FrameAvailable += FireFrameAvailable;
        }

        public Session Play(string source, string sessionName, uint start, int end)
        {
            // if the session is already loaded return it
            if (SessionSource == source 
                && _currentSessionName == sessionName
                && Start == start
                && End == end)
            {
                // if playback has paused continue playback
                if (!IsPlaying()) _player.Play();
                return _player.Session;
            }

            Start = start;
            End = end;

            // get the query service
            var queryService = _queryServices
                .DefaultIfEmpty(null)
                .FirstOrDefault(qs => qs.GetStorageName() == source);

            // load the session. this takes long!
            var session = queryService?.GetSession(sessionName, start, end);

            // start playback if a session was loaded
            if (session != null)
            {
                SessionSource = source;
                _currentSessionName = sessionName;

                // stop current playback if needed
                if (IsPlaying()) _player.Stop();

                _player.Session = session;
                _player.Play();
            }

            return _player.Session;
        }

        public bool IsPlaying()
        {
            return _player.IsPlaying;
        }

        public bool ContainsSession(string source, string sessionName, uint start, int end)
        {
            return SessionSource == source 
                   && Session?.Name == sessionName 
                   && Start == start 
                   && End == end;
        }

        public void Pause()
        {
            _player.Pause();
        }

        public void Stop()
        {
            _player.Stop();
        }

        public void Jump(uint millis)
        {
            _player.Jump(millis);
        }

        private void FireFrameAvailable(MocapFrame frame)
        {
            FrameAvailable?.Invoke(frame);
        }
    }
}