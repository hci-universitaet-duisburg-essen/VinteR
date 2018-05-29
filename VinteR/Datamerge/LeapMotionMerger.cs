using System;
using VinteR.Model;
using VinteR.Model.LeapMotion;

namespace VinteR.Datamerge
{
    public partial class DataMerger
    {
        public Body Merge(Hand hand)
        {
            //TODO complete leap motion merge implementation
            var result = new Body { BodyType = Body.EBodyType.Hand };
            OnMergedBodyAvailable(result);
            return result;
        }
    }
}
