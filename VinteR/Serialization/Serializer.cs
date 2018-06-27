using System.Linq;
using Google.Protobuf.WellKnownTypes;
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
                    Name = body.Name ?? string.Empty
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

        public void ToProtoBuf(Session session, out Model.Gen.Session output)
        {
            var mocapFrames = session.MocapFrames.Select(f =>
            {
                ToProtoBuf(f, out var generatedFrame);
                return generatedFrame;
            });
            output = new Model.Gen.Session()
            {
                Name = session.Name,
                Duration = session.Duration,
                SessionStart = Timestamp.FromDateTime(session.Datetime)
            };
            output.Frames.AddRange(mocapFrames);
        }
    }
}