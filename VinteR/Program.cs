using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinteR.KinectAdapter;
using System.Diagnostics;
using Microsoft.Kinect;

namespace VinteR
{
    internal class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            // Create a common watch as a synchronization mechanism - hope it is threadsafe :D
            Stopwatch syncrowatch = new Stopwatch();
            syncrowatch.Start();
            //TBD
            // Create adapter and give the watch (No loose coupling here, needs to change in future) till we have a mechanism and interface defined
            VinteR.KinectAdapter.KinectAdapter kinectAdapter = new KinectAdapter.KinectAdapter(syncrowatch);
            VinteR.LeapMotionAdapter.LeapMotionAdapter leapMotionAdapter = new LeapMotionAdapter.LeapMotionAdapter(syncrowatch);

            Logger.Info("VinteR server started");
        }
    }
}