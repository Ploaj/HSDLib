using HSDRaw.AirRide.Gr.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Text.Json.Serialization;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    public enum KdMeshKind
    {
        Basic,

        [Display(Name = "Conveyor Unused")]
        Conveyor1,

        [Display(Name = "Conveyor")]
        Conveyor2,

        Breakable,

        [Display(Name = "Moving (Translation)")]
        MovingTranslation,

        [Display(Name = "Moving (Rotation)")]
        MovingRotation,
    }

    public class KdMesh
    {
        [Browsable(false)]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Category("0 - General")]
        [DisplayName("Kind")]
        [JsonPropertyName("kind")]
        public KdMeshKind Kind { get; set; } = KdMeshKind.Basic;

        [Category("0 - General")]
        [DisplayName("Parent Bone")]
        [JsonPropertyName("parent")]
        public int Parent { get; set; } = 0;

        [Category("1 - Conveyor")]
        [DisplayName("Force X")]
        [Description("X component of force used by conveyor.")]
        [JsonPropertyName("fx")]
        public float ConveyorForceX { get; set; }

        [Category("1 - Conveyor")]
        [DisplayName("Force Y")]
        [Description("Y component of force used by conveyor.")]
        [JsonPropertyName("fy")]
        public float ConveyorForceY { get; set; }

        [Category("1 - Conveyor")]
        [DisplayName("Force Z")]
        [Description("Z component of force used by conveyor.")]
        [JsonPropertyName("fz")]
        public float ConveyorForceZ { get; set; }

        [Category("2 - Data")]
        [JsonPropertyName("vertices")]
        public List<List<float>> Vertices { get; set; } = new List<List<float>>();

        [Category("2 - Data")]
        [JsonPropertyName("triangles")]
        public List<KdTriangle> Triangles { get; set; } = new List<KdTriangle>();

        [Category("2 - Data")]
        [JsonPropertyName("materials")]
        public List<KdMaterial> Materials { get; set; } = new List<KdMaterial>();

        private bool IsConveyor => Kind == KdMeshKind.Conveyor1 || Kind == KdMeshKind.Conveyor2;

        public string ToMeshString()
        {
            var s = string.Join("_",
                "M",
                $"K{Kind}",
                $"P{Parent}"
            );

            if (IsConveyor)
            {
                s += "_" + string.Join("_",
                    $"X{ConveyorForceX:F4}",
                    $"Y{ConveyorForceY:F4}",
                    $"Z{ConveyorForceZ:F4}");
            }

            return s;
        }

        public void FromMeshString(string name)
        {
            foreach (var part in name.Split('_'))
            {
                if (part.StartsWith("K"))
                {
                    if (Enum.TryParse(part[1..], out KdMeshKind result))
                    {
                        Kind = result;
                    }
                }
                else if (part.StartsWith("P"))
                {
                    if (int.TryParse(part[1..], out int result))
                    {
                        Parent = result;
                    }
                }
                else if (part.StartsWith("X"))
                {
                    if (float.TryParse(part[1..], out float result))
                        ConveyorForceX = result;
                }
                else if (part.StartsWith("Y"))
                {
                    if (float.TryParse(part[1..], out float result))
                        ConveyorForceY = result;
                }
                else if (part.StartsWith("Z"))
                {
                    if (float.TryParse(part[1..], out float result))
                        ConveyorForceZ = result;
                }
            }
        }
    }
}
