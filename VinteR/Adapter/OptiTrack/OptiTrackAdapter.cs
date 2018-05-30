using NatNetML;
using System;
using VinteR.Configuration;
using VinteR.Model;

namespace VinteR.Adapter.OptiTrack
{
    public class OptiTrackAdapter : IInputAdapter
    {
        private IConfigurationService _configurationService;
        private IOptiTrackClient _otClient;
        private OptiTrackEventHandler listener;

        public event MocapFrameAvailableEventHandler FrameAvailable;
        public event ErrorEventHandler ErrorEvent;

        public bool Enabled => _configurationService.GetConfiguration().Adapters.OptiTrack.Enabled;

        public OptiTrackAdapter(IConfigurationService configurationService, IOptiTrackClient otClient)
        {
            this._configurationService = configurationService;
            this._otClient = otClient;
        }

        public void Run()
        {
            listener = new OptiTrackEventHandler(this);
            _otClient.Connect();
            _otClient.OnFrameReady += listener.ClientFrameReady;
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