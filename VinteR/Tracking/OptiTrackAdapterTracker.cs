using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NatNetML;
using VinteR.Adapter.OptiTrack;
using VinteR.Configuration;

namespace VinteR.Tracking
{
    public class OptiTrackAdapterTracker : IAdapterTracker
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static readonly object RigidBodiesLock = new object();

        private readonly IOptiTrackClient _client;

        private readonly IConfigurationService _configService;

        private readonly IDictionary<RigidBody, RigidBodyData>
            _rigidBodies = new Dictionary<RigidBody, RigidBodyData>();

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
             * Load rigid bodies if necessary. It is not sufficient to load rigid bodies only on the event
             * handler because it may not be executed.
             */
            lock (RigidBodiesLock)
            {
                if (_rigidBodies.Count == 0)
                    LoadRigidBodies();
            }

            /*
             * All adapters are tracked as rigid bodies. Try to locate the adapter
             * that has the given name specified inside motive.
             */
            RigidBodyData rigidBodyData;
            lock (RigidBodiesLock)
            {
                rigidBodyData = _rigidBodies.Where(p => p.Key.Name.Equals(name))
                    .Select(p => p.Value)
                    .DefaultIfEmpty(null)
                    .FirstOrDefault();
            }

            var result = Position.Zero;
            if (rigidBodyData != null)
            {
                result = new Position()
                {
                    Location = new Vector3(rigidBodyData.x, rigidBodyData.y, rigidBodyData.z) *
                               _client.TranslationUnitMultiplier,
                    Rotation = new Quaternion(rigidBodyData.qx, rigidBodyData.qy, rigidBodyData.qz, rigidBodyData.qw)
                };
            }

            return result;
        }

        private void HandleDataDescriptionsChanged()
        {
            lock (RigidBodiesLock)
            {
                _rigidBodies.Clear();
                LoadRigidBodies();
            }
        }

        private void LoadRigidBodies()
        {
            lock (RigidBodiesLock)
            {
                foreach (var rigidBody in _client.RigidBodies)
                {
                    _rigidBodies.Add(rigidBody, null);
                }
            }
        }

        private void HandleFrameReady(NatNetML.FrameOfMocapData mocapData)
        {
            // update last position of rigid bodies
            for (var i = 0; i < mocapData.nRigidBodies; i++)
            {
                var rigidBodyData = mocapData.RigidBodies[i];

                lock (RigidBodiesLock)
                {
                    var rigidBody = _rigidBodies.Where(p => p.Key.ID == rigidBodyData.ID)
                        .Select(p => p.Key)
                        .DefaultIfEmpty(null)
                        .FirstOrDefault();

                    if (rigidBody == null) continue;

                    _rigidBodies[rigidBody] = rigidBodyData;
                }
            }
        }
    }
}