using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Kinect;

namespace VinteR.Adapter.Kinect
{
    class KinectAdapter : IInputAdapter
    {
        // MocapFrame Event Handling
        public event MocapFrameAvailableEventHandler FrameAvailable;

        // ColorFrame Event Handling
        public delegate void KinectColorEventHandler(KinectAdapter adapter, byte[] colorPixels);
        public event KinectColorEventHandler ColorFramAvailable;

        // DepthFrame Event Handling
        public delegate void KinectDepthEventHandler(KinectAdapter adapter, DepthImagePixel[] depthImage);
        public event KinectDepthEventHandler DepthFramAvailable;

        public KinectSensor sensor;
        private KinectEventHandler kinectHandler;
        private Stopwatch syncroWatch;

        public KinectAdapter(Stopwatch synchroWatch)
        {

            // Create the Kinect Handler

            this.syncroWatch = synchroWatch;
            this.kinectHandler = new KinectEventHandler(this.syncroWatch, this);

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
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();
                this.sensor.ColorStream.Enable();
                this.sensor.DepthStream.Enable();
                // Update the SensorData - register EventHandler
                this.sensor.SkeletonFrameReady += this.kinectHandler.SensorSkeletonFrameReady;
                this.sensor.DepthFrameReady += this.kinectHandler.SensorDepthFrameReady;
                this.sensor.ColorFrameReady += this.kinectHandler.SensorColorFrameReady;

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
                throw new Exception("The Kinect is not ready! Please check the cables etc. and restart the system!");
            }

        }

       /*
       * Write all Data out using the File Based Writers
       */
        public void flushMocapData(string path)
        {
            // Write all Frames to the given JSON File
            this.kinectHandler.flushFrames(path);
        }

        public void flushDepthData(string path)
        {
            // Write all Depth information to the given JSON File
            this.kinectHandler.flushDepth(path);
        }

        public void flushColorData(string path)
        {
            // Write all Color bytes to the given JSON File
            this.kinectHandler.flushColor(path);
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
    }
}
