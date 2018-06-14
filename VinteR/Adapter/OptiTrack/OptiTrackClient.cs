using System;
using System.Collections.Generic;
using NatNetML;

namespace VinteR.Adapter.OptiTrack
{
    public delegate void OptiTrackFrameReadyEventHandler(FrameOfMocapData mocapData);

    public delegate void OptiTrackDataDescriptionsChangedEventHandler();

    /// <summary>
    /// An opti track client can be used to get data from a nat net server instance
    /// for example motive. There are multiple events with which data can be retrieved
    /// from the server.
    /// </summary>
    public interface IOptiTrackClient
    {
        /// <summary>
        /// Triggered after a <see cref="NatNetML.FrameOfMocapData"/> is returned from the nat net server.
        /// </summary>
        event OptiTrackFrameReadyEventHandler OnFrameReady;

        /// <summary>
        /// Triggered if the metadata of tracked items was changed.
        /// </summary>
        event OptiTrackDataDescriptionsChangedEventHandler OnDataDescriptionsChanged;

        /// <summary>
        /// Contains all rigid bodies that are defined inside the nat net server.
        /// </summary>
        IEnumerable<RigidBody> RigidBodies { get; }

        /// <summary>
        /// Contains the multiplier that must be used to get the correct position
        /// from the nat net server. For example Motive delivers all data in meters
        /// and not millimeters.
        /// </summary>
        float TranslationUnitMultiplier { get; }

        /// <summary>
        /// Returns <code>true</code> if this client is connected, <code>false</code>
        /// otherwise
        /// </summary>
        /// <returns></returns>
        bool IsConnected();

        /// <summary>
        /// Connects this application to optitrack using client and server ip and the given connection type
        /// </summary>
        /// <param name="clientIp">Ip of this machine</param>
        /// <param name="serverIp">Ip of the natnet server instance</param>
        /// <param name="connectionType">use <code>unicast</code> or <code>multicast</code></param>
        /// <exception cref="ApplicationException">Thrown if the client could not connect to opti track</exception>
        void Connect(string clientIp, string serverIp, string connectionType);

        /// <summary>
        /// Disconnects from the nat net server
        /// </summary>
        void Disconnect();
    }

    public class OptiTrackClient : IOptiTrackClient
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public event OptiTrackFrameReadyEventHandler OnFrameReady;
        public event OptiTrackDataDescriptionsChangedEventHandler OnDataDescriptionsChanged;

        public IEnumerable<RigidBody> RigidBodies => _rigidBodies;
        public float TranslationUnitMultiplier { get; private set; }

        private NatNetML.NatNetClientML _natNetClient;
        private bool _isConnected;

        private readonly List<RigidBody> _rigidBodies = new List<RigidBody>();
        private readonly List<Skeleton> _skeletons = new List<Skeleton>();
        private readonly List<MarkerSet> _markerSets = new List<MarkerSet>();
        private List<NatNetML.DataDescriptor> _dataDescriptor = new List<NatNetML.DataDescriptor>();

        public OptiTrackClient()
        {
            this._natNetClient = new NatNetClientML();
            this.TranslationUnitMultiplier = 1.0f;
        }

        public void Connect(string clientIp, string serverIp, string connectionType)
        {
            /*  [NatNet] Instantiate the client object  */
            _natNetClient = new NatNetML.NatNetClientML();

            /*  [NatNet] Checking verions of the NatNet SDK library */
            var natNetVersion = _natNetClient.NatNetVersion();
            Logger.Info("NatNet SDK Version: {0}.{1}.{2}.{3}", natNetVersion[0], natNetVersion[1], natNetVersion[2],
                natNetVersion[3]);

            /*  [NatNet] Connecting to the Server    */
            Logger.Info("\nConnecting...\n\tLocal IP address: {0}\n\tServer IP Address: {1}\n\n",
                clientIp, serverIp);

            var connectParams = new NatNetClientML.ConnectParams
            {
                ConnectionType = connectionType == "unicast"
                    ? ConnectionType.Unicast
                    : ConnectionType.Multicast,
                ServerAddress = serverIp,
                LocalAddress = clientIp
            };
            _natNetClient.Connect(connectParams);

            _isConnected = FetchServerDescription();
            if (_isConnected)
            {
                _natNetClient.OnFrameReady += NatNetClientOnOnFrameReady;
            }
            else
            {

                throw new ApplicationException("Could not connect to optitrack");
            }
        }

        public void Disconnect()
        {
            if (!_isConnected) return;

            _natNetClient.OnFrameReady -= NatNetClientOnOnFrameReady;
            _natNetClient.Disconnect();
        }

