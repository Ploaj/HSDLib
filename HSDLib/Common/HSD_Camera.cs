namespace HSDLib.Common
{
    public class HSD_Camera : IHSDNode
    {
        public int Unknown1 { get; set; }
        public int Flag { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ProjWidth { get; set; }
        public int ProjHeight { get; set; }
        public HSD_CameraInfo CamInfo1 { get; set; }
        public HSD_CameraInfo CamInfo2 { get; set; }
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }
        public float Unknown4 { get; set; }
        public float Unknown5 { get; set; }
        public float Unknown6 { get; set; }
        public float Unknown7 { get; set; }
    }

    public class HSD_CameraInfo : IHSDNode
    {
        public float Unknown1 { get; set; }
        public float Unknown2 { get; set; }
        public float Unknown3 { get; set; }
        public float Unknown4 { get; set; }
        public float Unknown5 { get; set; }
    }

}
