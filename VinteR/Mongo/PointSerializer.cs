using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinteR.Model;

namespace VinteR.Mongo
{
    // The class is currently not required since we have implemented Vector3 Mapping
    public class PointSerializer : SerializerBase<Point>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public override Point Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var rawDoc = context.Reader.ReadRawBsonDocument();
            var doc = new RawBsonDocument(rawDoc);

            Boolean providedName = doc.Contains("Name");
            Boolean providedState = doc.Contains("State");
            Boolean providedPosition = doc.Contains("Position");

            if (providedName && providedState && providedPosition)
            {
                var pointName = doc.GetElement("Name");
                var pointState = doc.GetElement("State");
                var pointPosition = doc.GetElement("Position");

                var pointDocument = pointPosition.Value.AsBsonDocument;

                if (pointDocument.Contains("X") && pointDocument.Contains("Y") && pointDocument.Contains("Z"))
                {
                    var point = new Point(
                                          (float) pointDocument.GetElement("X").Value.AsDouble,
                                          (float) pointDocument.GetElement("Y").Value.AsDouble,
                                          (float) pointDocument.GetElement("Z").Value.AsDouble
                                          );
                    point.Name = pointName.Value.AsString;
                    point.State = pointState.Value.AsString;

                    return point;

                } else
                {
                    Logger.Error("Deserialization Problem - Data Structure is not valid: PointPosition");
                    throw new ApplicationException("Deserialization Problem - Data Structure is not valid: Point Position");
                }

            } else
            {
                Logger.Error("Deserialization Problem - Data Structure is not valid");
                throw new ApplicationException("Deserialization Problem - Data Structure is not valid");
            }
            
        }
    }
}
