using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;
using System.Diagnostics;

namespace VinteR.LeapMotionAdapter
{
    class LeapMotionEventHandler
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private Stopwatch syncroWatch;

        public LeapMotionEventHandler(Stopwatch syncroWatch)
        {
            this.syncroWatch = syncroWatch;
        }

        public void OnServiceConnect(object sender, ConnectionEventArgs args)
        {
            Logger.Info("Leap Motion Listener Service Connected");
        }

        public void OnConnect(object sender, DeviceEventArgs args)
        {
            Logger.Info("Leap Motion Connected");
        }

        public void OnDisconnect(object sender, DeviceEventArgs args)
        {
            Logger.Info("Leap Motion Disconnected");
        }

        /**
         * Method to handle frame events
         */
        public void OnFrame(object sender, FrameEventArgs args)
        {
            Logger.Info("Leap Motion Frame Available:");

            // Get the most recent frame and report some basic information
            Frame frame = args.frame;

            Logger.Info("Frame id: {0}, hands: {1}", frame.Id, frame.Hands.Count);

            foreach (Hand hand in frame.Hands)
            {
                Logger.Info("Hand id: {0}, palm position: {1}, fingers: {2}", hand.Id, hand.PalmPosition, hand.Fingers.Count);
                // Get the hand's normal vector and direction
                Vector normal = hand.PalmNormal;
                Vector direction = hand.Direction;

                float pitch = direction.Pitch * 180.0f / (float)Math.PI;
                float roll = normal.Roll * 180.0f / (float)Math.PI;
                float yaw = direction.Yaw * 180.0f / (float)Math.PI;

                // Calculate the hand's pitch, roll, and yaw angles
                Logger.Info("Hand pitch: {0} degrees, roll: {1} degrees, yaw: {2} degrees", pitch, roll, yaw);

                // Get the Arm bone
                Arm arm = hand.Arm;
                Logger.Info("Arm direction: {0}, wrist position: {1}, elbow position: {2}", arm.Direction, arm.WristPosition, arm.ElbowPosition);

                // Get fingers
                foreach (Finger finger in hand.Fingers)
                {
                    Logger.Info(
                      "Finger {0}: length: {1}mm, width: {2}mm, fingertipPosition: {3}", finger.Type.ToString(), finger.Length, finger.Width, finger.TipPosition);

                    // Get finger bones
                    Bone bone;
                    for (int b = 0; b < 4; b++)
                    {
                        bone = finger.Bone((Bone.BoneType) b);
                        Logger.Info("Bone: {0}, start: {1}, end: {2}, direction: {3}", bone.Type, bone.PrevJoint, bone.NextJoint, bone.Direction);
                    }
                }
            }

            Logger.Info(this.syncroWatch.Elapsed.ToString());
        }
    }
}
