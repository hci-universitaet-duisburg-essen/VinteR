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
    class QuaternionSerializer : SerializerBase<Quaternion>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Quaternion value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteName("X");
            context.Writer.WriteDouble(value.X);
            context.Writer.WriteName("Y");
            context.Writer.WriteDouble(value.Y);
            context.Writer.WriteName("Z");
            context.Writer.WriteDouble(value.Z);
            context.Writer.WriteName("W");
            context.Writer.WriteDouble(value.W);
            context.Writer.WriteEndDocument();
        }

        public override Quaternion Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var rawDoc = context.Reader.ReadRawBsonDocument();
            var doc = new RawBsonDocument(rawDoc);

            Boolean providedX = doc.Contains("X");
            Boolean providedY = doc.Contains("Y");
            Boolean providedZ = doc.Contains("Z");
            Boolean providedW = doc.Contains("W");

            if (providedX && providedY && providedZ && providedW)
            {
                var vector = new Vector3(
                                          (float)doc.GetElement("X").Value.AsDouble,
                                          (float)doc.GetElement("Y").Value.AsDouble,
                                          (float)doc.GetElement("Z").Value.AsDouble
                                         );
                var quaternion = new Quaternion(vector, (float) doc.GetElement("W").Value.AsDouble);

                return quaternion;
            }
            else
            {
                Logger.Error("Deserialization Problem - Data Structure is not valid");
                throw new ApplicationException("Deserialization Problem - Data Structure is not valid");
            }

        }
    }
}
