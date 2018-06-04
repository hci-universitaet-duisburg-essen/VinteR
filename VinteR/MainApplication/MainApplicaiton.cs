using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ninject;
using VinteR.Adapter;
using VinteR.Datamerge;
using VinteR.Stream;

namespace VinteR.MainApplication
{

    public class MainApplicaiton : IMainApplication
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static bool keepRunning = true;

        /*
         * kernel must be an attribute to this class. I tired to reach it by using
         * Bind<IMainApplication>().To<MainApplicaiton>().WithPropertyValue("kernel", kernel);
         * But consider our load function, it doesn't work here. So I pass the variable
         * to start function, after the kernel created.
         */
        public void Start(StandardKernel kernel)
        {

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
                    Logger.Error("Adapter: {0}, has severe problems: {1}", a.GetType().Name, e.Message); MainApplicaiton.keepRunning = false;
                };
                var thread = new Thread(adapter.Run);
                thread.Start();
                Logger.Info("Adapter {0,20} started", adapter.GetType().Name);
            }

            Logger.Info("VinteR server started");

            // Event for stopping the program
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                e.Cancel = true;
                MainApplicaiton.keepRunning = false;
            };

            while (MainApplicaiton.keepRunning)
            {
                // Run the Server till we press Cancel

            }

            foreach (var adapter in adapters)
            {
                adapter.Stop();
            }

            Logger.Info("Exited gracefully");
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
