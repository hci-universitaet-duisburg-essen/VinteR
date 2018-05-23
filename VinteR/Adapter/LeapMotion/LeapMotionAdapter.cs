using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;
using System.Diagnostics;

namespace VinteR.Adapter.LeapMotion
{
    class LeapMotionAdapter : IInputAdapter
    {
        private Controller controller;
        private LeapMotionEventHandler listener;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public event MocapFrameAvailableEventHandler FrameAvailable;

        /**
         * Init Leap Motion Listener and add subscriber methods for controller events
         */
        public LeapMotionAdapter(Stopwatch syncrowatch)
        {
            controller = new Controller();
            listener = new LeapMotionEventHandler(syncrowatch, this);
            controller.Connect += listener.OnServiceConnect;
            controller.Device += listener.OnConnect;
            controller.DeviceLost += listener.OnDisconnect;
            controller.FrameReady += listener.OnFrame;

            Logger.Info("Init Leap Motion Adapter complete");
        }

        /**
         * Destructor
         */
        ~LeapMotionAdapter()
        {
            // controller.RemoveListener(listener);
            controller.Dispose();
            Logger.Info("Destructor Leap Motion Adapter finished");
        }

        public virtual void OnFrameAvailable(Model.MocapFrame frame)
        {
            if (FrameAvailable != null) // Check if there are subscribers to the event
            {
                FrameAvailable(this, frame);
            }
        }
    }
}
