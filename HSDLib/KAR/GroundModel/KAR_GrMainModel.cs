using HSDLib.Common;

namespace HSDLib.KAR
{
    public class KAR_GrMainModel : IHSDNode
    {
        public HSD_JOBJ JOBJRoot { get; set; }

        public uint Unk1 { get; set; }

        public uint Unk2 { get; set; }

        public uint Unk3 { get; set; }

        public KAR_GrModel_Bounding ModelBounding { get; set; }
    }
}
