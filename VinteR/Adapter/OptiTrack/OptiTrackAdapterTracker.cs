using System.Linq;
using System.Numerics;
using NatNetML;
using VinteR.Configuration;

namespace VinteR.Adapter.OptiTrack
{
    public class OptiTrackAdapterTracker : IAdapterTracker
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static readonly RigidBody EmptyRigidBody = new RigidBody();
        private static readonly RigidBodyData EmptyRigidBodyData = new RigidBodyData();

        private readonly IOptiTrackClient _client;
        private readonly string _adapterNameKinect;
        private readonly string _adapterNameLeapMotion;

        private RigidBody _kinect;
        private RigidBodyData _kinectBodyData;

        private RigidBody _leapMotion;
        private RigidBodyData _leapMotionBodyData;

        public OptiTrackAdapterTracker(IOptiTrackClient client, IConfigurationService configurationService)
        {
            this._client = client;
            this._client.OnFrameReady += HandleFrameReady;
            this._client.OnDataDescriptionsChanged += HandleDataDescriptionsChanged;

            var adapters = configurationService.GetConfiguration().Adapters;
            this._adapterNameKinect = adapters.Kinect.Name;
            this._adapterNameLeapMotion = adapters.LeapMotion.Name;
        }

        public Vector3? Locate(string name)
        {
            Logger.Debug("Locating {0}", name);
            if (!_client.IsConnected())
            {
                _client.Connect();
            }

            if (name.Equals(_adapterNameKinect) && _kinectBodyData != null)
            {
                return new Vector3(_kinectBodyData.x, _kinectBodyData.y, _kinectBodyData.z);
            }
            else if (name.Equals(_adapterNameLeapMotion) && _leapMotionBodyData != null)
            {
                return new Vector3(_leapMotionBodyData.x, _leapMotionBodyData.y, _leapMotionBodyData.z);
            }

            return null;
        }

        private void HandleDataDescriptionsChanged()
        {
            var newKinectBody = _client.RigidBodies
                .DefaultIfEmpty(EmptyRigidBody)
                .FirstOrDefault(rb => rb.Name.Equals(_adapterNameKinect));
            _kinect = newKinectBody != EmptyRigidBody
                ? newKinectBody
                : _kinect;

            var newLeapMotionBody = _client.RigidBodies
                .DefaultIfEmpty(EmptyRigidBody)
                .FirstOrDefault(rb => rb.Name.Equals(_adapterNameLeapMotion));
            _leapMotion = newLeapMotionBody != EmptyRigidBody
                ? newLeapMotionBody
                : _leapMotion;
        }

        private void HandleFrameReady(NatNetML.FrameOfMocapData mocapData)
        {
            var newKinectBodyData = mocapData.RigidBodies
                .DefaultIfEmpty(EmptyRigidBodyData)
                .FirstOrDefault(rb => rb.ID == _kinect?.ID);
            _kinectBodyData = newKinectBodyData != EmptyRigidBodyData
                ? newKinectBodyData
                : _kinectBodyData;

            var newLeapMotionBodyData = mocapData.RigidBodies
                .DefaultIfEmpty(EmptyRigidBodyData)
                .FirstOrDefault(rb => rb.ID == _leapMotion?.ID);
            _leapMotionBodyData = newLeapMotionBodyData != EmptyRigidBodyData
                ? newLeapMotionBodyData
                : _leapMotionBodyData;
        }
    }
}