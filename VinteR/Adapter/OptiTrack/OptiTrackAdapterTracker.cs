using System;
using System.Collections;
using System.Collections.Generic;
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

        private readonly IList<Configuration.Adapter> _adapters;
        private readonly IConfigurationService _configService;

        public OptiTrackAdapterTracker(IOptiTrackClient client, IConfigurationService configurationService)
        {
            this._client = client;
            this._client.OnFrameReady += HandleFrameReady;
            this._client.OnDataDescriptionsChanged += HandleDataDescriptionsChanged;

            this._adapters = configurationService.GetConfiguration().Adapters;
            this._configService = configurationService;
        }

        public Vector3? Locate(string name)
        {
            Logger.Debug("Locating {0}", name);
            if (!_client.IsConnected())
            {
                var adapter = _configService.GetConfiguration().Adapters
                    .Where(a => a.AdapterType.Equals(OptiTrackAdapter.AdapterTypeName) && a.IsGlobalRoot)
                    .DefaultIfEmpty(null)
                    .FirstOrDefault();
                if (adapter == null)
                    throw new ApplicationException("No optitrack config with global root given");
                _client.Connect(adapter.ClientIp, adapter.ServerIp, adapter.ConnectionType);
            }

            var firstOrDefault = _adapters
                .Where(a => a.Name.Equals(name))
                .DefaultIfEmpty(null)
                .FirstOrDefault();
            if (firstOrDefault != null)
                return getPosition(firstOrDefault.Name);

            return null;
        }

        private Vector3 getPosition(string name)
        {
            return Vector3.Zero;
        }

        private void HandleDataDescriptionsChanged()
        {

        }

        private void HandleFrameReady(NatNetML.FrameOfMocapData mocapData)
        {
        }
    }
}