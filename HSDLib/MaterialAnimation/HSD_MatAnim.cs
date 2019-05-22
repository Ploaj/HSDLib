using System;
using HSDLib.Animation;

namespace HSDLib.MaterialAnimation
{
    public class HSD_MatAnim : IHSDList<HSD_MatAnim>
    {
        public override HSD_MatAnim Next { get; set; }
        
        public HSD_AOBJ AnimationObject { get; set; }
        
        public HSD_TexAnim TextureAnimation { get; set; }

        //render anim
        public int Renderanim { get; set; }

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);
            if (Renderanim != 0)
                throw new Exception("Error reading MatAnim");
        }
    }
}
