using HSDRaw.AirRide.Gr.Data;
using System.ComponentModel;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    public class KdPosition
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KdVector Position { get; set; } = new KdVector();

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KdVector Forward { get; set; } = new KdVector();

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KdVector Up { get; set; } = new KdVector();

        public KAR_grPositionData ToPositionData()
        {
            KAR_grPositionData d = new();

            if (Position != null)
            {
                d.X = Position.X;
                d.Y = Position.Y;
                d.Z = Position.Z;
            }
            if (Forward != null)
            {
                d.FX = Forward.X;
                d.FY = Forward.Y;
                d.FZ = Forward.Z;
            }
            if (Up != null)
            {
                d.UX = Up.X;
                d.UY = Up.Y;
                d.UZ = Up.Z;
            }

            return d;
        }
    }
}
