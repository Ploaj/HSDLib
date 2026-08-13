using HSDRaw.AirRide.Gr.Data;
using HSDRawViewer.Rendering.Models;
using OpenTK.Mathematics;
using System.Linq;
using HSDRawViewer.Tools;
using System.ComponentModel;
using System;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    public enum KdPositionKind
    {
        START,
        ENEMY,
        GRAVITY,
        AIRFLOW,
        CONVEYOR,
        ITEM,
        EVENT,
        VEHICLE,
        GLOBAL_DEAD,
        LOCAL_DEAD,
        YAKUMONO,
    }

    public class KdPositionList
    {
        [Browsable(false)]
        public ObservableList<KdPosition> Positions { get; set; } = new ObservableList<KdPosition>();

        public KdPositionList() { }

        public KdPositionList(LiveJObj root, KAR_grPositionList list)
        {
            if (list.JointIndices != null)
            {
                var ids = list.JointIndices.Array;

                int index = 0;
                foreach (var i in ids)
                {
                    var joint = root.GetJObjAtIndex(i);
                    var m = joint.WorldTransform;

                    var p = m.ExtractTranslation();

                    var up = new Vector3(
                        m.M21,
                        m.M22,
                        m.M23
                    );

                    var forward = new Vector3(
                        m.M31,
                        m.M32,
                        m.M33
                    );

                    Positions.Add(new KdPosition()
                    {
                        Position    = new KdVector(p.X, p.Y, p.Z),
                        Forward     = new KdVector(forward.X, forward.Y, forward.Z),
                        Up          = new KdVector(up.X, up.Y, up.Z),
                    });

                    index++;
                    if (index >= list.Count)
                        break;
                }
            }
            else if (list.PositionData != null)
            {
                int count = list.Count;
                var data = list.PositionData.Array;
                for (int i = 0; i < Math.Min(count, data.Length); i++)
                {
                    var p = data[i];
                    Positions.Add(new KdPosition()
                    {
                        Position = new KdVector(p.X, p.Y, p.Z ),
                        Forward = new KdVector(p.FX, p.FY, p.FZ),
                        Up = new KdVector(p.UX, p.UY, p.UZ),
                    });
                }
            }
        }

        public KAR_grPositionList ToPositionList()
        {
            var ls = new KAR_grPositionList()
            {
                PositionData = new HSDRaw.HSDArrayAccessor<KAR_grPositionData>()
                {
                    Array = Positions.Select(p => p.ToPositionData()).ToArray(),
                },
                Count = Positions.Count(),
            };

            return ls;
        }
    }
}
