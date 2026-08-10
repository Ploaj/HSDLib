
using System.ComponentModel;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool
{
    public class GrDisplaySettings
    {
        [Category("0 - Collisions")]
        [DisplayName("Opacity")]
        [Description("Opacity of collisions when not selected")]
        public float CollisionOpacity { get; set; } = 0.75f;

        [Category("0 - Collisions")]
        [DisplayName("Opacity (Selected)")]
        [Description("Opacity of collisions when selected")]
        public float CollisionSelectedOpacity { get; set; } = 0.9f;


        [Category("1 - Zones")]
        [DisplayName("Opacity")]
        [Description("Opacity of zones when not selected")]
        public float ZonesOpacity { get; set; } = 0.5f;

        [Category("1 - Zones")]
        [DisplayName("Opacity (Selected)")]
        [Description("Opacity of zones when selected")]
        public float ZonesSelectedOpacity { get; set; } = 0.7f;


        [Category("1 - Positions")]
        [DisplayName("Opacity")]
        [Description("Opacity of zones when not selected")]
        public float PositionOpacity { get; set; } = 0.5f;

        [Category("1 - Positions")]
        [DisplayName("Opacity (Selected)")]
        [Description("Opacity of zones when selected")]
        public float PositionSelectedOpacity { get; set; } = 1.0f;

        [Category("1 - Positions")]
        [DisplayName("Opacity (Selected)")]
        [Description("Opacity of zones when selected")]
        public float PositionRadius { get; set; } = 8.0f;

        [Category("1 - Positions")]
        [DisplayName("Opacity")]
        [Description("AreaOpacity of zones when not selected")]
        public float PositionAreaOpacity { get; set; } = 0.5f;

        [Category("1 - Positions")]
        [DisplayName("Area Opacity (Selected)")]
        [Description("Opacity of zones when selected")]
        public float PositionAreaSelectedOpacity { get; set; } = 0.7f;

    }

}
