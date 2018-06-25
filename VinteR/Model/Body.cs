﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace VinteR.Model
{
    /// <summary>
    /// A <code>Body</code> defines a single object that is build
    /// upon a collection of points. For various input adapters the
    /// body may have a more specific type. In addition a body has
    /// a rotation based on the global coordinate system.
    /// </summary>
    public class Body
    {   
        // This just defines a BodyType Enum we need a separate of type BodyType to hold the information
        public enum EBodyType
        {
            Marker,
            MarkerSet,
            RigidBody,
            Skeleton,
            Hand
        }

        [BsonId]
        public BsonObjectId _id;

        public EBodyType BodyType { get; set; }

        /// <summary>
        /// Contains the side of a body if one exists, for example "left" hand
        /// </summary>
        public ESideType Side { get; set; } = ESideType.NoSide;

        private IList<Point> _points;

        /// <summary>
        /// Collection of points that may be connected or are
        /// loose coupled and define the structure of this body.
        /// </summary>
        
        public IList<Point> Points
        {
            get => _points;
            set
            {
                if (value == null) _points.Clear();
                else _points = value;
            }
        }

        /// <summary>
        /// Contains the rotation of this body inside the global
        /// coordinate system.
        /// </summary>
        
        public Quaternion Rotation { get; set; }

        // The Body Type of the Body object
        public Body()
        {
            this._points = new List<Point>();
        }

        /// <summary>
        /// Loads all values from properties of given source object
        /// into this body.
        /// </summary>
        /// <param name="source"></param>
        public void Load(Body source)
        {
            this.BodyType = source.BodyType;
            this.Points = source.Points;
            this.Rotation = source.Rotation;
        }

        public Gen.MocapFrame.Types.Body.Types.EBodyType GetBodyTypeProto()
        {
            switch (BodyType)
            {
                case EBodyType.Hand:
                    return Gen.MocapFrame.Types.Body.Types.EBodyType.Hand;
                case EBodyType.MarkerSet:
                    return Gen.MocapFrame.Types.Body.Types.EBodyType.MarkerSet;
                case EBodyType.Marker:
                    return Gen.MocapFrame.Types.Body.Types.EBodyType.Marker;
                case EBodyType.RigidBody:
                    return Gen.MocapFrame.Types.Body.Types.EBodyType.RigidBody;
                case EBodyType.Skeleton:
                    return Gen.MocapFrame.Types.Body.Types.EBodyType.Skeleton;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Gen.MocapFrame.Types.Body.Types.ESideType GetSideTypeProto()
        {
            switch (Side)
            {
                case ESideType.Left:
                    return Gen.MocapFrame.Types.Body.Types.ESideType.Left;
                case ESideType.Right:
                    return Gen.MocapFrame.Types.Body.Types.ESideType.Right;
                case ESideType.NoSide:
                    return Gen.MocapFrame.Types.Body.Types.ESideType.NoSide;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
