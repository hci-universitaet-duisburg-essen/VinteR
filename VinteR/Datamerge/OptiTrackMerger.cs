using System;
using VinteR.Model;
using VinteR.Model.OptiTrack;

namespace VinteR.Datamerge
{
    public partial class DataMerger
    {
        public Body Merge(OptiTrackBody body)
        {
            //TODO complete optitrack merge implementation
            var result = new Body {BodyType = Body.EBodyType.MarkerSet};
            return result;
        }
    }
}
