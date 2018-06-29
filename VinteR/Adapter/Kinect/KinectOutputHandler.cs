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
        // Logger
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


        // Output Configuration from conf file
        private readonly string DataDir;
        private readonly string ColorStreamPath;
        private int colorFlushCount = 0;
        private readonly string DepthStreamPath;
        private int depthFlushCount = 0;
        private readonly string SkeletonStreamPath;
        private int skeletonFlushCount = 0;
        private Configuration.Adapter _config;
        private KinectAdapter adapter;

        // Lists
        private List<MocapFrame> frameList = new List<MocapFrame>();
        private List<DepthImagePixel[]> depthList = new List<DepthImagePixel[]>();
        private List<byte[]> colorPixelList = new List<byte[]>();


        // JSON Serializer
        JsonSerializer serializer = new JsonSerializer();

        public KinectOutputHandler(IConfigurationService configurationService, Configuration.Adapter _config, KinectAdapter adapter) {
            this._configurationService = configurationService;
            this._config = _config;
            this.adapter = adapter;
            this.DataDir = this._configurationService.GetConfiguration().HomeDir + "\\" + this._config.DataDir;
            this.ColorStreamPath = this.DataDir + "\\" + this._config.ColorStreamFlushDir;
            this.DepthStreamPath = this.DataDir + "\\" + this._config.DepthStreamFlushDir;
            this.SkeletonStreamPath = this.DataDir + "\\" + this._config.SkeletonStreamFlushDir;

            // Subscribe Frame Available Event
            this.adapter.FrameAvailable += flushFrames;
            // Subscribe to DepthAvailable Event
            this.adapter.DepthFramAvailable += flushDepth;
            // Subscribe to ColorAvailable Event
            this.adapter.ColorFramAvailable += flushColor;
            
            // Check if the DataDirectory exists
            if (!(Directory.Exists(this.DataDir)))
            {
                // Create the DataDirectory
                Directory.CreateDirectory(this.DataDir);
            }

            // ColorStream
            if (this._config.ColorStreamEnabled && this._config.ColorStreamFlush)
            {
                CreateDirectory(ColorStreamPath);
            }

            // DepthStream
            if (this._config.DepthStreamEnabled && this._config.DepthStreamFlush)
            {
                CreateDirectory(DepthStreamPath);
            }


            // SkeletonStream
            if (this._config.SkeletonStreamFlush)
            {
                CreateDirectory(SkeletonStreamPath);
            }
        }

        private static void CreateDirectory(string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                Logger.Info("Created directory {0}", directory);
            }
        }

        public void flushFrames(IInputAdapter adapter, MocapFrame frame)
        {
            if (this._config.SkeletonStreamFlush)
            {
                this.frameList.Add(frame);

                if (this.frameList.Count > this._config.SkeletonStreamFlushSize)
                {
                    // freeze to serialize
                    List<MocapFrame> serializeList = new List<MocapFrame>(frameList);
                    // Serialize 
                    string flushPath = this.SkeletonStreamPath + "\\" + this.skeletonFlushCount.ToString() + ".json";
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(flushPath))
                        using (JsonWriter writer = new JsonTextWriter(sw))
                        {
                            serializer.Serialize(writer, serializeList);
                        }
                    } catch(Exception e)
                    {
                        Logger.Error("Error occurred during flushing frames: {0}", e.ToString());
                    }
                    
                    this.skeletonFlushCount += 1;

                    // Clear the List
                    this.frameList.Clear();
                }
               
            }
        }

        public void flushDepth(KinectAdapter adapter, DepthImagePixel[] depthImage)
        {
            if (this._config.DepthStreamFlush)
            {
                this.depthList.Add(depthImage);
                if (this.depthList.Count >= this._config.DepthStreamFlushSize)
                {
                    // freeze to serialize
                    List<DepthImagePixel[]> serializeList = new List<DepthImagePixel[]>(depthList);
                    string flushPath = this.DepthStreamPath + "\\" + this.depthFlushCount.ToString() + ".json";
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(flushPath))
                        using (JsonWriter writer = new JsonTextWriter(sw))
                            {
                                serializer.Serialize(writer, serializeList);
                            }
                    } catch (Exception e)
                    {
                        Logger.Error("Error occured during flushing Depth: {0}", e.ToString());
                    }

                    this.depthFlushCount += 1;
                    // Clear the List
                    this.depthList.Clear();
                }
               
            }

        }

        public void flushColor(KinectAdapter adapter, byte[] colorPixels)
        {
            if (this._config.ColorStreamFlush)
            {
                this.colorPixelList.Add(colorPixels);
                if (this.colorPixelList.Count > this._config.ColorStreamFlushSize)
                {
                    List<byte[]> serializeList = new List<byte[]>(colorPixelList);
                    string flushPath = this.ColorStreamPath + "\\" + this.colorFlushCount.ToString() + ".json";
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(flushPath))
                        using (JsonWriter writer = new JsonTextWriter(sw))
                        {
                            serializer.Serialize(writer, serializeList);
                        }
                    } catch (Exception e)
                    {
                        Logger.Error("Error occured during flushing Color: {0}", e.ToString());
                    }
                    
                    this.colorFlushCount += 1;
                    // Clear List
                    this.colorPixelList.Clear();
                }

            }

        }
    }
}