        private bool FetchServerDescription()
        {
            var description = new NatNetML.ServerDescription();
            var errorCode = _natNetClient.GetServerDescription(description);

            if (errorCode == 0)
            {
                Logger.Info("Success: Connected to the optitrack server\n");
                // Tracking Tools and Motive report in meters - lets convert to millimeters
                if (description.HostApp.Contains("TrackingTools") || description.HostApp.Contains("Motive"))
                    TranslationUnitMultiplier = 1000.0f;

                PrintServerDescription(description);
                FireDataDescriptionChanged();
                return true;
            }
            else
            {
                Logger.Error("Error: Failed to connect. Check the connection settings.");
                Logger.Error("Program terminated (Enter ESC to exit)");
                return false;
            }
        }

        private static void PrintServerDescription(ServerDescription serverDescription)
        {
            Logger.Info("OptiTrack Server Info:");
            Logger.Info("\tHost: {0}", serverDescription.HostComputerName);
            Logger.Info("\tApplication Name: {0}", serverDescription.HostApp);
            Logger.Info("\tApplication Version: {0}.{1}.{2}.{3}", serverDescription.HostAppVersion[0],
                serverDescription.HostAppVersion[1], serverDescription.HostAppVersion[2],
                serverDescription.HostAppVersion[3]);
            Logger.Info("\tNatNet Version: {0}.{1}.{2}.{3}\n", serverDescription.NatNetVersion[0],
                serverDescription.NatNetVersion[1], serverDescription.NatNetVersion[2],
                serverDescription.NatNetVersion[3]);
        }

        private void NatNetClientOnOnFrameReady(FrameOfMocapData data, NatNetClientML client)
        {
            /*  Exception handler for cases where assets are added or removed.
                Data description is re-obtained in the main function so that contents
                in the frame handler is kept minimal. */
            if ((data.bTrackingModelsChanged
                 || data.nMarkerSets != _markerSets.Count
                 || data.nRigidBodies != _rigidBodies.Count
                 || data.nSkeletons != _skeletons.Count))
            {
                Logger.Debug("\n===============================================================================\n");
                Logger.Debug("Change in the list of the assets. Refetching the descriptions");

                /*  Clear out existing lists */
                _dataDescriptor.Clear();
                _markerSets.Clear();
                _rigidBodies.Clear();
                _skeletons.Clear();

                /* [NatNet] Re-fetch the updated list of descriptors  */
                FetchDataDescriptor();
                Logger.Debug("===============================================================================\n");

                FireDataDescriptionChanged();
            }
            
            FireOnFrameReady(data);
        }

        public virtual void FireOnFrameReady(FrameOfMocapData data)
        {
            if (OnFrameReady != null)
            {
                OnFrameReady(data);
            }
        }

        private void FetchDataDescriptor()
        {
            /*  [NatNet] Fetch Data Descriptions. Instantiate objects for saving data descriptions and frame data    */
            var result = _natNetClient.GetDataDescriptions(out _dataDescriptor);
            if (result)
            {
                Logger.Info("Success: Data Descriptions obtained from the server.");
                ParseDataDescriptor(_dataDescriptor);
            }
            else
            {
                Logger.Info("Error: Could not get the Data Descriptions");
            }
        }

        private void ParseDataDescriptor(IReadOnlyList<DataDescriptor> description)
        {
            //  [NatNet] Request a description of the Active Model List from the server. 
            //  This sample will list only names of the data sets, but you can access 
            var numDataSet = description.Count;
            Logger.Info("Total {0} data sets in the capture:", numDataSet);

            for (var i = 0; i < numDataSet; i++)
            {
                var dataSetType = description[i].type;
                // Parse Data Descriptions for each data sets and save them in the delcared lists and hashtables for later uses.
                switch (dataSetType)
                {
                    case ((int) NatNetML.DataDescriptorType.eMarkerSetData):
                        var ms = (NatNetML.MarkerSet) description[i];
                        Logger.Info("\tMarkerset ({0})", ms.Name);

                        // Saving Rigid Body Descriptions
                        _markerSets.Add(ms);
                        break;
                    case ((int) NatNetML.DataDescriptorType.eRigidbodyData):
                        var rb = (NatNetML.RigidBody) description[i];
                        Logger.Info("\tRigidBody ({0})", rb.Name);

                        // Saving Rigid Body Descriptions
                        _rigidBodies.Add(rb);
                        break;
                    case ((int) NatNetML.DataDescriptorType.eSkeletonData):
                        var skeleton = (NatNetML.Skeleton) description[i];
                        Logger.Info("\tSkeleton ({0}), Bones:", skeleton.Name);

                        //Saving Skeleton Descriptions
                        _skeletons.Add(skeleton);
                        break;

                    default:
                        // When a Data Set does not match any of the descriptions provided by the SDK.
                        Logger.Error("\tError: Invalid Data Set");
                        break;
                }
            }
        }

        private void FireDataDescriptionChanged()
        {
            if (OnDataDescriptionsChanged != null)
            {
                OnDataDescriptionsChanged();
            }
        }

        public bool IsConnected()
        {
            return _isConnected;
        }
    }
}