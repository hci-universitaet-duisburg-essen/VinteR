using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Ninject;
using VinteR.Adapter;
using VinteR.Configuration;
using VinteR.Datamerge;

namespace VinteR
{
    internal class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static bool keepRunning = true;

        private static readonly object StopwatchLock = new object();

        public static void Main(string[] args)
        {
            // create and load dependency injection kernel
            var kernel = new StandardKernel(new VinterNinjectModule());
            var configService = kernel.Get<IConfigurationService>();
            var adapters = new List<IInputAdapter>();

            // for each json object inside inside the adapters array inside the config
            foreach (var adapterItem in configService.GetConfiguration().Adapters)
            {
                if (!adapterItem.Enabled) continue;

                /* create an input adapter based on the adapter type given
                 * Example: "adaptertype": "kinect" -> KinectAdapter
                 * See VinterDependencyModule for named bindings
                 */
                var inputAdapter = kernel.Get<IInputAdapter>(adapterItem.AdapterType);

                // set the specific config into the adapter
                inputAdapter.Config = adapterItem;

                // add the adapter to the list that will be run
                adapters.Add(inputAdapter);
            }

            /* contains the stopwatch for the program that will be used to
             * add elapsed millis inside mocap frames
             */
            var applicationWatch = Stopwatch.StartNew();
            foreach (var adapter in adapters)
            {
                // Add delegate to frame available event
                adapter.FrameAvailable += (source, frame) =>
                {
                    /* frame available occurs inside adapter thread
                     * so synchronize access to the stopwatch
                     */
                    lock (StopwatchLock)
                    {
                        frame.ElapsedMillis = applicationWatch.ElapsedMilliseconds;
                    }

                    /* get a data merger specific to the type of input adapter,
                     * so only a optitrack merger gets frames from an optitrack
                     * input adapter and so forth.
                     */
                    var merger = kernel.Get<IDataMerger>(source.Config.AdapterType);
                    Logger.Debug("{Frame #{0} available from {1}", frame.ElapsedMillis, source.Config.AdapterType);
                    merger.HandleFrame(frame);
                };
                /* add delegate to error events. the application shuts down
                 * when a error occures from one of the adapters
                 */
                adapter.ErrorEvent += (a, e) =>
                {
                    Logger.Error("Adapter: {0}, has severe problems: {1}", a.GetType().Name, e.Message);
                    Program.keepRunning = false;
                };
                // start each adapter
                var thread = new Thread(adapter.Run);
                thread.Start();
                Logger.Info("Adapter {0,20} started", adapter.GetType().Name);
            }

            Logger.Info("VinteR server started");

            // Event for stopping the program
            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e)
            {
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