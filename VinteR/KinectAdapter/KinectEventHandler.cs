using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Kinect;
using VinteR.Model;
using Newtonsoft.Json;

namespace VinteR.KinectAdapter
{
    class KinectEventHandler
    {
        List<MocapFrame> frameList = new List<MocapFrame>();
        JsonSerializer serializer = new JsonSerializer();

        public void flushFrames()
        {
            // Implement the JSON Serialization of FrameList here! to file
            Debug.WriteLine(frameList);
            // Serialize
            using (StreamWriter sw = new StreamWriter(@"c:\mocapjson.txt"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, frameList);
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

                    List<VinteR.Model.Joint> jointList = new List<VinteR.Model.Joint>();
                    MocapFrame frame = new MocapFrame("Kinect_Version_1");

                    // loop through all skeltons
                    foreach (Skeleton skeleton in skeletons)
                    {
                        foreach (Microsoft.Kinect.Joint joint in skeleton.Joints)
                        {
                            // Create a Joint
                            VinteR.Model.Joint currentJointModel = new VinteR.Model.Joint (joint.Position.X, joint.Position.Y, joint.Position.Z);
                            currentJointModel.Name = joint.JointType.ToString();
                            currentJointModel.State = joint.TrackingState.ToString();


                            // Add the Joint to the List of all captured skeleton points
                            jointList.Add(currentJointModel);
                        }

                    }

                    // Create and append the frame
                    Body body = new KinectBody(jointList, Body.BodyTypeEnum.Skeleton);
                    frame.AddBody(ref body);
                    frameList.Add(frame);
                }
            }

        }

    }

   
}



