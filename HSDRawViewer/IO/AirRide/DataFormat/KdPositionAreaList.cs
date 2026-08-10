using HSDRaw.AirRide.Gr.Data;
using HSDRawViewer.Rendering.Models;
using System.Linq;
using HSDRawViewer.Tools;
using System.ComponentModel;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    public enum KdAreaPositionKind
    {
        ITEM_AREA,
        VEHICLE_AREA
    }

    public class KdPositionArea
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KdVector StartPosition { get; set; } = new KdVector();

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KdVector StartDirection { get; set; } = new KdVector(0, 0, 1);

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KdVector EndPosition { get; set; } = new KdVector();

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KdVector EndDirection { get; set; } = new KdVector(0, 0, 1);

        public KdPositionArea() { }

        public KdPositionArea(KAR_grAreaPositionData d1, KAR_grAreaPositionData d2)
        {
            StartPosition = new KdVector (d1.X, d1.Y, d1.Z);
            StartDirection = new KdVector (d1.DX, d1.DY, d1.DZ);

            EndPosition = new KdVector(d2.X, d2.Y, d2.Z);
            EndDirection = new KdVector(d2.DX, d2.DY, d2.DZ);
        }

        public (KAR_grAreaPositionData, KAR_grAreaPositionData) ToPositionData()
        {
            KAR_grAreaPositionData d1 = new()
            {
                X = StartPosition.X,
                Y = StartPosition.Y,
                Z = StartPosition.Z,
                DX = StartDirection.X,
                DY = StartDirection.Y,
                DZ = StartDirection.Z,
            };

            KAR_grAreaPositionData d2 = new()
            {
                X = EndPosition.X,
                Y = EndPosition.Y,
                Z = EndPosition.Z,
                DX = EndDirection.X,
                DY = EndDirection.Y,
                DZ = EndDirection.Z,
            };

            return (d1, d2);
        }
    }

    public class KdPositionAreaList
    {
        [Browsable(false)]
        public ObservableList<KdPositionArea> Positions { get; set; } = new ObservableList<KdPositionArea>();

        public KdPositionAreaList() { }

        public KdPositionAreaList(LiveJObj root, KAR_grAreaPositionList list)
        {
            if (list.JointIndices != null)
            {
                var ids = list.JointIndices.Array;

                int index = 0;
                foreach (var i in ids)
                {
                    var joint = root.GetJObjAtIndex(i);
                    var world = joint.WorldTransform;
                    var loc = world.ExtractTranslation();

                    Positions.Add(new KdPositionArea());

                    index++;
                    if (index >= list.Count)
                        break;
                }
            }
            else if (list.AreaPosition != null)
            {
                var arr = list.AreaPosition.Array;
                for (int i = 0; i < list.Count; i += 2)
                {
                    Positions.Add(new KdPositionArea(arr[i], arr[i + 1]));
                }
            }
        }

        public KAR_grAreaPositionList ToPositionList()
        {
            var ls = new KAR_grAreaPositionList()
            {
                AreaPosition = new HSDRaw.HSDArrayAccessor<KAR_grAreaPositionData>()
                {
                    Array = Positions.SelectMany(p => new KAR_grAreaPositionData[] { p.ToPositionData().Item1, p.ToPositionData().Item2 }).ToArray(),
                },
                Count = Positions.Count(),
            };

            return ls;
        }
    }
}
