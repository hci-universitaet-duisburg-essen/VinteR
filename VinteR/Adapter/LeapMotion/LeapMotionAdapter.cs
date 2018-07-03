using System;
using Leap;

namespace VinteR.Adapter.LeapMotion
{
    public class LeapMotionAdapter : IInputAdapter
    {
        public event MocapFrameAvailableEventHandler FrameAvailable;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        private Controller controller;
        private LeapMotionEventHandler listener;

        // Error Handling
        public event ErrorEventHandler ErrorEvent;

        public bool Enabled => Config.Enabled;

        public string Name => Config?.Name;

        public string AdapterType => HardwareSystems.LeapMotion;

        public Configuration.Adapter Config { get; set; }       
        
        /**
         * Destructor
         */
        ~LeapMotionAdapter()
        {
            // controller.RemoveListener(listener);
            controller?.Dispose();
            Logger.Info("Destructor Leap Motion Adapter finished");
        }

        public void Run()
        {
            controller = new Controller();
            listener = new LeapMotionEventHandler(this);
            controller.Connect += listener.OnServiceConnect;
            controller.Device += listener.OnConnect;
            controller.DeviceLost += listener.OnDisconnect;
            controller.FrameReady += listener.OnFrame;

            Logger.Info("Init Leap Motion Adapter complete");
        }

        public void Stop()
        {
        }

        public virtual void OnFrameAvailable(Model.MocapFrame frame)
        {
            if (FrameAvailable != null) // Check if there are subscribers to the event
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
