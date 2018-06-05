using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Kinect;
using Newtonsoft.Json;
using VinteR.Model;
using VinteR.Model.Kinect;

namespace VinteR.Adapter.Kinect
{
    /*
     * This class contains all EventHandler for the Sensor Data
     * Frame Creation is also handled here using the Model Definition before the Data Merger.
     */
    class KinectEventHandler
    {
        static List<MocapFrame> frameList = new List<MocapFrame>();
        JsonSerializer serializer = new JsonSerializer();
        KinectAdapter adapter;
        static List<DepthImagePixel[]> depthList = new List<DepthImagePixel[]>();
        static List<byte[]> colorPixelList = new List<byte[]>();
        // Logger
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public KinectEventHandler(KinectAdapter adapter)
        {
            this.adapter = adapter;
        }

        public void flushFrames(string path)
        {
            // Implement the JSON Serialization of FrameList here! to file
            // Debug.WriteLine(frameList);
            // Since the FrameList changes permanent - freeze for serialize
            List<MocapFrame> serializeList = new List<MocapFrame>(frameList);
            // Serialize 
            using (StreamWriter sw = new StreamWriter(path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, serializeList);
            }

        }

        public void flushDepth(string path)
        {
            // Debug.WriteLine(depthList);
            List<DepthImagePixel[]> serializeList = new List<DepthImagePixel[]>(depthList);
            using (StreamWriter sw = new StreamWriter(path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, serializeList);
            }

        }

        public void flushColor(string path)
        {
            // Debug.WriteLine(colorPixelList);
            List<byte[]> serializeList = new List<byte[]>(colorPixelList);
            using (StreamWriter sw = new StreamWriter(path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, serializeList);
            }

        }

        public void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons); // Copy skeelton data to the array

                    List<VinteR.Model.Point> jointList = new List<VinteR.Model.Point>();
                    MocapFrame frame = new MocapFrame(adapter.Config.Name, adapter.Config.AdapterType);

                    // loop through all skeltons
                    foreach (Skeleton skeleton in skeletons)
                    {
                        foreach (Microsoft.Kinect.Joint joint in skeleton.Joints)
                        {
                            // Create a Point
                            VinteR.Model.Point currentPointModel = new VinteR.Model.Point (joint.Position.X, joint.Position.Y, joint.Position.Z);
                            currentPointModel.Name = joint.JointType.ToString();
                            currentPointModel.State = joint.TrackingState.ToString();


                            // Add the Point to the List of all captured skeleton points
                            jointList.Add(currentPointModel);
                        }

                        // Create and append the frame
                        Body body = new KinectBody(jointList, Body.EBodyType.Skeleton);
                        frame.AddBody(ref body);
                    }

                    frameList.Add(frame);
                    this.adapter.OnFrameAvailable(frame); // publish MocapFrame
                }
            }

        }

        public void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            DepthImagePixel[] depthPixels = new DepthImagePixel[this.adapter.sensor.DepthStream.FramePixelDataLength];

            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    depthFrame.CopyDepthImagePixelDataTo(depthPixels);
                    depthList.Add(depthPixels);
                    this.adapter.OnDepthFrameAvailable(depthPixels);
                }
            }
        }

        public void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            byte[] colorPixels = colorPixels = new byte[this.adapter.sensor.ColorStream.FramePixelDataLength];
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    colorFrame.CopyPixelDataTo(colorPixels);
                    colorPixelList.Add(colorPixels);
                    this.adapter.OnColorFrameAvailable(colorPixels);
                }
            }
        }

    }

   
}



