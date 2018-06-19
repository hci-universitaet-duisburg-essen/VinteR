using System.Linq;
using System.Numerics;
using NLog;
using VinteR.Model;
using VinteR.Model.OptiTrack;

namespace VinteR.Datamerge
{
    public class OptiTrackMerger : IDataMerger
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public MocapFrame HandleFrame(MocapFrame frame)
        {
            foreach (var body in frame.Bodies)
            {
                if (body is OptiTrackBody optiTrackBody)
                {
                    var mergedBody = Merge(optiTrackBody);
                    body.Load(mergedBody);
                }
                else
                {
                    Logger.Warn("Could not merge frame for {0,15} by type {1}", frame.SourceId, frame.AdapterType);
                }
            }
            return frame;
        }

        public Body Merge(OptiTrackBody body)
        {
            Body result;
            switch (body.BodyType)
            {
                case Body.EBodyType.Skeleton:
                    result = MergeSkeleton(body as Skeleton);
                    break;
                default:
                    result = MergeDefault(body);
                    break;
            }

            return result;
        }

        private static Body MergeSkeleton(Skeleton skeleton)
        {
            var points = skeleton.RigidBodies.SelectMany(rb => rb.Points).ToList();

            var body = new Body
            {
                BodyType = Body.EBodyType.Skeleton,
                Points = points,
                Rotation = skeleton.Rotation
            };
            return body;
        }

        private static Body MergeDefault(OptiTrackBody body)
        {
            var result = new Body
            {
                Points = body.Points,
                Centroid = body.Centroid,
                Rotation = body.Rotation,
                Side = body.Side
            };
            if (result.Points?.Count == 1 && result.BodyType.Equals(Body.EBodyType.MarkerSet))
                result.BodyType = Body.EBodyType.Marker;
            return result;
        }
    }
}