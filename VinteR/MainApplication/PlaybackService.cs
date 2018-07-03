using VinteR.Model;
using VinteR.Streaming;

namespace VinteR.MainApplication
{
    public class PlaybackService : IPlaybackService
    {
        private readonly ISessionPlayer _player;

        public PlaybackService(ISessionPlayer player)
        {
            _player = player;
        }

        public void Start()
        {
        }

        public void Start(Session session)
        {
            _player.Session = session;
            _player.Play();
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
    }
}