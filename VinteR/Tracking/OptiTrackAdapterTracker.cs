using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NatNetML;
using VinteR.Adapter;
using VinteR.Adapter.OptiTrack;
using VinteR.Configuration;
using VinteR.Transform;

namespace VinteR.Tracking
{
    public class OptiTrackAdapterTracker : IAdapterTracker
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IOptiTrackClient _client;

        private readonly IConfigurationService _configService;
        private readonly IDictionary<RigidBody, RigidBodyData> _rigidBodies = new Dictionary<RigidBody, RigidBodyData>();

        public OptiTrackAdapterTracker(IOptiTrackClient client, IConfigurationService configurationService)
        {
            this._client = client;
            this._client.OnFrameReady += HandleFrameReady;
            this._client.OnDataDescriptionsChanged += HandleDataDescriptionsChanged;

            this._configService = configurationService;
        }

        public Position Locate(string name)
        {
            Logger.Debug("Locating {0}", name);
            if (!_client.IsConnected())
            {
                var config = _configService.GetConfiguration().Adapters
                    .Where(a => a.AdapterType.Equals(OptiTrackAdapter.AdapterTypeName) && a.IsGlobalRoot)
                    .DefaultIfEmpty(null)
                    .FirstOrDefault();
                if (config == null)
                    throw new ApplicationException("No optitrack config with global root given");
                _client.Connect(config.ClientIp, config.ServerIp, config.ConnectionType);
            }

            /*
             * All adapters are tracked as rigid bodies. Try to locate the adapter
             * that has the given name specified inside motive.
             */
            var rigidBodyData = _rigidBodies.Where(p => p.Key.Name.Equals(name))
                .Select(p => p.Value)
                .DefaultIfEmpty(null)
                .FirstOrDefault();
            var result = Position.Zero;
            if (rigidBodyData != null)
            {
                result = new Position()
                {
                    Location = new Vector3(rigidBodyData.x, rigidBodyData.y, rigidBodyData.z),
                    Rotation = new Quaternion(rigidBodyData.qx, rigidBodyData.qy, rigidBodyData.qz, rigidBodyData.qw)
                };
            }

            return result;
        }

        private void HandleDataDescriptionsChanged()
        {
            _rigidBodies.Clear();
            foreach (var rigidBody in _client.RigidBodies)
            {
                _rigidBodies.Add(rigidBody, null);
            }
        }

        private void HandleFrameReady(NatNetML.FrameOfMocapData mocapData)
        {
            // update last position of rigid bodies
            for (var i = 0; i < mocapData.nRigidBodies; i++)
            {
                var rigidBodyData = mocapData.RigidBodies[i];
                var rigidBody = _rigidBodies.Where(p => p.Key.ID == rigidBodyData.ID)
                    .Select(p => p.Key)
                    .DefaultIfEmpty(null)
                    .FirstOrDefault();
                if (rigidBody != null)
                    _rigidBodies[rigidBody] = rigidBodyData;
            }
        }
    }
}