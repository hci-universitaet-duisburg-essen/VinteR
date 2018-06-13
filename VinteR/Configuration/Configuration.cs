using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VinteR.Configuration
{
    [JsonObject]
    public class Configuration
    {
        [JsonProperty("home.dir")] public string HomeDir { get; set; }
        [JsonProperty("port")] public int Port { get; set; }

        [JsonProperty("udp.receivers")] public IList<UdpReceiver> UdpReceivers { get; set; }

        [JsonProperty("adapters")] public IList<Adapter> Adapters { get; set; }
    }

    public class UdpReceiver
    {
        [JsonProperty("ip")] public string Ip { get; set; }
        [JsonProperty("port")] public int Port { get; set; }
    }

    public class Adapter
    {
        [JsonProperty("enabled")] public bool Enabled { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("adaptertype")] public string AdapterType { get; set; }
        [JsonExtensionData] private readonly IDictionary<string, JToken> _additionalSettings;

        public IDictionary<string, JToken> AdditionalSettings => _additionalSettings;

        // kinect props
        public string DataDir { get; set; }
        public bool ColorStreamEnabled { get; set; }
        public bool ColorStreamFlush { get; set; }
        public bool DepthStreamEnabled { get; set; }
        public bool DepthStreamFlush { get; set; }
        public bool SkeletonStreamFlush { get; set; }
        public string ColorStreamFlushFile { get; set; }
        public string DepthStreamFlushFile { get; set; }
        public string SkeletonStreamFlushFile { get; set; }

        // optitrack props
        public string ServerIp { get; set; }
        public string ClientIp { get; set; }
        public string ConnectionType { get; set; }

        public Adapter()
        {
            _additionalSettings = new Dictionary<string, JToken>();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            var setting = _additionalSettings.GetValueOrDefault("colorStream.enabled", null);
            ColorStreamEnabled = setting != null && bool.TrueString.Equals((string) setting);
            setting = _additionalSettings.GetValueOrDefault("colorStream.enabled", null);
            ColorStreamEnabled = setting != null && bool.TrueString.Equals((string) setting);
            setting = _additionalSettings.GetValueOrDefault("colorStream.flush", null);
            ColorStreamFlush = setting != null && bool.TrueString.Equals((string) setting);
            setting = _additionalSettings.GetValueOrDefault("depthStream.enabled", null);
            DepthStreamEnabled = setting != null && bool.TrueString.Equals((string) setting);
            setting = _additionalSettings.GetValueOrDefault("depthStream.flush", null);
            DepthStreamFlush = setting != null && bool.TrueString.Equals((string) setting);
            setting = _additionalSettings.GetValueOrDefault("skeletonStream.flush", null);
            SkeletonStreamFlush = setting != null && bool.TrueString.Equals((string) setting);
            setting = _additionalSettings.GetValueOrDefault("data.dir", "KinectData");
            DataDir = (string) setting;
            setting = _additionalSettings.GetValueOrDefault("colorStream.flush.filename", "ColorStreamData");
            ColorStreamFlushFile = (string) setting;
            setting = _additionalSettings.GetValueOrDefault("depthStream.flush.filename", "DepthStreamData");
            DepthStreamFlushFile = (string) setting;
            setting = _additionalSettings.GetValueOrDefault("skeletonStream.flush.filename", "SkeletonStreamData");
            SkeletonStreamFlushFile = (string) setting;

            setting = _additionalSettings.GetValueOrDefault("server.ip", null);
            ServerIp = setting != null ? (string) setting : null;
            setting = _additionalSettings.GetValueOrDefault("client.ip", null);
            ClientIp = setting != null ? (string) setting : null;
            setting = _additionalSettings.GetValueOrDefault("connection.type", null);
            ConnectionType = setting != null ? (string) setting : null;
        }
    }

    public static class DictionaryExtension
    {
        public static TV GetValueOrDefault<TK, TV>(this IDictionary<TK, TV> @this, TK key, TV @default)
        {
            return @this.ContainsKey(key) ? @this[key] : @default;
        }
    }
}