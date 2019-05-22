namespace HSDLib.Common
{
    public class HSD_DOBJ : IHSDList<HSD_DOBJ>
    {
        public uint NameOffset { get; set; }
        
        public override HSD_DOBJ Next { get; set; }
        
        public HSD_MOBJ MOBJ { get; set; }
        
        public HSD_POBJ POBJ { get; set; }
    }
}
