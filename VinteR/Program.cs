using System;
using System.Linq;
using System.Threading;
using Ninject;
using VinteR.Adapter;
using VinteR.Datamerge;
using VinteR.Stream;

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
            var merger = kernel.Get<DataMerger>();
            var processStream = new StreamingManager(kernel.Get<DataMerger>());


            foreach (var adapter in adapters)
            {
                adapter.FrameAvailable += (a, f) =>
                {
                    Logger.Info("{Frame #{0} available from {1}", f.timestamp, a.GetType().Name);
                    merger.HandleFrame(f);
                };
                adapter.ErrorEvent += (a, e) => {
                    Logger.Error("Adapter: {0}, has severe problems: {1}", a.GetType().Name, e.Message); Program.keepRunning = false;
                };
                var thread = new Thread(adapter.Run);
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