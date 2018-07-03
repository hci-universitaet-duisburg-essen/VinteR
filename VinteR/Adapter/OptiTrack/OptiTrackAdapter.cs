using System;
using VinteR.Model;

namespace VinteR.Adapter.OptiTrack
{
    public class OptiTrackAdapter : IInputAdapter
    {
        private readonly IOptiTrackClient _otClient;
        private OptiTrackEventHandler _listener;

        public event MocapFrameAvailableEventHandler FrameAvailable;
        public event ErrorEventHandler ErrorEvent;

        public bool Enabled => Config.Enabled;

        public string Name => Config?.Name;

        private Configuration.Adapter _config;

        public string AdapterType => HardwareSystems.OptiTrack;

        public Configuration.Adapter Config
        {
            get => _config;
            set
            {
                if (value.AdapterType.Equals(AdapterType))
                    _config = value;
                else
                    OnError(new ApplicationException("Accepting only opti track configuration"));
            }
        }

        public OptiTrackAdapter(IOptiTrackClient otClient)
        {
            this._otClient = otClient;
        }

        public void Run()
        {
            _listener = new OptiTrackEventHandler(this, _otClient);
            try
            {
                _otClient.Connect(_config.ClientIp, _config.ServerIp, _config.ConnectionType);
                _listener.TranslationUnitMultiplier = _otClient.TranslationUnitMultiplier;
                _otClient.OnFrameReady += _listener.ClientFrameReady;
            }
            catch (ApplicationException e)
            {
                _otClient.OnFrameReady -= _listener.ClientFrameReady;
                OnError(e);
            }
        }

        public void Stop()
        {
            if (_otClient.IsConnected())
                _otClient.Disconnect();
        }

        public virtual void OnFrameAvailable(MocapFrame frame)
        {
            if (FrameAvailable != null)
            {
                FrameAvailable(this, frame);
            }
        }

        public virtual void OnError(Exception e)
        {
            if (ErrorEvent != null) // Check if there are subscribers to the event
            {
                // Raise an Error Event
                ErrorEvent(this, e);
            }
        }
    }
}