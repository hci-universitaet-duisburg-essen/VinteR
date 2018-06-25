using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Collections;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace VinteR.Model
{
    /// <summary>
    /// A motion capture frame contains data sent by one input adapter and
    /// contains all data regarding position and rotation of recognized
    /// objects.
    /// </summary>
    public class MocapFrame
    {
        [BsonId]
        public BsonObjectId _id;

        /// <summary>
        /// Time in milliseconds since application start
        /// </summary>
        
        public long ElapsedMillis { get; set; }

        /// <summary>
        /// Name of the input adapter that sends the frame
        /// </summary>
        
        public string SourceId { get; set; }

        /// <summary>
        /// Contains the type of the adapter that sends the frame
        /// </summary>
        
        public string AdapterType { get; set; }

        /// <summary>
        /// There might be a gesture recognized through validation
        /// of previous frames. If the gesture is completely recorgnized
        /// this field contains the name of the gesture.
        /// </summary>
        
        public string Gesture { get; set; } = "";

        /// <summary>
        /// Contains the time when all tracking data is processed and
        /// ready to be streamed.
        /// </summary>
        
        public float Latency { get; set; }

        // [BsonElement]
        // private List<Body> _bson_bodies { get; set; }

        
        public List<BsonObjectId> _referenceBodies;
        
       
        [BsonIgnore]
        private IList<Body> _bodies;

        /// <summary>
        /// Contains a list of bodies that the input adapter has
        /// detected.
        /// </summary>
        
        [BsonIgnore]
        public IList<Body> Bodies
        {
            get => _bodies;
            set
            {
                if (value == null)
                {
                    _bodies.Clear();
                }
                else
                {
                    _bodies = value;
                }
            }
        }
        
        public MocapFrame(string sourceId, string adapter)
        {
            this.Bodies = new List<Body>();
            this.SourceId = sourceId;
            this.AdapterType = adapter;
            this._referenceBodies = new List<BsonObjectId>();
        }

        public MocapFrame(string sourceId, string adapter, IList<Body> bodies)
        {
            this.Bodies = bodies;
            this.SourceId = sourceId;
            this.AdapterType = adapter;
            this._referenceBodies = new List<BsonObjectId>();
        }

        public void AddBody(ref Body body)
        {
            this.Bodies.Add(body);
        }

        /// <summary>
        /// Maps this mocap frame to a mocap frame defined inside the
        /// protobuf model. For faster calculation this should be
        /// done in each input adapter.
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            byte[] bytes;
            // create mapping from MocapFrame to Gen.MocapFrame
            var protoBufFrame = new Gen.MocapFrame()
            {
                AdapterType = this.AdapterType,
                ElapsedMillis = this.ElapsedMillis,
                Gesture = this.Gesture ?? "", // set default value otherwise serialization breaks
                Latency = this.Latency,
                SourceId = this.SourceId
            };

            foreach (var body in Bodies)
            {
                var protoBody = new Gen.MocapFrame.Types.Body()
                {
                    BodyType = body.GetBodyTypeProto(),
                    Rotation = body.Rotation.ToProto(),
                    SideType = body.GetSideTypeProto()
                };
                foreach (var point in body.Points)
                {
                    var protoPoint = new Gen.MocapFrame.Types.Body.Types.Point()
                    {
                        Name = point.Name ?? "",
                        State = point.State ?? "",
                        Position = point.Position.ToProto()
                    };
                    protoBody.Points.Add(protoPoint);
                }
                protoBufFrame.Bodies.Add(protoBody);
            }

            using (var stream = new MemoryStream())
            {
                // Save the frame to a stream
                protoBufFrame.WriteTo(stream);
                bytes = stream.ToArray();
            }

            return bytes;
        }
    }
}