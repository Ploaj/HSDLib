namespace HSDLib.MaterialAnimation
{
    public class HSD_MatAnimJoint : IHSDTree<HSD_MatAnimJoint>
    {
        public override HSD_MatAnimJoint Child { get; set; }
        
        public override HSD_MatAnimJoint Next { get; set; }
        
        public HSD_MatAnim MaterialAnimation { get; set; }
    }
}
