using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Ninject;
using VinteR.Adapter;
using VinteR.Configuration;
using VinteR.Datamerge;
using VinteR.Model;

namespace VinteR.MainApplication
{
    public class MainApplication : IMainApplication
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static readonly object StopwatchLock = new object();
        public bool IsAvailable { get; set; }

        /// <summary>
        /// contains the stopwatch for the program that will be used to add elapsed millis inside mocap frames
        /// </summary>
        private readonly Stopwatch _applicationWatch = new Stopwatch();

        private IList<IInputAdapter> _adapters;
        private StandardKernel _kernel;

        /*
         * kernel must be an attribute to this class. I tired to reach it by using
         * Bind<IMainApplication>().To<MainApplication>().WithPropertyValue("kernel", kernel);
         * But consider our load function, it doesn't work here. So I pass the variable
         * to start function, after the kernel created.
         */

        public void Start(StandardKernel kernel)
        {
            this.IsAvailable = true;
            this._kernel = kernel;
            this._adapters = new List<IInputAdapter>();
            var configService = kernel.Get<IConfigurationService>();

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
                _adapters.Add(inputAdapter);
            }

            lock (StopwatchLock)
            {
                _applicationWatch.Start();
            }

            foreach (var adapter in _adapters)
            {
                // Add delegate to frame available event
                adapter.FrameAvailable += HandleFrameAvailable;

                /* add delegate to error events. the application shuts down
                 * when a error occures from one of the adapters
                 */
                adapter.ErrorEvent += HandleErrorEvent;

                // start each adapter
                var thread = new Thread(adapter.Run);
                thread.Start();
                Logger.Info("Adapter {0,20} started", adapter.GetType().Name);
            }

            Logger.Info("VinteR server started");
        }

        public void Stop()
        {
            Logger.Info("Stopping started adapters");
            foreach (var adapter in _adapters)
            {
                adapter.FrameAvailable -= HandleFrameAvailable;
                adapter.Stop();
            }

            IsAvailable = false;
            Logger.Info("Exited gracefully");
        }

        private void HandleFrameAvailable(IInputAdapter source, MocapFrame frame)
        {
            /* frame available occurs inside adapter thread
             * so synchronize access to the stopwatch
             */
            lock (StopwatchLock)
            {
                frame.ElapsedMillis = _applicationWatch.ElapsedMilliseconds;
            }

            /* get a data merger specific to the type of input adapter,
             * so only a optitrack merger gets frames from an optitrack
             * input adapter and so forth.
             */
            var merger = _kernel.Get<IDataMerger>(source.Config.AdapterType);
            Logger.Debug("{Frame #{0} available from {1}", frame.ElapsedMillis, source.Config.AdapterType);
            merger.HandleFrame(frame);
        }

        private void HandleErrorEvent(IInputAdapter source, Exception e)
        {
            Logger.Error("Adapter: {0}, has severe problems: {1}", source.Name, e.Message);
            this.Stop();

            // keep console open until key is pressed
            if (Logger.IsDebugEnabled)
                Console.ReadKey();
        }
    }
}