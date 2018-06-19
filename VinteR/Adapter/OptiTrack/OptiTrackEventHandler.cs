using System.Collections.Generic;
using System.Numerics;
using NatNetML;
using VinteR.Model;
using VinteR.Model.OptiTrack;
using Skeleton = VinteR.Model.OptiTrack.Skeleton;


namespace VinteR.Adapter.OptiTrack
{
    public class OptiTrackEventHandler
    {
        private const string PointIdDivider = "_";
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly OptiTrackAdapter _adapter;
        private readonly IOptiTrackClient _client;

        public OptiTrackEventHandler(OptiTrackAdapter adapter, IOptiTrackClient client)
        {
            _adapter = adapter;
            _client = client;
        }

        public float TranslationUnitMultiplier { get; set; }

        //Method to handle frame events
        public void ClientFrameReady(FrameOfMocapData data)
        {
            /* Write values into the handledFrame object */
            var handledFrame =
                new MocapFrame(_adapter.Config.Name, _adapter.Config.AdapterType)
                {
                    Latency = ExtractLatency(data)
                };
            ExtractBodies(data, ref handledFrame);

            _adapter.OnFrameAvailable(handledFrame);
            handledFrame.Bodies.Clear();
        }

        /*
         Method that is extracting the latency from FrameOfMocapData
         */
        public float ExtractLatency(FrameOfMocapData data)
        {
            /* So far without transmission latency
             client instance is needed, can't really find the right thing in OptiTrackClient -> NatNet though*/
            return data.TransmitTimestamp - data.CameraMidExposureTimestamp;
        }

        /*
         Method that is extracting Rigidbodies and Skeletons from FrameOfMocapData 
         */
        public void ExtractBodies(FrameOfMocapData data, ref MocapFrame handledFrame)
        {
            /* save all marker sets in local dict to add all markers to
             * their corresponding rigid bodies or skeletons
             */
            var markerSets = ExtractMarkerSets(data);
            ExtractRigidBodies(data, markerSets, ref handledFrame);
            ExtractSkeletons(data, ref handledFrame);
        }

        private void ExtractSkeletons(FrameOfMocapData data, ref MocapFrame handledFrame)
        {
            for (var i = 0; i < data.nSkeletons; i++)
            {
                var sklData = data.Skeletons[i];  // Received skeleton frame data
                var skl = new Skeleton(sklData.ID.ToString())
                {
                    BodyType = Body.EBodyType.Skeleton
                };

                /*  Now, for each of the skeleton segments  */
                for (var j = 0; j < sklData.nRigidBodies; j++)
                {
                    var boneData = sklData.RigidBodies[j];

                    var bone = new OptiTrackBody(boneData.ID.ToString())
                    {
                        Centroid = new Vector3(boneData.x, boneData.y, boneData.z) * TranslationUnitMultiplier,
                        Rotation = new Quaternion(boneData.qx, boneData.qy, boneData.qz, boneData.qw)
                    }; // Create RB
                    skl.RigidBodies.Add(bone); // Add bone to skeleton
                }
                handledFrame.Bodies.Add(skl);
            }
        }

        private void ExtractRigidBodies(FrameOfMocapData data, IDictionary<string, OptiTrackBody> markerSets, ref MocapFrame handledFrame)
        {
            for (var i = 0; i < data.nRigidBodies; i++)
            {
                var rbData = data.RigidBodies[i]; // Received rigid body descriptions

                if (!rbData.Tracked) continue;

                // add the rigid body with centroid and rotation
                var rb = new OptiTrackBody(rbData.ID.ToString())
                {
                    Centroid = new Vector3(rbData.x, rbData.y, rbData.z) * TranslationUnitMultiplier,
                    Rotation = new Quaternion(rbData.qx, rbData.qy, rbData.qz, rbData.qw),
                    BodyType = Body.EBodyType.RigidBody
                };

                // in addition add each marker as point to the body
                var name = _client.NameById(rbData.ID);
                if (markerSets.TryGetValue(name, out var ms))
                {
                    foreach (var point in ms.Points)
                    {
                        rb.Points.Add(point);
                    }
                    markerSets.Remove(name);
                }
                handledFrame.Bodies.Add(rb); // Add to MocapFrame list of bodies
            }
        }

        private IDictionary<string, OptiTrackBody> ExtractMarkerSets(FrameOfMocapData data)
        {
            var markerSets = new Dictionary<string, OptiTrackBody>();
            for (var i = 0; i < data.nMarkerSets - 1; i++)
            {
                var msData = data.MarkerSets[i]; // Received marker set descriptions
                var ms = new OptiTrackBody(msData.MarkerSetName)
                {
                    BodyType = msData.nMarkers > 1
                        ? Body.EBodyType.MarkerSet
                        : Body.EBodyType.Marker
                };
                for (var j = 0; j < msData.nMarkers; j++)
                {
                    var markerData = msData.Markers[j];
                    var markerId = markerData.ID == -1 ? j : markerData.ID;
                    var marker = new Point(new Vector3(markerData.x, markerData.y, markerData.z) * TranslationUnitMultiplier)
                    {
                        Name = string.Join(PointIdDivider, msData.MarkerSetName, markerId),
                    };
                    ms.Points.Add(marker);
                }
                markerSets.Add(msData.MarkerSetName, ms);
            }

            return markerSets;
        }
    }
}