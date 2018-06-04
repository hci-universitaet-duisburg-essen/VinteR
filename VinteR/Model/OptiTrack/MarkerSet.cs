using System.Collections.Generic;

namespace VinteR.Model.OptiTrack
{
    class MarkerSet : OptiTrackBody
    {
        /// <summary>
        /// List of Markers in the markerset
        /// </summary>
        public IList<Point> Markers
        {
            get => _markers;
            set
            {
                if (value == null) _markers.Clear();
                else _markers = value;
            }
        }

        private IList<Point> _markers;

        public MarkerSet(string id) : base(id)
        {
            this._markers = new List<Point>();
            this.Type = EBodyType.MarkerSet;
        }
    }
}
