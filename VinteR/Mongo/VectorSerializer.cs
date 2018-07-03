using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VinteR.Mongo
{
    class VectorSerializer : SerializerBase<Vector3>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Vector3 value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteName("X");
            context.Writer.WriteDouble(value.X);
            context.Writer.WriteName("Y");
            context.Writer.WriteDouble(value.Y);
            context.Writer.WriteName("Z");
            context.Writer.WriteDouble(value.Z);
            context.Writer.WriteEndDocument();
        }

        public override Vector3 Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var rawDoc = context.Reader.ReadRawBsonDocument();
            var doc = new RawBsonDocument(rawDoc);

            Boolean providedX = doc.Contains("X");
            Boolean providedY = doc.Contains("Y");
            Boolean providedZ = doc.Contains("Z");

            if (providedX && providedY && providedZ)
            {
                var vector = new Vector3(
                                          (float) doc.GetElement("X").Value.AsDouble,
                                          (float) doc.GetElement("Y").Value.AsDouble,
                                          (float) doc.GetElement("Z").Value.AsDouble
                                         );
                return vector;

            }
            else
            {
                Logger.Error("Deserialization Problem - Data Structure is not valid");
                throw new ApplicationException("Deserialization Problem - Data Structure is not valid");
            }

        }

    }
}
