using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NatNetML;
using VinteR.Configuration;
using VinteR.Transform;

namespace VinteR.Adapter.OptiTrack
{
    public class OptiTrackAdapterTracker : IAdapterTracker
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IOptiTrackClient _client;

        private readonly IList<Configuration.Adapter> _adapters;
        private readonly IConfigurationService _configService;
        private readonly IDictionary<MarkerSet, MarkerSetData> _markerSets = new Dictionary<MarkerSet, MarkerSetData>();
        private readonly ITransformator transformator;

        public OptiTrackAdapterTracker(IOptiTrackClient client, IConfigurationService configurationService, ITransformator transformator)
        {
            this._client = client;
            this._client.OnFrameReady += HandleFrameReady;
            this._client.OnDataDescriptionsChanged += HandleDataDescriptionsChanged;

            this._adapters = configurationService.GetConfiguration().Adapters;
            this._configService = configurationService;

            this.transformator = transformator;
        }

        public Vector3? Locate(string name)
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
            var markerSetData = _markerSets.Where(p => p.Key.Name.Equals(name))
                .Select(p => p.Value)
                .DefaultIfEmpty(null)
                .FirstOrDefault();
            return markerSetData != null
                ? transformator.GetCentroid(ToVector3(markerSetData))
                : Vector3.Zero;
        }

        private static IEnumerable<Vector3> ToVector3(MarkerSetData markerSet)
        {
            var vectors = new Vector3[markerSet.nMarkers];
            for (var i = 0; i < markerSet.nMarkers; i++)
            {
                var marker = markerSet.Markers[i];
                vectors[i] = new Vector3(marker.x, marker.y, marker.z);
            }

            return vectors;
        }

        private void HandleDataDescriptionsChanged()
        {
            _markerSets.Clear();
            foreach (var markerSet in _client.MarkerSets)
            {
                _markerSets.Add(markerSet, null);
            }
        }

        private void HandleFrameReady(NatNetML.FrameOfMocapData mocapData)
        {
            // update last position of rigid bodies
            for (var i = 0; i < mocapData.nMarkerSets; i++)
            {
                var markerSetData = mocapData.MarkerSets[i];
                var markerSet = _markerSets.Where(p => p.Key.Name.Equals(markerSetData.MarkerSetName))
                    .Select(p => p.Key)
                    .DefaultIfEmpty(null)
                    .FirstOrDefault();
                if (markerSet != null)
                    _markerSets[markerSet] = markerSetData;
            }
        }
    }
}