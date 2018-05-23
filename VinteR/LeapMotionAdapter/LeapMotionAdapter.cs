using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;
using System.Diagnostics;

namespace VinteR.LeapMotionAdapter
{
    class LeapMotionAdapter
    {
        private Controller controller;
        private LeapMotionEventHandler listener;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /**
         * Init Leap Motion Listener and add subscriber methods for controller events
         */
        public LeapMotionAdapter(Stopwatch syncrowatch)
        {
            controller = new Controller();
            listener = new LeapMotionEventHandler(syncrowatch);
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
    }
}
