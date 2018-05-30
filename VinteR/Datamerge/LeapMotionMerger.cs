using System;
using System.Collections.Generic;
using VinteR.Model;
using VinteR.Model.LeapMotion;

namespace VinteR.Datamerge
{
    public partial class DataMerger
    {
        /**
         * Method to convert hand into hand body consisting of joint points
         */
        public Body Merge(Hand hand)
        {
            var result = new Body { BodyType = Body.EBodyType.Hand };
            IList<Point> points = new List<Point>();

            // Convert all joints to points (each position only once!)
            if (hand.Fingers != null)
            {
                foreach (Finger finger in hand.Fingers)
                {
                    //Logger.Info("Finger: " + finger.Type.ToString());
                    if (finger.Bones != null)
                    {
                        foreach (FingerBone bone in finger.Bones)
                        {
                            //Logger.Info("Finger Bone: " + bone.ToString());
                            if (bone.Type == EFingerBoneType.Metacarpal) // first bone in hand, needs start and end point added
                            {
                                points.Add(new Point(bone.LocalStartPosition));
                                //Logger.Info(finger.Type.ToString() + " point: " + bone.LocalStartPosition.ToString());

                                if (finger.Type != EFingerType.Thumb) // thumb has zero length metacarpal bone, so do not add end point as well
                                {
                                    points.Add(new Point(bone.LocalEndPosition));
                                    //Logger.Info(finger.Type.ToString() + " point: " + bone.LocalEndPosition.ToString());
                                }
                            }
                            else // add all other bone end points
                            {
                                points.Add(new Point(bone.LocalEndPosition));
                                //Logger.Info(finger.Type.ToString() + " point: " + bone.LocalEndPosition.ToString());
                            }
                        }
                    }
                }
            }

            //Logger.Info("Number of hand points (should be 24): " + points.Count);

            // alert that merged body is available
            result.Points = points;
            FireBodyMerged(result);
            return result;
        }
    }
}
