using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NLog;
using VinteR.Adapter;
using VinteR.Model;
using VinteR.Model.LeapMotion;
using VinteR.Tracking;
using VinteR.Transform;

namespace VinteR.Datamerge
{
    public class LeapMotionMerger : IDataMerger
    {
        private const string NameDivider = "_";
        private static readonly Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static readonly Quaternion LeapMotionOptiTrackAdjustment =
            Quaternion.CreateFromAxisAngle(Vector3.UnitY, 180f.ToRadians());

        public string MergerType => HardwareSystems.LeapMotion;

        private readonly IAdapterTracker _adapterTracker;
        private readonly ITransformator _transformator;

        public LeapMotionMerger(IAdapterTracker adapterTracker, ITransformator transformator)
        {
            this._adapterTracker = adapterTracker;
            this._transformator = transformator;
        }

        public MocapFrame HandleFrame(MocapFrame frame)
        {
            foreach (var body in frame.Bodies)
            {
                if (body is Hand hand)
                {
                    var mergedHand = Merge(hand, frame.SourceId);
                    body.Load(mergedHand);
                }
                else
                {
                    Logger.Warn("Could not merge frame for {0,15} by type {1}", frame.SourceId, frame.AdapterType);
                }
            }
            return frame;
        }

        /**
         * Method to convert hand into hand body consisting of joint points
         */
        private Body Merge(Hand hand, string sourceId)
        {
            IList<Point> points = new List<Point>();
            var leapMotionPosition = _adapterTracker.Locate(sourceId);
            leapMotionPosition.Rotation = Quaternion.Multiply(leapMotionPosition.Rotation, LeapMotionOptiTrackAdjustment);
            Logger.Debug("Leap motion position: {0}", leapMotionPosition);

            var result = new Body
            {
                BodyType = Body.EBodyType.Hand,
                Side = hand.Side,
                Centroid = _transformator.GetGlobalPosition(leapMotionPosition, hand.LocalPosition),
                Rotation = hand.LocalRotation
            };
            // Convert all joints to points (each position only once!)
            if (hand.Fingers != null)
            {
                foreach (Finger finger in hand.Fingers)
                {
                    if (finger.Bones != null)
                    {
                        foreach (FingerBone bone in finger.Bones)
                        {
                            if (bone.Type == EFingerBoneType.Metacarpal) // first bone in hand, needs start and end point added
                            {
                                var boneGlobalStartPosition = _transformator.GetGlobalPosition(leapMotionPosition, bone.LocalStartPosition);
                                points.Add(CreatePoint(boneGlobalStartPosition, finger.Type, bone.Type));
                                Logger.Debug(finger.Type.ToString() + " point: " + bone.LocalStartPosition.ToString() + ", global point: " + boneGlobalStartPosition);

                                if (finger.Type != EFingerType.Thumb) // thumb has zero length metacarpal bone, so do not add end point as well
                                {
                                    var boneGlobalEndPosition = _transformator.GetGlobalPosition(leapMotionPosition,
                                        bone.LocalEndPosition);
                                    points.Add(CreatePoint(boneGlobalEndPosition, finger.Type, bone.Type));
                                }
                            }
                            else // add all other bone end points
                            {
                                var boneGlobalEndPosition = _transformator.GetGlobalPosition(leapMotionPosition,
                                    bone.LocalEndPosition);
                                points.Add(CreatePoint(boneGlobalEndPosition, finger.Type, bone.Type));
                            }
                        }
                    }
                }
            }

            Logger.Debug("Number of hand points (should be 24): " + points.Count);

            // alert that merged body is available
            result.Points = points;
            return result;
        }

        private static Point CreatePoint(Vector3 position, EFingerType fingerType, EFingerBoneType boneType)
        {
            return new Point(position)
            {
                Name = string.Join(NameDivider, fingerType.ToString(), boneType.ToString())
            };
        }
    }
}