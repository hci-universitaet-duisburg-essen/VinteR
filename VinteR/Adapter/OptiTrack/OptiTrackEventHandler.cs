using System.Collections.Generic;
using System.Numerics;
using VinteR.Model;
using VinteR.Model.OptiTrack;


namespace VinteR.Adapter.OptiTrack
{
    public class OptiTrackEventHandler
    {
        private const string PointIdDivider = "_";
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly OptiTrackAdapter _adapter;

        public OptiTrackEventHandler(OptiTrackAdapter adapter)
        {
            this._adapter = adapter;
        }

        public float TranslationUnitMultiplier { get; set; }

        //Method to handle frame events
        public void ClientFrameReady(NatNetML.FrameOfMocapData data)
        {
            /* Write values into the handledFrame object */
            var handledFrame =
                new MocapFrame(_adapter.Config.Name, _adapter.Config.AdapterType)
                {
                    Latency = ExtractLatency(data)
                };
            ExtractMarkerSets(data, handledFrame);
            // Adding ElapsedMillis to MocapFrame

            _adapter.OnFrameAvailable(handledFrame);
            handledFrame.Bodies.Clear();
        }

        /*
         Method that is extracting the latency from FrameOfMocapData
         */
        public float ExtractLatency(NatNetML.FrameOfMocapData data)
        {
            /* So far without transmission latency
             client instance is needed, can't really find the right thing in OptiTrackClient -> NatNet though*/
            return data.TransmitTimestamp - data.CameraMidExposureTimestamp;
        }

        /*
         Method that is extracting Rigidbodies and Skeletons from FrameOfMocapData 
         */
        public void ExtractMarkerSets(NatNetML.FrameOfMocapData data, MocapFrame handledFrame)
        {
            for (var i = 0; i < data.nMarkerSets - 1; i++)
            {
                var msData = data.MarkerSets[i]; // Received rigid body descriptions
                var ms = new MarkerSet(msData.MarkerSetName);
                for (var j = 0; j < msData.nMarkers; j++)
                {
                    var markerData = msData.Markers[j];
                    var markerId = markerData.ID == -1 ? j : markerData.ID;
                    var marker = new Point(new Vector3(markerData.x, markerData.y, markerData.z) * TranslationUnitMultiplier)
                    {
                        Name = string.Join(PointIdDivider, msData.MarkerSetName, markerId)
                    };
                    ms.Markers.Add(marker);
//                    Logger.Debug("Marker in Set -- Name: {0} || Position: {1}, {2}, {3}", ms.OptiTrackId, marker.Position.X, marker.Position.Y, marker.Position.Z);
                }
                handledFrame.Bodies.Add(ms);
            }
        }
    }
}