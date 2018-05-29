using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VinteR.Configuration
{
    [JsonObject]
    public class Configuration
    {
        [JsonProperty("home.dir")] public string HomeDir { get; set; }

        [JsonProperty("adapters")] public Adapters Adapters { get; set; }
    }

    public class Adapters
    {
        [JsonProperty("kinect")] public Kinect Kinect { get; set; }
        [JsonProperty("leapmotion")] public LeapMotion LeapMotion { get; set; }
        [JsonProperty("optitrack")] public OptiTrack OptiTrack { get; set; }
    }

    public class Adapter
    {
        [JsonProperty("enabled")] public bool Enabled { get; set; }

        [JsonProperty("canHandleGlobalCoordinateSystem")]
        public bool CanHandleGlobalCoordinateSystem { get; set; }
    }

    public class Kinect : Adapter
    {
        [JsonProperty("colorStream.enabled")] public bool ColorStreamEnabled { get; set; }
        [JsonProperty("colorStream.flush")] public bool ColorStreamFlush { get; set; }
        [JsonProperty("depthStream.enabled")] public bool DepthStreamEnabled { get; set; }
        [JsonProperty("depthStream.flush")] public bool DepthStreamFlush { get; set; }
        [JsonProperty("skeletonStream.flush")] public bool SkeletonStreamFlush { get; set; }

    }

    public class LeapMotion : Adapter
    {
    }

    public class OptiTrack : Adapter
    {
        [JsonProperty("server.ip")] public string ServerIp { get; set; }

        [JsonProperty("client.ip")] public string ClientIp { get; set; }

        [JsonProperty("connection.type")] public string ConnectionType { get; set; }
    }
}