using System.Collections.Generic;
using VinteR.Adapter.OptiTrack;
using NatNetML;

namespace VinteR.Tests.Adapter.OptiTrack
{
    public class OptiTrackMockClient : IOptiTrackClient
    {
        public event OptiTrackFrameReadyEventHandler OnFrameReady;
        public event OptiTrackDataDescriptionsChangedEventHandler OnDataDescriptionsChanged;

        public IEnumerable<MarkerSet> MarkerSets { get; }
        public IEnumerable<RigidBody> RigidBodies => _rigidBodies;
        public IEnumerable<Skeleton> Skeletons { get; }

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
            _rigidBodies = new List<RigidBody>()
            {
                new RigidBody() {Name = "kinect", ID = 1},
                new RigidBody() {Name = "leapmotion", ID = 2}
            };
            // call event handler that descriptions have changed
            OnDataDescriptionsChanged?.Invoke();

            // call frame available event handler
            OnFrameReady?.Invoke(new FrameOfMocapData()
            {
                RigidBodies = new RigidBodyData[]
                {
                    // kinect
                    MockRigidBodyData(1, 1, 1, 1),
                    // leap motion
                    MockRigidBodyData(2, 2, 2, 2)
                }
            });
        }

        public void Disconnect()
        {

        }

        private static RigidBodyData MockRigidBodyData(int id, int x = 0, int y = 0, int z = 0)
        {
            return new RigidBodyData() {ID = id, x = x, y = y, z = z};
        }
    }
}