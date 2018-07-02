using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using VinteR.Adapter;
using VinteR.Configuration;
using VinteR.Datamerge;
using VinteR.Model;
using VinteR.Net;
using VinteR.OutputAdapter;
using VinteR.OutputManager;

namespace VinteR.MainApplication
{
    public class RecordService : IRecordService
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly object StopwatchLock = new object();
        public bool IsRecording { get; set; }

        /// <summary>
        /// contains the stopwatch for the program that will be used to add elapsed millis inside mocap frames
        /// </summary>
        private readonly Stopwatch _applicationWatch = new Stopwatch();

        private readonly IEnumerable<IInputAdapter> _inputAdapters;
        private readonly IEnumerable<IOutputAdapter> _outputAdapters;
        private readonly IEnumerable<IDataMerger> _dataMergers;
        private readonly IOutputManager _outputManager;
        private readonly IConfigurationService _configurationService;
        private readonly ISessionNameGenerator _sessionNameGenerator;
        private readonly IStreamingServer _streamingServer;

        private IList<IInputAdapter> _runningInputAdapters;

        private Session _session;

        public RecordService(IConfigurationService configurationService,
            IEnumerable<IInputAdapter> inputAdapters,
            IEnumerable<IOutputAdapter> outputAdapters, 
            IEnumerable<IDataMerger> dataMergers,
            IStreamingServer streamingServer,
            IOutputManager outputManager,
            ISessionNameGenerator sessionNameGenerator)
        {
            _configurationService = configurationService;
            _inputAdapters = inputAdapters;
            _outputAdapters = outputAdapters;
            _dataMergers = dataMergers;
            _streamingServer = streamingServer;
            _outputManager = outputManager;
            _sessionNameGenerator = sessionNameGenerator;
        }

        public void Start()
        {
            IsRecording = true;

            // session name generator
            _session = new Session(_sessionNameGenerator.Generate());

            _runningInputAdapters = new List<IInputAdapter>();

            // Start output adapters
            foreach (var outputAdapter in _outputAdapters)
            {
                _outputManager.OutputNotification += outputAdapter.OnDataReceived;
                var t = new Thread(() => outputAdapter.Start(_session));
                t.Start();
                Logger.Info("Output adapter {0,30} started", outputAdapter.GetType().Name);
            }

            // start streaming server
            _outputManager.OutputNotification += _streamingServer.Send;
            _streamingServer.Start();

            // for each json object inside inside the adapters array inside the config
            foreach (var adapterItem in _configurationService.GetConfiguration().Adapters)
            {
                if (!adapterItem.Enabled) continue;

                /* create an input adapter based on the adapter type given
                 * Example: "adaptertype": "kinect" -> KinectAdapter
                 * See VinterDependencyModule for named bindings
                 */
                var inputAdapter = _inputAdapters.First(a => a.AdapterType == adapterItem.AdapterType);

                // set the specific config into the adapter
                inputAdapter.Config = adapterItem;

                _runningInputAdapters.Add(inputAdapter);
            }

            lock (StopwatchLock)
            {
                _applicationWatch.Start();
            }

            foreach (var adapter in _runningInputAdapters)
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
                Logger.Info("Input adapter {0,30} started", adapter.GetType().Name);
            }

            Logger.Info("Started record of session {0}", _session.Name);
        }

        private void HandleFrameAvailable(IInputAdapter source, MocapFrame frame)
        {
            /* frame available occurs inside adapter thread
             * so synchronize access to the stopwatch
             */
            lock (StopwatchLock)
            {
                frame.ElapsedMillis = _applicationWatch.ElapsedMilliseconds;
                _session.Duration = _applicationWatch.ElapsedMilliseconds;
            }

            /* get a data merger specific to the type of input adapter,
             * so only a optitrack merger gets frames from an optitrack
             * input adapter and so forth.
             */
            var merger = _dataMergers.First(m => m.MergerType == source.AdapterType);
            Logger.Debug("{Frame #{0} available from {1}", frame.ElapsedMillis, source.Config.AdapterType);
            var mergedFrame = merger.HandleFrame(frame);

            //get the output from datamerger to output manager
            _outputManager.ReadyToOutput(mergedFrame);
        }

        private void HandleErrorEvent(IInputAdapter source, Exception e)
        {
            Logger.Error("Adapter: {0}, has severe problems: {1}", source.Name, e.Message);
            Stop();

            // keep console open until key is pressed
            if (Logger.IsDebugEnabled)
                Console.ReadKey();
        }

        public void Stop()
        {
            Logger.Info("Stopping input adapters");
            foreach (var adapter in _runningInputAdapters)
            {
                adapter.FrameAvailable -= HandleFrameAvailable;
                adapter.Stop();
            }

            Logger.Info("Stopping output adapters");
            foreach (var outputAdapter in _outputAdapters)
            {
                _outputManager.OutputNotification -= outputAdapter.OnDataReceived;
                outputAdapter.Stop();
            }
            Logger.Info("Stopping streaming server");
            _outputManager.OutputNotification -= _streamingServer.Send;
            _streamingServer.Stop();

            IsRecording = false;
            Logger.Info("Record stopped");
        }
    }
}