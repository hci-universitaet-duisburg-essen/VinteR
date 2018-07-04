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

        private string _currentSource;
        private string _currentSessionName;

        public PlaybackService(ISessionPlayer player, IQueryService[] queryServices)
        {
            _player = player;
            _queryServices = queryServices;
        }

        public Session Session => _player.Session;
        public bool IsPlaying => _player.IsPlaying;

        public Session Play(string source, string sessionName)
        {
            // if the session is already loaded return it
            if (_currentSource == source && _currentSessionName == sessionName)
            {
                // if playback has paused continue playback
                if (!IsPlaying) _player.Play();
                return _player.Session;
            }

            // get the query service
            var queryService = _queryServices
                .DefaultIfEmpty(null)
                .FirstOrDefault(qs => qs.GetStorageName() == source);

            // load the session. this takes long!
            var session = queryService?.GetSession(sessionName);

            // start playback if a session was loaded
            if (session != null)
            {
                _currentSource = source;
                _currentSessionName = sessionName;

                // stop current playback if needed
                if (IsPlaying) _player.Stop();

                _player.Session = session;
                _player.FrameAvailable += FireFrameAvailable;
                _player.Play();
            }

            return _player.Session;
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