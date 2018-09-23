using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDLib.MaterialAnimation
{
    public class HSD_MatAnim : IHSDList<HSD_MatAnim>
    {
        [FieldData(typeof(HSD_MatAnim))]
        public override HSD_MatAnim Next { get; set; }
        
        [FieldData(typeof(HSD_AOBJ))]
        public HSD_AOBJ AnimationObject { get; set; }
        
        [FieldData(typeof(HSD_TexAnim))]
        public HSD_TexAnim TextureAnimation { get; set; }

        //render anim
        [FieldData(typeof(int))]
        public int Renderanim { get; set; }

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);
            if (Renderanim != 0)
                throw new Exception("Error reading MatAnim");
        }
    }
}
