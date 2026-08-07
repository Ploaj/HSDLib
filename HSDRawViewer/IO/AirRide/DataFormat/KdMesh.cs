using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

        [DisplayName("Kind")]
        [JsonPropertyName("kind")]
        public KdMeshKind Kind { get; set; } = KdMeshKind.Basic;

        [DisplayName("Parent Bone")]
        [JsonPropertyName("parent")]
        public int Parent { get; set; } = -1;

        [DisplayName("Force X")]
        [Description("X component of force used by conveyor.")]
        [JsonPropertyName("fx")]
        public float ConveyorForceX { get; set; }

        [DisplayName("Force Y")]
        [Description("Y component of force used by conveyor.")]
        [JsonPropertyName("fy")]
        public float ConveyorForceY { get; set; }

        [DisplayName("Force Z")]
        [Description("Z component of force used by conveyor.")]
        [JsonPropertyName("fz")]
        public float ConveyorForceZ { get; set; }

        [JsonPropertyName("vertices")]
        public List<List<float>> Vertices { get; set; } = new List<List<float>>();

        [JsonPropertyName("triangles")]
        public List<KdTriangle> Triangles { get; set; } = new List<KdTriangle>();

        [JsonPropertyName("materials")]
        public List<KdMaterial> Materials { get; set; } = new List<KdMaterial>();
    }
}
