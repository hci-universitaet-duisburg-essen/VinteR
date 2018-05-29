using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinteR.Datamerge;
using VinteR.Model;

namespace VinteR.Stream
{
    // A Stream represents some Workflow of data
    class StreamingManager
    {
        // Logger
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private DataMerger merger;

        public StreamingManager(DataMerger merger)
        {
            this.merger = merger; // Assign the merger
            this.merger.BodyMerged += processBody; // Register an event for processing the Bodies in this data flow!
        }

        public void processBody(Body body)
        {
            Logger.Debug("Got a Body to process: ", body);
        }
    }
}
