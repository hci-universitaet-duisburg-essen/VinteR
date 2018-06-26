using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace VinteR.Model
{
    /// <summary>
    /// A <code>Session</code> is a record that is created on application start
    /// and ended on application stop.
    /// </summary>
    public class Session
    {
        /// <summary>
        /// Unique name of a session
        /// </summary>
        [BsonElement]
        public string Name { get; }

        /// <summary>
        /// Datetime when the session was created
        /// </summary>
        [BsonElement]
        public DateTime Datetime { get; }

        /// <summary>
        /// Time in millis how long the session lasts
        /// </summary>
        [BsonElement]
        public long Duration { get; set; }

        /// <summary>
        /// Frames for the session. This MUST NOT be set during a
        /// running record as it leads to memory issues.
        /// </summary>
        [BsonIgnore]
        public IList<MocapFrame> MocapFrames
        {
            get => _mocapFrames;
            set
            {
                if (value == null) _mocapFrames.Clear();
                else _mocapFrames = value;
            }
        }

        [BsonIgnore]
        private IList<MocapFrame> _mocapFrames;

        /// <summary>
        /// Creates a new session with given name and current datetime
        /// </summary>
        /// <param name="name"></param>
        public Session(string name)
        {
            _mocapFrames = new List<MocapFrame>();
            Name = name;
            Datetime = DateTime.Now;
        }
    }
}