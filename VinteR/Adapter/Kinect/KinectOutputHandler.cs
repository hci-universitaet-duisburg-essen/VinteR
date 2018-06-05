using Microsoft.Kinect;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinteR.Configuration;
using VinteR.Model;

namespace VinteR.Adapter.Kinect
{   /*
        The purpose of this class is to extract all current File operations from the KinectEventHandler 
        and adapter itself. This class is temporary till and may become part of the data output merger!
    */
    class KinectOutputHandler
    {
        // Kinect Configuration
        private readonly IConfigurationService _configurationService;
        
        // Output Configuration from conf file
        private readonly string DataDir;
        private readonly string ColorStreamPath;
        private readonly string DepthStreamPath;
        private readonly string SkeletonStreamPath;
        private Configuration.Adapter _config;

        // JSON Serializer
        JsonSerializer serializer = new JsonSerializer();

        public KinectOutputHandler(IConfigurationService configurationService, Configuration.Adapter _config) {
            this._configurationService = configurationService;
            this._config = _config;
            this.DataDir = this._configurationService.GetConfiguration().HomeDir + "\\" + this._config.DataDir;
            this.ColorStreamPath = this.DataDir + "\\" + this._config.ColorStreamFlushFile;
            this.DepthStreamPath = this.DataDir + "\\" + this._config.DepthStreamFlushFile;
            this.SkeletonStreamPath = this.DataDir + "\\" + this._config.SkeletonStreamFlushFile;
            
            // Check if the DataDirectory exists
            if (!(Directory.Exists(this.DataDir)))
            {
                // Create the DataDirectory
                Directory.CreateDirectory(this.DataDir);
            }

            // ColorStream
            if (this._config.ColorStreamEnabled && this._config.ColorStreamFlush)
            {
                if (! (File.Exists( this.ColorStreamPath )))
                {
                    // Create file
                    File.Create( this.ColorStreamPath );
                }
            }

            // DepthStream
            if (this._config.DepthStreamEnabled && this._config.DepthStreamFlush)
            {
                if (! (File.Exists( this.DepthStreamPath )))
                {
                    // Create file
                    File.Create( this.DepthStreamPath );
                }
            }


            // SkeletonStream
            if (this._config.SkeletonStreamFlush)
            {
                if (! (File.Exists( this.SkeletonStreamPath )))
                {
                    // Create file
                    File.Create( this.SkeletonStreamPath );
                }
            }
        }

        public void flushFrames(List<MocapFrame> frameList)
        {
            if (this._config.SkeletonStreamFlush)
            {
                // Since the FrameList might change - freeze for serialize
                List<MocapFrame> serializeList = new List<MocapFrame>(frameList);
                // Serialize 
                using (StreamWriter sw = new StreamWriter(this.SkeletonStreamPath))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, serializeList);
                }
            }
        }

        public void flushDepth(List<DepthImagePixel[]> depthList)
        {
            if (this._config.DepthStreamFlush)
            {
                // Debug.WriteLine(depthList);
                List<DepthImagePixel[]> serializeList = new List<DepthImagePixel[]>(depthList);
                using (StreamWriter sw = new StreamWriter(this.DepthStreamPath))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, serializeList);
                }
            }

        }

        public void flushColor(List<byte[]> colorPixelList)
        {
            if (this._config.ColorStreamFlush)
            {
                List<byte[]> serializeList = new List<byte[]>(colorPixelList);
                using (StreamWriter sw = new StreamWriter(this.ColorStreamPath))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, serializeList);
                }

            }

        }
    }
}
