using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VinteR.Adapter.OptiTrack;
using NatNetML;
using Ninject.Infrastructure.Language;

namespace VinteR.Tests.Adapter.OptiTrack
{
    public class OptiTrackMockClient : IOptiTrackClient
    {
        public event OptiTrackFrameReadyEventHandler OnFrameReady;
        public event OptiTrackDataDescriptionsChangedEventHandler OnDataDescriptionsChanged;

        public IEnumerable<MarkerSet> MarkerSets => _markerSets;
        public IEnumerable<RigidBody> RigidBodies => _rigidBodies;
        public IEnumerable<Skeleton> Skeletons { get; }

        private IList<MarkerSet> _markerSets;
        private IList<RigidBody> _rigidBodies;

        private bool _isConnected;

        public bool IsConnected()
        {
            return _isConnected;
        }

        public void Connect(string clientIp, string serverIp, string connectionType)
        {
            _isConnected = true;

            // create mock objects for all input adaters
            _markerSets = new List<MarkerSet>()
            {
                new MarkerSet() {Name = "kinect"},
                new MarkerSet() {Name = "leapmotion"}
            };
            _rigidBodies = new List<RigidBody>()
            {
                new RigidBody() {Name = "kinect", ID = 1},
                new RigidBody() {Name = "leapmotion", ID = 2}
            };
            // call event handler that descriptions have changed
            OnDataDescriptionsChanged?.Invoke();

            var leapMotionRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, 30f.ToRadians());
            // call frame available event handler
            OnFrameReady?.Invoke(new FrameOfMocapData()
            {
                MarkerSets = new MarkerSetData[]
                {
                    // kinect
                    MockMarkerSetData("kinect", new Vector3[]
                    {
                        Vector3.Zero, new Vector3(3, 0, 3), new Vector3(0, 0, 3)
                    }),
                    // leap motion
                    MockMarkerSetData("leapmotion", new Vector3[]
                    {
                        Vector3.Zero, new Vector3(6, 6, 3), new Vector3(6, 0, 3)
                    })
                },
                nMarkerSets = 2,
                RigidBodies = new RigidBodyData[]
                {
                    new RigidBodyData() {ID = 1, x = 1, y = 0, z = 2, qx = 0, qy = 0, qz = 0, qw = 1},
                    new RigidBodyData()
                    {
                        ID = 2,
                        x = 4,
                        y = 2,
                        z = 2,
                        qx = leapMotionRotation.X,
                        qy = leapMotionRotation.Y,
                        qz = leapMotionRotation.Z,
                        qw = leapMotionRotation.W
                    }
                },
                nRigidBodies = 2
            });
        }

        public void Disconnect()
        {
        }

        private static MarkerSetData MockMarkerSetData(string name, IReadOnlyList<Vector3> points)
        {
            var result = new MarkerSetData()
            {
                MarkerSetName = name,
                nMarkers = points.Count()
            };
            var markers = new Marker[points.Count];
            for (var i = 0; i < markers.Length; i++)
            {
                var vector3 = points[i];
                markers[i] = MockMarker(vector3.X, vector3.Y, vector3.Z);
            }

            result.Markers = markers;
            return result;
        }

        private static Marker MockMarker(float x = 0, float y = 0, float z = 0)
        {
            return new Marker() {x = x, y = y, z = z};
        }
    }
}