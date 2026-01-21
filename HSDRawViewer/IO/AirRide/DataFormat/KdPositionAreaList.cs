using HSDRaw.AirRide.Gr.Data;
using HSDRawViewer.Rendering.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using OpenTK.Mathematics;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    public enum KdAreaPositionKind
    {
        ITEM_AREA,
        VEHICLE_AREA
    }

    internal class KdPositionArea
    {
        public float[] Position { get; set; }

        public float[] Plane { get; set; }

        public KdPositionArea() { }

        public KdPositionArea(Vector3 position, Vector3 plane)
        {
            Position = new float[] { position.X, position.Y, position.Z };
            Plane = new float[] { plane.X, plane.Y, plane.Z };
        }

        public KAR_grAreaPositionData ToPositionData()
        {
            KAR_grAreaPositionData d = new();

            if (Position != null && Position.Length >= 3)
            {
                d.X = Position[0];
                d.Y = Position[1];
                d.Z = Position[2];
            }

            if (Plane != null && Plane.Length >= 3)
            {
                d.DX = Plane[0];
                d.DY = Plane[1];
                d.DZ = Plane[2];
            }

            return d;
        }
    }

    internal class KdPositionAreaList
    {

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public KdAreaPositionKind Kind { get; set; }

        public int Slot { get; set; }

        public List<KdPositionArea> Positions { get; set; } = new List<KdPositionArea>();

        public KdPositionAreaList() { }

        public KdPositionAreaList(KdAreaPositionKind kind, int slot, LiveJObj root, KAR_grAreaPositionList list)
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
                    var world = joint.WorldTransform;
                    var loc = world.ExtractTranslation();

                    Positions.Add(new KdPositionArea(loc, new Vector3(0, 0, -1)));

                    index++;
                    if (index >= list.Count)
                        break;
                }
            }
            else if (list.AreaPosition != null)
            {
                int index = 0;
                foreach (var p in list.AreaPosition.Array)
                {
                    Positions.Add(new KdPositionArea()
                    {
                        Position = new float[] { p.X, p.Y, p.Z },
                        Plane = new float[] { p.DX, p.DY, p.DZ },
                    });

                    index++;
                    if (index >= list.Count)
                        break;
                }
            }
        }

        public KAR_grAreaPositionList ToPositionList()
        {
            var ls = new KAR_grAreaPositionList()
            {
                AreaPosition = new HSDRaw.HSDArrayAccessor<KAR_grAreaPositionData>()
                {
                    Array = Positions.Select(p => p.ToPositionData()).ToArray(),
                },
                Count = Positions.Count,
            };

            return ls;
        }
    }
}
