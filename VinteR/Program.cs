using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using Microsoft.Kinect;
using Ninject;
using Ninject.Planning.Bindings;
using VinteR.Adapter;
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

            var adapters = from adapter in kernel.GetAll<IInputAdapter>()
                    where adapter.Enabled
                    select adapter;

            // Create a common watch as a synchronization mechanism - hope it is threadsafe :D
            Stopwatch syncrowatch = new Stopwatch();
            syncrowatch.Start();

            foreach (var adapter in adapters)
            {
                adapter.FrameAvailable += (a, f) => Logger.Info("{Frame #{0} available from {1}", f.timestamp, a.GetType().Name);
                var thread = new Thread(() => adapter.Run(syncrowatch));
                thread.Start();
                Logger.Info("Adapter {0,20} started", adapter.GetType().Name);
            }

            Logger.Info("VinteR server started");

            // Event for stopping the program
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                e.Cancel = true;
                Program.keepRunning = false;
            };

            while (Program.keepRunning)
            {
                // Run the Server till we press Cancel
                
            }

            foreach (var adapter in adapters)
            {
                adapter.Stop();
            }

            Logger.Info("Exited gracefully");
        
        }
    }
}