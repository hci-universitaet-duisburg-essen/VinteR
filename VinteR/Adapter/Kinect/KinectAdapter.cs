using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Kinect;
using VinteR.Configuration;

namespace VinteR.Adapter.Kinect
{
    class KinectAdapter : IInputAdapter
    {
        // Logger
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        // MocapFrame Event Handling
        public event MocapFrameAvailableEventHandler FrameAvailable;

        // ColorFrame Event Handling
        public delegate void KinectColorEventHandler(KinectAdapter adapter, byte[] colorPixels);
        public event KinectColorEventHandler ColorFramAvailable;

        // DepthFrame Event Handling
        public delegate void KinectDepthEventHandler(KinectAdapter adapter, DepthImagePixel[] depthImage);
        public event KinectDepthEventHandler DepthFramAvailable;

        // Error Handling
        public event ErrorEventHandler ErrorEvent;

        public KinectSensor sensor;
        private KinectEventHandler kinectHandler;
 
        private readonly IConfigurationService _configurationService;

        public bool Enabled => _configurationService.GetConfiguration().Adapters.Kinect.Enabled;

        public KinectAdapter(IConfigurationService configurationService)
        {
            this._configurationService = configurationService;
            // Create the Kinect Handler
            this.kinectHandler = new KinectEventHandler(this);
        }

        public void Run()
        {

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Skeleton Stream - always on !
                this.sensor.SkeletonStream.Enable();
                this.sensor.SkeletonFrameReady += this.kinectHandler.SensorSkeletonFrameReady;

                // Enable Color Stream
                if (_configurationService.GetConfiguration().Adapters.Kinect.ColorStreamEnabled)
                {
                    this.sensor.ColorStream.Enable();
                    this.sensor.ColorFrameReady += this.kinectHandler.SensorColorFrameReady;
                }

                if (_configurationService.GetConfiguration().Adapters.Kinect.DepthStreamEnabled)
                {
                    this.sensor.DepthStream.Enable();
                    this.sensor.DepthFrameReady += this.kinectHandler.SensorDepthFrameReady;
                }

                // Further EventListener can be appended here, currently no support for depth frame etc. intended.

                // Start the sensor!
                try
                {
                    this.sensor.Start();

                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                OnError( new Exception("The Kinect is not ready! Please check the cables etc. and restart the system!") );
            }

        }

        public void Stop()
        {
            if (_configurationService.GetConfiguration().Adapters.Kinect.SkeletonStreamFlush)
            {
                var SkeletonLogFile = Path.Combine(_configurationService.GetConfiguration().HomeDir, "Kinect", "frames.json");
                flushMocapData(SkeletonLogFile);
            }

            if (_configurationService.GetConfiguration().Adapters.Kinect.ColorStreamEnabled && _configurationService.GetConfiguration().Adapters.Kinect.ColorStreamFlush)
            {
                var colorStreamLogFile = Path.Combine(_configurationService.GetConfiguration().HomeDir, "Kinect", "colorStream.json");
                flushColorData(colorStreamLogFile);
            }

            if (_configurationService.GetConfiguration().Adapters.Kinect.DepthStreamEnabled && _configurationService.GetConfiguration().Adapters.Kinect.DepthStreamFlush)
            {
                var depthStreamLogFile = Path.Combine(_configurationService.GetConfiguration().HomeDir, "Kinect", "depthStream.json");
                flushColorData(depthStreamLogFile);
            }
        }

       /*
       * Write all Data out using the File Based Writers
       */
        public void flushMocapData(string path)
        {
            if (this.kinectHandler != null)
            {
                // Write all Frames to the given JSON File
                this.kinectHandler.flushFrames(path);
            }  else
            {
                Logger.Debug("Could not Write Skeleton Data! this.kinectHandler is null");
            }
        }

        public void flushDepthData(string path)
        {
            if (this.kinectHandler != null)
            {
                // Write all Depth information to the given JSON File
                this.kinectHandler.flushDepth(path);
            }
            else
            {
                Logger.Debug("Could not Write Depth Data! this.kinectHandler is null");
            }
            
        }

        public void flushColorData(string path)
        {
            if (this.kinectHandler != null)
            {
                // Write all Color bytes to the given JSON File
                this.kinectHandler.flushColor(path);

            } else
            {
                Logger.Debug("Could not Write Color Data! this.kinectHandler is null");
            }
                
        }

        public virtual void OnFrameAvailable(Model.MocapFrame frame)
        {
            if (FrameAvailable != null) // Check if there are subscribers to the event
            {
                FrameAvailable(this, frame);
            }
        }

        public virtual void OnDepthFrameAvailable(DepthImagePixel[] depthImage)
        {
            if (DepthFramAvailable != null) // Check if there are subscribers to the event
            {
                DepthFramAvailable(this, depthImage);
            }
        }

        public virtual void OnColorFrameAvailable(byte[] colorPixels)
        {
            if (DepthFramAvailable != null) // Check if there are subscribers to the event
            {
                ColorFramAvailable(this, colorPixels);
            }
        }

        public virtual void OnError(Exception e)
        {
            if (ErrorEvent != null) // Check if there are subscribers to the event
            {
                // Raise an Error Event
                ErrorEvent(this, e);
            }
        }
    }
}
