using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Kinect;

namespace VinteR.Adapter.Kinect
{
    class KinectAdapter : IInputAdapter
    {
        public event MocapFrameAvailableEventHandler FrameAvailable;

        private KinectSensor sensor;
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

                // Update the SensorData - register EventHandler
                this.sensor.SkeletonFrameReady += this.kinectHandler.SensorSkeletonFrameReady;

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
        public void flushData(string path)
        {
            // Write all Frames to the given JSON File
            this.kinectHandler.flushFrames(path);
        }

        public virtual void OnFrameAvailable(Model.MocapFrame frame)
        {
            if (FrameAvailable != null) // Check if there are subscribers to the event
            {
                FrameAvailable(this, frame);
            }
        }
    }
}
