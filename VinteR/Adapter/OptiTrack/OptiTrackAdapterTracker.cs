using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NatNetML;
using VinteR.Configuration;

namespace VinteR.Adapter.OptiTrack
{
    public class OptiTrackAdapterTracker : IAdapterTracker
    {
        private static readonly RigidBody EMPTY_RIGID_BODY = new RigidBody();
        private static readonly RigidBodyData EMPTY_RIGID_BODY_DATA = new RigidBodyData();

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

        public Vector3 Locate(string name)
        {
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

            return Vector3.Zero;
        }

        private void HandleDataDescriptionsChanged()
        {
            var newKinectBody = _client.RigidBodies
                .DefaultIfEmpty(EMPTY_RIGID_BODY)
                .First(rb => rb.Name.Equals(_adapterNameKinect));
            _kinect = newKinectBody != EMPTY_RIGID_BODY
                ? newKinectBody
                : _kinect;

            var newLeapMotionBody = _client.RigidBodies
                .DefaultIfEmpty(EMPTY_RIGID_BODY)
                .First(rb => rb.Name.Equals(_adapterNameLeapMotion));
            _leapMotion = newLeapMotionBody != EMPTY_RIGID_BODY
                ? newLeapMotionBody
                : _leapMotion;
        }

        private void HandleFrameReady(NatNetML.FrameOfMocapData mocapData)
        {
            var newKinectBodyData = mocapData.RigidBodies
                .DefaultIfEmpty(EMPTY_RIGID_BODY_DATA)
                .First(rb => rb.ID == _kinect?.ID);
            _kinectBodyData = newKinectBodyData != EMPTY_RIGID_BODY_DATA 
                ? newKinectBodyData 
                : _kinectBodyData;

            var newLeapMotionBodyData = mocapData.RigidBodies
                .DefaultIfEmpty(EMPTY_RIGID_BODY_DATA)
                .First(rb => rb.ID == _leapMotion?.ID);
            _leapMotionBodyData = newLeapMotionBodyData != EMPTY_RIGID_BODY_DATA
                ? newKinectBodyData
                : _kinectBodyData;
        }
    }
}