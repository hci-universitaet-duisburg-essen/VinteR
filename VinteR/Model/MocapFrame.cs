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
        
       
        [BsonIgnore]
        private List<Body> _bodies;

        /// <summary>
        /// Contains a list of bodies that the input adapter has
        /// detected.
        /// </summary>

        [BsonElement]
        public List<Body> Bodies
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
        }

        public MocapFrame(string sourceId, string adapter, IList<Body> bodies)
        {
            this.Bodies = (List<Body>) bodies;
            this.SourceId = sourceId;
            this.AdapterType = adapter;
        }

        public void AddBody(ref Body body)
        {
            this.Bodies.Add(body);
        }
    }
}