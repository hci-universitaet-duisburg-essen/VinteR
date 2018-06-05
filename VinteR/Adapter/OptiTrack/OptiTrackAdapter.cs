using NatNetML;
using System;
using System.Linq;
using VinteR.Configuration;
using VinteR.Model;

namespace VinteR.Adapter.OptiTrack
{
    public class OptiTrackAdapter : IInputAdapter
    {
        public static readonly string AdapterTypeName = "optitrack";

        private IOptiTrackClient _otClient;
        private OptiTrackEventHandler _listener;

        public event MocapFrameAvailableEventHandler FrameAvailable;
        public event ErrorEventHandler ErrorEvent;

        public bool Enabled => Config.Enabled;

        public string Name { get; set; }

        private Configuration.OptiTrack _config;

        public Configuration.Adapter Config
        {
            get => _config;
            set => _config = value as Configuration.OptiTrack ?? throw new ApplicationException("Accepting only opti track configuration");
        }

        public OptiTrackAdapter(IOptiTrackClient otClient)
        {
            this._otClient = otClient;
        }

        public void Run()
        {
            _listener = new OptiTrackEventHandler(this);
            _otClient.Connect(_config.ClientIp, _config.ServerIp, _config.ConnectionType);
            _otClient.OnFrameReady += _listener.ClientFrameReady;
        }

        public void Stop()
        {
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