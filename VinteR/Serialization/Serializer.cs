using System.IO;
using Google.Protobuf;
using VinteR.Model;

namespace VinteR.Serialization
{
    public class Serializer : ISerializer
    {
        public void ToProtoBuf(MocapFrame frame, out Model.Gen.MocapFrame output)
        {
            // create mapping from MocapFrame to Gen.MocapFrame
            output = new Model.Gen.MocapFrame()
            {
                AdapterType = frame.AdapterType,
                ElapsedMillis = frame.ElapsedMillis,
                Gesture = frame.Gesture ?? "", // set default value otherwise serialization breaks
                Latency = frame.Latency,
                SourceId = frame.SourceId
            };

            foreach (var body in frame.Bodies)
            {
                var protoBody = new Model.Gen.MocapFrame.Types.Body()
                {
                    BodyType = body.GetBodyTypeProto(),
                    Rotation = body.Rotation.ToProto(),
                    SideType = body.GetSideTypeProto(),
                    Centroid = body.Centroid.ToProto(),
                    Name = body.Name
                };
                foreach (var point in body.Points)
                {
                    var protoPoint = new Model.Gen.MocapFrame.Types.Body.Types.Point()
                    {
                        Name = point.Name ?? "",
                        State = point.State ?? "",
                        Position = point.Position.ToProto()
                    };
                    protoBody.Points.Add(protoPoint);
                }
                output.Bodies.Add(protoBody);
            }
        }
    }
}