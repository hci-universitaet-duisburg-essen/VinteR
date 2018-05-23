using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Kinect;
using Ninject;
using Ninject.Planning.Bindings;
using VinteR.Configuration;

namespace VinteR
{
    internal class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static bool keepRunning = true;

        public static void Main(string[] args)
        {
            // create and load dependency injection kernel
            var kernel = new StandardKernel(new VinterNinjectModule());

            // Create a common watch as a synchronization mechanism - hope it is threadsafe :D
            Stopwatch syncrowatch = new Stopwatch();
            syncrowatch.Start();
            //TBD
            // Create adapter and give the watch (No loose coupling here, needs to change in future) till we have a mechanism and interface defined
            Adapter.Kinect.KinectAdapter kinectAdapter = new Adapter.Kinect.KinectAdapter(syncrowatch);
            kinectAdapter.FrameAvailable += (adapter, frame) => Logger.Info("{Frame #{0} available from {1}", frame.timestamp, adapter.GetType().Name);
            Adapter.LeapMotion.LeapMotionAdapter leapMotionAdapter = new Adapter.LeapMotion.LeapMotionAdapter(syncrowatch);
            leapMotionAdapter.FrameAvailable += (adapter, frame) => Logger.Info("{Frame #{0} available from {1}", frame.timestamp, adapter.GetType().Name);

            Logger.Info("VinteR server started");

            // Event for stopping the program
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                e.Cancel = true;
                kinectAdapter.flushData("C:\\Users\\hci-one\\Documents\\Kinect\\Test\\frames.json");
                Program.keepRunning = false;
            };

            while (Program.keepRunning)
            {
                // Run the Server till we press Cancel
                
            }
            Logger.Info("Exited gracefully");
        
        }
    }
}