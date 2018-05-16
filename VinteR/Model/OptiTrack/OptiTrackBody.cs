namespace VinteR.Model.OptiTrack
{
    ///  <inheritdoc />
    /// <summary>
    ///  An opti track body is the base class for all bodies
    ///  tracked by OptiTrack. Each body has a specific type, as
    ///  optitrack handles marker sets, rigid bodies, skeletons
    ///  and force plates.
    ///  <seealso cref="https://v20.wiki.optitrack.com/index.php?title=NatNet:_Data_Types#Frame_of_Mocap_Data"/>
    /// </summary>
    public class OptiTrackBody : Body
    {
        /// <summary>
        /// Unique identifier given by optitrack
        /// </summary>
        public string OptiTrackId { get; }

        /// <summary>
        /// Type of the body, MarkerSet by default.
        /// </summary>
        public BodyTypeEnum Type { get; protected set; }

        public OptiTrackBody(string id)
        {
            this.OptiTrackId = id;
            this.Type = BodyTypeEnum.MarkerSet;
        }
    }
}