using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Leap;

namespace VinteR.Adapter.LeapMotion
{
    class LeapMotionEventHandler
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private LeapMotionAdapter adapter;

        public LeapMotionEventHandler(LeapMotionAdapter adapter)
        {
            this.adapter = adapter;
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
            // Get the most recent frame and report some basic information
            Frame frame = args.frame;

            //Logger.Info("Leap Motion Frame Available");
            //Logger.Info("Frame id: {0}, hands: {1}", frame.Id, frame.Hands.Count);

            // Create Mocap Frame to send to server later
            VinteR.Model.MocapFrame mocapFrame = new VinteR.Model.MocapFrame("Leap_Motion");

            foreach (Leap.Hand hand in frame.Hands)
            {
                //Logger.Info("Leap Hand id: {0}, fingers: {1}", hand.Id, hand.Fingers.Count);

                // Get the Arm bone
                // Arm arm = hand.Arm;
                // Logger.Info("Arm direction: {0}, wrist position: {1}, elbow position: {2}", arm.Direction, arm.WristPosition, arm.ElbowPosition);

                // Get fingers and bones
                IList<Model.LeapMotion.Finger> fingers = new List<Model.LeapMotion.Finger>();

                foreach (Leap.Finger finger in hand.Fingers)
                {
                    Model.LeapMotion.Finger modelFinger = new Model.LeapMotion.Finger(getFingerType(finger.Type.ToString()));
                    IList<Model.LeapMotion.FingerBone> fingerBones = new List<Model.LeapMotion.FingerBone>();

                    //Logger.Info("Leap Finger {0}: length: {1}mm, width: {2}mm, fingertipPosition: {3}", finger.Type.ToString(), finger.Length, finger.Width, finger.TipPosition);
                    //Logger.Info("Model Finger {0}", getFingerType(finger.Type.ToString()));

                    // Get finger bones
                    Bone bone;
                    for (int b = 0; b < 4; b++)
                    {
                        bone = finger.Bone((Bone.BoneType) b);

                        Model.LeapMotion.FingerBone modelBone = new Model.LeapMotion.FingerBone(getFingerBoneType(bone.Type.ToString()));
                        modelBone.LocalStartPosition = new System.Numerics.Vector3(bone.PrevJoint.x, bone.PrevJoint.y, bone.PrevJoint.z);
                        modelBone.LocalEndPosition = new System.Numerics.Vector3(bone.NextJoint.x, bone.NextJoint.y, bone.NextJoint.z);

                        fingerBones.Add(modelBone);

                        //Logger.Info("Leap Bone: {0}, start: {1}, end: {2}", bone.Type.ToString(), bone.PrevJoint, bone.NextJoint);
                        //Logger.Info("Model Bone: {0}, start: {1}, end: {2}", getFingerBoneType(bone.Type.ToString()), modelBone.LocalStartPosition, modelBone.LocalEndPosition);
                    }

                    modelFinger.Bones = fingerBones;
                    fingers.Add(modelFinger);
                }

                // Get the hand's normal vector and direction
                Vector normal = hand.PalmNormal;
                Vector direction = hand.Direction;

                float pitch = direction.Pitch * 180.0f / (float)Math.PI;
                float roll = normal.Roll * 180.0f / (float)Math.PI;
                float yaw = direction.Yaw * 180.0f / (float)Math.PI;
                //Logger.Info("Leap Hand pitch: {0} degrees, roll: {1} degrees, yaw: {2} degrees", pitch, roll, yaw);

                // Create Model hand
                Model.LeapMotion.Hand modelHand = new Model.LeapMotion.Hand();
                modelHand.LocalRotation = System.Numerics.Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
                modelHand.LocalPosition = new System.Numerics.Vector3(direction.x, direction.y, direction.z);
                modelHand.Side = getHandSideType(hand);
                modelHand.Fingers = fingers;
                //Logger.Info("Model Hand side: {0}", modelHand.Side.ToString());

                // Create hand body to add to mocapFrame
                Logger.Info("Hand Model: " + modelHand.Fingers.Count);
                VinteR.Model.Body modelBody = modelHand;
                mocapFrame.AddBody(ref modelBody);
            }

            // Add timestamp to mocapFrame and send to server via adapter
            mocapFrame.timestamp = System.DateTime.Now.ToString();
            adapter.OnFrameAvailable(mocapFrame);
        }

        /**
         * Helper method to get hand side type from Leap Motion hand
         */
        private Model.ESideType getHandSideType(Hand hand)
        {
            if (hand.IsLeft)
            {
                return Model.ESideType.Left;
            }
            return Model.ESideType.Right;
        }

        /**
         * Helper method to get finger type from Leap Motion finger type
         */
        private Model.LeapMotion.EFingerType getFingerType(String type)
        {
            switch(type.ToUpper())
            {
                case "TYPE_THUMB":
                    return Model.LeapMotion.EFingerType.Thumb;
                case "TYPE_INDEX":
                    return Model.LeapMotion.EFingerType.Index;
                case "TYPE_MIDDLE":
                    return Model.LeapMotion.EFingerType.Middle;
                case "TYPE_RING":
                    return Model.LeapMotion.EFingerType.Ring;
                case "TYPE_PINKY":
                    return Model.LeapMotion.EFingerType.Pinky;
                default:
                    return Model.LeapMotion.EFingerType.Index;
            }
        }

        /**
         * Helper method to get finger bone type from Leap Motion finger bone type
         */
        private Model.LeapMotion.EFingerBoneType getFingerBoneType(String type)
        {
            switch (type.ToUpper())
            {
                case "TYPE_METACARPAL":
                    return Model.LeapMotion.EFingerBoneType.Metacarpal;
                case "TYPE_PROXIMAL":
                    return Model.LeapMotion.EFingerBoneType.Proximal;
                case "TYPE_INTERMEDIATE":
                    return Model.LeapMotion.EFingerBoneType.Intermediate;
                case "TYPE_DISTAL":
                    return Model.LeapMotion.EFingerBoneType.Distal;
                default:
                    return Model.LeapMotion.EFingerBoneType.Metacarpal;
            }
        }
    }
}
