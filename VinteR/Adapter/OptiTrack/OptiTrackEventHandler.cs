using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Numerics;
using VinteR.Model;
using VinteR.Model.OptiTrack;


namespace VinteR.Adapter.OptiTrack
{
    public class OptiTrackEventHandler
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private OptiTrackAdapter adapter;

        public OptiTrackEventHandler(OptiTrackAdapter adapter)
        {
            this.adapter = adapter;
        }

        //Method to handle frame events
        public void ClientFrameReady(NatNetML.FrameOfMocapData data)
        {
            /* Write values into the handledFrame object */
            var handledFrame =
                new MocapFrame(adapter.Config.Name, adapter.Config.AdapterType)
                {
                    Latency = ExtractLatency(data)
                };
            ExtractBodies(data, handledFrame);
            // Adding ElapsedMillis to MocapFrame

            adapter.OnFrameAvailable(handledFrame);
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
        public void ExtractBodies(NatNetML.FrameOfMocapData data, MocapFrame handledFrame)
        {
            for (int j = 0; j < data.nRigidBodies; j++)
            {
                NatNetML.RigidBodyData rbData = data.RigidBodies[j]; // Received rigid body descriptions

                if (rbData.Tracked == true)
                {
                    VinteR.Model.OptiTrack.RigidBody rb = new RigidBody(rbData.ID.ToString()); // Create RB
                    rb.Position = new Vector3(rbData.x, rbData.y, rbData.z); // Position
                    rb.LocalRotation = new Quaternion(rbData.qx, rbData.qy, rbData.qz, rbData.qw); // Orientation
                    rb.Points = new List<Point>() { new Point(rb.Position) };
                    handledFrame.Bodies.Add(rb); // Add to MocapFrame list of bodies
                }
            }

            for (int j = 0; j < data.nSkeletons; j++)
            {
                NatNetML.SkeletonData sklData = data.Skeletons[j];  // Received skeleton frame data
                VinteR.Model.OptiTrack.Skeleton skl = new Skeleton(sklData.ID.ToString());

                /*  Now, for each of the skeleton segments  */
                for (int k = 0; k < sklData.nRigidBodies; k++)
                {
                    NatNetML.RigidBodyData boneData = sklData.RigidBodies[k];

                    VinteR.Model.OptiTrack.RigidBody bone = new RigidBody(boneData.ID.ToString()); // Create RB
                    bone.Position = new Vector3(boneData.x, boneData.y, boneData.z); // Position
                    skl.RigidBodies.Add(bone); // Add bone to skeleton

                }
                handledFrame.Bodies.Add(skl);
            }

            for (int j = 0; j < data.nMarkerSets - 1; j++)
            {
                NatNetML.MarkerSetData msData = data.MarkerSets[j]; // Received rigid body descriptions
                VinteR.Model.OptiTrack.MarkerSet ms = new MarkerSet(msData.MarkerSetName);
                for (int k = 0; k < msData.nMarkers; k++)
                {
                    NatNetML.Marker markerData = msData.Markers[k];

                    VinteR.Model.Point marker = new Point(markerData.x, markerData.y, markerData.z);
                    marker.Name = markerData.ID.ToString();
                    ms.Markers.Add(marker);
                    Logger.Debug("Marker in Set -- Name: {0} || Position: {1}, {2}, {3}", ms.OptiTrackId, marker.Position.X, marker.Position.Y, marker.Position.Z);
                }
                Logger.Debug("MarkerSet vorhanden");
                handledFrame.Bodies.Add(ms);
            }
        }
    }
}
