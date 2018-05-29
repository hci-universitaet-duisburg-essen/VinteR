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
        List<MocapFrame> frameList = new List<MocapFrame>();
        JsonSerializer serializer = new JsonSerializer();
        Stopwatch syncroWatch;
        KinectAdapter adapter;
        
        public KinectEventHandler(Stopwatch syncroWatch, KinectAdapter adapter)
        {
            this.syncroWatch = syncroWatch;
            this.adapter = adapter;
        }

        public void flushFrames(string path)
        {
            // Implement the JSON Serialization of FrameList here! to file
            Debug.WriteLine(frameList);
            // Since the FrameList changes permanent - freeze for serialize
            List<MocapFrame> serializeList = new List<MocapFrame>(frameList);
            // Serialize 
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
                    MocapFrame frame = new MocapFrame("Kinect_Version_1");

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

                    // Attach the timestamp to the motion frame
                    frame.timestamp = this.syncroWatch.Elapsed.ToString();
                    Debug.WriteLine(frame.ToString());
                    frameList.Add(frame);
                    adapter.OnFrameAvailable(frame); // publish MocapFrame
                }
            }

        }

    }

   
}



