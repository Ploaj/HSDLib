using HSDRaw.AirRide.Gr.Data;
using HSDRawViewer.Rendering.Models;
using IONET.Collada.Core.Lighting;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Linq;

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

    internal class KdPosition
    {
        public float[] Position { get; set; }
        public float[] Forward { get; set; }
        public float[] Up { get; set; }

        public KAR_grPositionData ToPositionData()
        {
            KAR_grPositionData d = new();

            if (Position != null && Position.Length >= 3)
            {
                d.X = Position[0];
                d.Y = Position[1];
                d.Z = Position[2];
            }
            if (Forward != null && Forward.Length >= 3)
            {
                d.FX = Forward[0];
                d.FY = Forward[1];
                d.FZ = Forward[2];
            }
            if (Up != null && Up.Length >= 3)
            {
                d.UX = Up[0];
                d.UY = Up[1];
                d.UZ = Up[2];
            }

            return d;
        }
    }

    internal class KdPositionList
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public KdPositionKind Kind { get; set; }

        public int Slot { get; set; } = 0;

        public List<KdPosition> Positions { get; set; } = new List<KdPosition>();

        public KdPositionList() { }

        public KdPositionList(KdPositionKind kind, int slot, LiveJObj root, KAR_grPositionList list)
        {
            Kind = kind;
            Slot = slot;

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
                        Position    = new float[] { p.X, p.Y, p.Z },
                        Forward     = new float[] { forward.X, forward.Y, forward.Z},
                        Up          = new float[] { up.X, up.Y, up.Z },
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
                for (int i = 0; i < count; i++)
                {
                    var p = data[i];
                    Positions.Add(new KdPosition()
                    {
                        Position    = new float[] { p.X, p.Y, p.Z },
                        Forward     = new float[] { p.FX, p.FY, p.FZ },
                        Up          = new float[] { p.UX, p.UY, p.UZ },
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
                Count = Positions.Count,
            };

            return ls;
        }
    }
}
