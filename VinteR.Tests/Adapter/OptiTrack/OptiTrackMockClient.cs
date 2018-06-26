using System.Collections.Generic;
using System.Numerics;
using VinteR.Adapter.OptiTrack;
using NatNetML;

namespace VinteR.Tests.Adapter.OptiTrack
{
    public class OptiTrackMockClient : IOptiTrackClient
    {
        public event OptiTrackFrameReadyEventHandler OnFrameReady;
        public event OptiTrackDataDescriptionsChangedEventHandler OnDataDescriptionsChanged;

        public IEnumerable<RigidBody> RigidBodies => _rigidBodies;
        public float TranslationUnitMultiplier => 1f;

        private IList<RigidBody> _rigidBodies;

        private bool _isConnected;

        public bool IsConnected()
        {
            return _isConnected;
        }

        public void Connect(string clientIp, string serverIp, string connectionType)
        {
            _isConnected = true;

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
                MarkerSets = new[]
                {
                    // kinect
                    Mock.MockMarkerSetData("kinect", new[]
                    {
                        Vector3.Zero, new Vector3(3, 0, 3), new Vector3(0, 0, 3)
                    }),
                    // leap motion
                    Mock.MockMarkerSetData("leapmotion", new[]
                    {
                        Vector3.Zero, new Vector3(6, 6, 3), new Vector3(6, 0, 3)
                    })
                },
                nMarkerSets = 2,
                RigidBodies = new[]
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

        public string NameById(int id)
        {
            return id == 1
                ? "kinect"
                : id == 2
                    ? "leapmotion"
                    : string.Empty;
        }
    }
}